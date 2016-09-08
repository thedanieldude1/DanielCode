using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

public class ServerTest{
	public const int port = 8888;
	IPAddress ip = IPAddress.Any;
	TcpListener _server = new TcpListener(IPAddress.Any,8888);
	public List<ConnectedClient> _clients = new List<ConnectedClient>();
	public ServerTest(){
		_server.Start();
		Task.Run(()=>{StartAcceptingConnectionsAsync();});
		
	}
	public async Task StartAcceptingConnectionsAsync(){
		while(true){
			var _client = await _server.AcceptTcpClientAsync();
			Console.WriteLine("CLIENT CONNECTED");
			var conclient = new ConnectedClient(_client);
			_clients.Add(conclient);
			Task.Run(()=>{ProcessDataAsync(conclient);});
		}
	}
	public async Task ProcessDataAsync(ConnectedClient _client){
		var client = _client.Client;
		while(true){
				
				var stream = client.GetStream();
					byte[] length = new byte[4];
					await stream.ReadAsync(length,0,4);
					int count = BitConverter.ToInt32(length,0);
					byte[] buffer = new byte[count];
					stream.Read(buffer,0,count);

					Packet[] newpackets = Packet.ArrayFromBytes(buffer);
					_client.IncomingData.AddRange(newpackets.ToList());
					Console.WriteLine(BitConverter.ToInt32(newpackets[0].data,0)+" "+newpackets.Length);
					var outgoing = Packet.ArrayToBytes(_client.OutgoingData.ToArray());
					_client.OutgoingData.Clear();
					stream.Flush();
					SendData(outgoing,client);

			
		}
	}
	public void SendData(byte[] data,TcpClient _client){
		var stream = _client.GetStream();
		byte[] buffer = new byte[data.Length+4];
		byte[] count = BitConverter.GetBytes(data.Length);
		Array.Copy(count,buffer,count.Length);
		Array.Copy(data,0,buffer,4,data.Length);
		Console.WriteLine(data.Length);
		stream.Write(buffer,0,buffer.Length);
	}
	public void consolewrite(string yes){Console.WriteLine(yes);}
	public class ConnectedClient{
		public TcpClient Client;
		public ServerTest server;
		public List<Packet> IncomingData= new List<Packet>();
		public List<Packet> OutgoingData = new List<Packet>();
		public int ID{
			get{
				return server._clients.IndexOf(this);
			}
		}
		public ConnectedClient(TcpClient client,ServerTest server){Client=client;this.server=server}
	}

}
	public struct Packet{
		public int Reciever;
		public int Sender;
		public byte[] data;
		public Packet(int Reciever,int Sender,byte[] data){
			this.Reciever = Reciever;this.Sender=Sender;this.data=data;
		}
		public static byte[] ToBytes(Packet packet){
			byte[] buffer = new byte[12+packet.data.Length];
			var sender = BitConverter.GetBytes(packet.Sender);
			Array.Copy(sender,buffer,4);
			Array.Copy(BitConverter.GetBytes(packet.Reciever),0,buffer,4,4);
			Array.Copy(BitConverter.GetBytes(packet.data.Length),0,buffer,8,4);
			Array.Copy(packet.data,0,buffer,12,packet.data.Length);
			return buffer;

		}
		public static byte[] ArrayToBytes(Packet[] packets){
			int byteamount = 0;
			foreach(Packet p in packets){byteamount+=12+p.data.Length;}
			byte[] buffer=new byte[byteamount];
			for(int i = 0;i<packets.Length;i++){
			var packet = packets[i];
			var sender = BitConverter.GetBytes(packet.Sender);
			Array.Copy(sender,buffer,4);
			Array.Copy(BitConverter.GetBytes(packet.Reciever),0,buffer,4,4);
			Array.Copy(BitConverter.GetBytes(packet.data.Length),0,buffer,8,4);
			Array.Copy(packet.data,0,buffer,12,packet.data.Length);
			
			}
			return buffer;
		}

		public static Packet FromBytes(byte[] buffer){
			var sender = new byte[4];
			Array.Copy(buffer,0,sender,0,4);
			int Sender = BitConverter.ToInt32(sender,0);
			Array.Copy(buffer,4,sender,4,4);
			int Reciever = BitConverter.ToInt32(sender,0);
			byte[] data = new byte[buffer.Length-12];
			Array.Copy(buffer,12,data,0,buffer.Length);
			return new Packet(Reciever,Sender,data);
		}
		public static Packet[] ArrayFromBytes(byte[] buffer){
			List<Packet> packets = new List<Packet>();
			bool EndNotReached = true;
			int currentPos=0;
			while(EndNotReached){
			var sender = new byte[4];
			Array.Copy(buffer,0+currentPos,sender,0,4);
			int Sender = BitConverter.ToInt32(sender,0);
			Array.Copy(buffer,4+currentPos,sender,0,4);
			int Reciever = BitConverter.ToInt32(sender,0);
			Array.Copy(buffer,8+currentPos,sender,0,4);
			int datalength = BitConverter.ToInt32(sender,0);
			byte[] data = new byte[datalength];
			Array.Copy(buffer,12+currentPos,data,0,datalength);
			currentPos+=data.Length+12;	
			packets.Add(new Packet(Reciever,Sender,data));
			if(currentPos>=buffer.Length){EndNotReached=false;}
			}
			return packets.ToArray();
		}
	}
public class ClientTest{
	public TcpClient _client = new TcpClient();
	TcpListener _server;
	public List<Packet> IncomingData= new List<Packet>();
	public List<Packet> OutgoingData = new List<Packet>();
	public ClientTest(){
		_client.Connect(IPAddress.Parse("127.0.0.1"),8888);
		Task.Run(()=>{ProcessDataAsync();});
		
	}
	public async Task ProcessDataAsync(){
		while(true){
				
				var stream = _client.GetStream();
					byte[] length = new byte[4];

					await stream.ReadAsync(length,0,4);
					int count = BitConverter.ToInt32(length,0);
					byte[] buffer = new byte[count];
					stream.Read(buffer,0,count);
					Packet[] newpackets = Packet.ArrayFromBytes(buffer);
					IncomingData.AddRange(newpackets.ToList());
					var outgoing = Packet.ArrayToBytes(OutgoingData.ToArray());
					OutgoingData.Clear();
					stream.Flush();
					WriteData(outgoing);
			
		}
	}
	public void WriteData(byte[] data){
		var stream = _client.GetStream();
		byte[] buffer = new byte[data.Length+4];
		byte[] count = BitConverter.GetBytes(data.Length);
		Array.Copy(count,buffer,count.Length);
		Array.Copy(data,0,buffer,4,data.Length);
		Console.WriteLine(data.Length);
		stream.Write(buffer,0,buffer.Length);
	}
}
public static class main{
	public static void Main(string[] args){
		var server = new ServerTest();
		var client = new ClientTest();
var client2 = new ClientTest();
var outputs = BitConverter.GetBytes(1337);
Packet testpack = new Packet(-1,1,outputs);
var test = Packet.ArrayToBytes(new Packet[]{testpack});
var test2 = Packet.ArrayFromBytes(test);
Console.WriteLine(BitConverter.ToInt32(test2[0].data,0));

		Console.ReadLine();
//server.SendData(BitConverter.GetBytes(3021),client2._client);
client2.WriteData(Packet.ToBytes(testpack));
Console.ReadLine();
	}
}