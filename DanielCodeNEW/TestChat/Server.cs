using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Specialized;
namespace Chat{
	public class Server{
		public List<ClientInfo> clients = new List<ClientInfo>();
		public TcpListener server = new TcpListener(IPAddress.Any,8888);
		public StringCollection log = new StringCollection();
		public Server(){
			server.Start();
			Task.Run(async ()=>{await ClientConnectionAsync();});
		}
		public async Task ClientConnectionAsync(){
			while(true){
				var client = await server.AcceptTcpClientAsync();
				var stream = client.GetStream();
				byte[] length = new byte[4];
				await stream.ReadAsync(length,0,4);
				int count = BitConverter.ToInt32(length,0);
				byte[] buffer = new byte[count];
				stream.Read(buffer,0,count);
				Packet info = Packet.FromBytes(buffer);
				var clientinfo = ClientInfo.FromPacket(info,client);
                    		clients.Add(clientinfo);
				Console.WriteLine("SERVER: " +clientinfo.Name+" has joined");
				int c = clients.IndexOf(clientinfo);
				BroadcastData(Packet.ToBytes(new Packet(-3,-1,Encoding.ASCII.GetBytes(clientinfo.Name+" has joined"),0)),x=>c!=clients.IndexOf(x));
				
				Task.Run(async ()=>{await ProcessIncomingDataAsync(clients.IndexOf(clientinfo));});
				List<Packet> packets = new List<Packet>();
				foreach(string s in log){
					packets.Add(new Packet(-2,-1,Encoding.ASCII.GetBytes(s),0));
				}
				log.Add(clientinfo.Name+" has joined");
				SendData(Packet.ArrayToBytes(packets.ToArray()),client);
			}
		}
		public void SendData(byte[] data,TcpClient _client){
			var stream = _client.GetStream();
			byte[] buffer = new byte[data.Length+4];
			byte[] count = BitConverter.GetBytes(data.Length);
			Array.Copy(count,buffer,count.Length);
			Array.Copy(data,0,buffer,4,data.Length);
			
			stream.Write(buffer,0,buffer.Length);
		}
		public async Task ProcessIncomingDataAsync(int Index){
			var info = clients[Index];
			var client = info.Client;
			while(clients.Contains(info)){
				var stream = client.GetStream();
            			byte[] length = new byte[4];
            			await stream.ReadAsync(length, 0, 4);
            			int count = BitConverter.ToInt32(length, 0);
            			byte[] buffer = new byte[count];
            			stream.Read(buffer, 0, count);
				Packet[] newpackets = Packet.ArrayFromBytes(buffer);
				Task.Run(async ()=>{await ProcessPacketsAsync(newpackets,info);});
				stream.Flush();
			}
		}
		public async Task ProcessPacketsAsync(Packet[] packet, ClientInfo info){
			foreach(Packet p in packet){
				switch(p.PacketType){
					case 0:
						var strea = Encoding.ASCII.GetString(p.data);
						Console.WriteLine(strea);
						log.Add(strea);
						int c = clients.IndexOf(info);
						BroadcastData(Packet.ToBytes(new Packet(-3,-1,Encoding.ASCII.GetBytes(strea),0)),(x)=>clients.IndexOf(x)!=c);
						break;
					case 2:
						BroadcastData(Packet.ToBytes(new Packet(-3,-1,Encoding.ASCII.GetBytes(info.Name+" has disconnected"),0)));
						log.Add(info.Name+" has disconnected");
						Console.WriteLine(info.Name+" has disconnected");
						info.Client.Close();
						clients.Remove(info);
						break;
					default:
						Console.WriteLine("Unrecognized Packet Recieved");
						break;
				}
			}
		}
		public void BroadcastData(byte[] data,Func<ClientInfo,bool> filter = null){

			byte[] buffer = new byte[data.Length+4];
			byte[] count = BitConverter.GetBytes(data.Length);
			Array.Copy(count,buffer,count.Length);
			Array.Copy(data,0,buffer,4,data.Length);
			if(filter!=null){
				foreach(ClientInfo x in clients){if(filter(x)) x.Client.GetStream().Write(buffer,0,buffer.Length);}
			}
			else{
				foreach(ClientInfo x in clients){
					x.Client.GetStream().Write(buffer,0,buffer.Length);
				}
				
			}
		}
	}

}