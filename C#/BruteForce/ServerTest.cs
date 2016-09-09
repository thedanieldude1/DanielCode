using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Request = RequestHandler.Request;
public class ServerTest{
	public const int port = 8888;
	IPAddress ip = IPAddress.Any;
	TcpListener _server = new TcpListener(IPAddress.Any,8888);
	public List<ConnectedClient> _clients = new List<ConnectedClient>();
    public Dictionary<int, Request> Requests = new Dictionary<int, Packet>();
    public Dictionary<int, Packet> Responses = new Dictionary<int, Packet>();
    public List<Packet> JunctionedData = new List<Packet>();
    public List<Packet> FromClientData = new List<Packet>();
	public ServerTest(){ 
		_server.Start();
		Task.Run(()=>{StartAcceptingConnectionsAsync();});
        Task.Run(() => { NetworkJunctionAsync(); });
	}
	public async Task StartAcceptingConnectionsAsync(){
		while(true){
			var _client = await _server.AcceptTcpClientAsync();
			Console.WriteLine("CLIENT CONNECTED");
			var conclient = new ConnectedClient(_client,this);
			_clients.Add(conclient);
			Task.Run(()=>{ProcessIncomingDataAsync(conclient);});
            Task.Run(()=>{ProcessOutgoingDataAsync(conclient);});
		}
	}
    public async Task NetworkJunctionAsync() {
        for (int i = 0; i < JunctionedData.Count; i++)
        {
            var client = _clients[JunctionedData[i].Reciever];
            client.FromClientData.Add(JunctionedData[i]);
        }
    }
    public async Task ProcessOutgoingDataAsync(ConnectedClient _client)
    {
        var client = _client.Client;
        while (true)
        {

            var stream = client.GetStream();
            //	byte[] length = new byte[4];
            //	await stream.ReadAsync(length,0,4);
            //	int count = BitConverter.ToInt32(length,0);
            //	//stream.Read(buffer,0,count);

            //Packet[] newpackets = Packet.ArrayFromBytes(buffer);

            //JunctionedData.AddRange(newpackets.ToList());
            //Console.WriteLine(BitConverter.ToInt32(newpackets[0].data,0)+" "+newpackets.Length);
            if (_client.ToClientData.Count > 0)
            {
                var outgoing = Packet.ArrayToBytes(_client.ToClientData.ToArray());
                // JunctionedData.AddRange(_client.OutgoingData);
                _client.ToClientData.Clear();
                //stream.Flush();
                SendData(outgoing, client);
            }

        }
    }
	public async Task ProcessIncomingDataAsync(ConnectedClient _client){
		var client = _client.Client;
		while(true){
				
				var stream = client.GetStream();
					byte[] length = new byte[4];
					await stream.ReadAsync(length,0,4);
					int count = BitConverter.ToInt32(length,0);
					byte[] buffer = new byte[count];
					stream.Read(buffer,0,count);

					Packet[] newpackets = Packet.ArrayFromBytes(buffer);
                    
					JunctionedData.AddRange(newpackets.ToList());
					//Console.WriteLine(BitConverter.ToInt32(newpackets[0].data,0)+" "+newpackets.Length);
                    //var outgoing = Packet.ArrayToBytes(_client.OutgoingData.ToArray());
                   // JunctionedData.AddRange(_client.OutgoingData);
					//_client.OutgoingData.Clear();
					stream.Flush();
					//SendData(outgoing,client);

			
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
	
	public class ConnectedClient{
		public TcpClient Client;
		public ServerTest server;
		public List<Packet> FromClientData= new List<Packet>();
		public List<Packet> ToClientData = new List<Packet>();
        
		public int ID{
			get{
				return server._clients.IndexOf(this);
			}
		}
        public ConnectedClient(TcpClient client, ServerTest server) { Client = client; this.server = server;}
        public void ProcessData(Packet packet)
        {

        } 
	}

}
	public struct Packet{
		public int Reciever;
		public int Sender;
		public byte[] data;
        public int PacketType;
		public Packet(int Reciever,int Sender,byte[] data,int type){
            this.Reciever = Reciever; this.Sender = Sender; this.data = data; PacketType = type;
		}
		public static byte[] ToBytes(Packet packet){
			byte[] buffer = new byte[16+packet.data.Length];
			var sender = BitConverter.GetBytes(packet.Sender);
			Array.Copy(sender,buffer,4);
			Array.Copy(BitConverter.GetBytes(packet.Reciever),0,buffer,4,4);
            Array.Copy(BitConverter.GetBytes(packet.PacketType), 0, buffer, 8, 4);
			Array.Copy(BitConverter.GetBytes(packet.data.Length),0,buffer,12,4);
			Array.Copy(packet.data,0,buffer,16,packet.data.Length);
			return buffer;

		}
		public static byte[] ArrayToBytes(Packet[] packets){
			int byteamount = 0;
			foreach(Packet p in packets){byteamount+=16+p.data.Length;}
			byte[] buffer=new byte[byteamount];
			for(int i = 0;i<packets.Length;i++){
			var packet = packets[i];
			var sender = BitConverter.GetBytes(packet.Sender);
			Array.Copy(sender,buffer,4);
			Array.Copy(BitConverter.GetBytes(packet.Reciever),0,buffer,4,4);
            Array.Copy(BitConverter.GetBytes(packet.PacketType), 0, buffer, 8, 4);
			Array.Copy(BitConverter.GetBytes(packet.data.Length),0,buffer,12,4);
			Array.Copy(packet.data,0,buffer,16,packet.data.Length);
			
			}
			return buffer;
		}

		public static Packet FromBytes(byte[] buffer){
			var sender = new byte[4];
			Array.Copy(buffer,0,sender,0,4);
			int Sender = BitConverter.ToInt32(sender,0);
			Array.Copy(buffer,4,sender,0,4);
			int Reciever = BitConverter.ToInt32(sender,0);
            Array.Copy(buffer, 8, sender, 0, 4);
            int type = BitConverter.ToInt32(sender, 0);
			byte[] data = new byte[buffer.Length-16];
			Array.Copy(buffer,16,data,0,data.Length);
			return new Packet(Reciever,Sender,data,type);
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
            Array.Copy(buffer, 8 + currentPos, sender, 0, 4);
            int type = BitConverter.ToInt32(sender, 0);
			Array.Copy(buffer,12+currentPos,sender,0,4);
			int datalength = BitConverter.ToInt32(sender,0);
			byte[] data = new byte[datalength];
			Array.Copy(buffer,16+currentPos,data,0,datalength);
			currentPos+=data.Length+16;	
			packets.Add(new Packet(Reciever,Sender,data,type));
			if(currentPos>=buffer.Length){EndNotReached=false;}
			}
			return packets.ToArray();
		}
	}
    public class ClientPacketProcessor
    {
        
        public ClientPacketProcessor() { }
        public Packet ProcessData(Packet packet)
        {
            Request request = Request.FromPacket(packet);
            switch (packet.PacketType)
            {
                
                case 100:
                    switch(request.RequestType){
                        case 0:
                            Packet response = new Packet(packet.Sender, packet.Receiver, packet.data, 200);
                            return response;
                            break;
                    }
            break;
                case 200:
            Console.WriteLine("Request Obtained");
            }
            
        }
    }
public class ClientTest{
	public TcpClient _client = new TcpClient();
	TcpListener _server;
	public List<Packet> FromServerData= new List<Packet>();
	public List<Packet> ToServerData = new List<Packet>();
    public Dictionary<int, Packet> Requests = new Dictionary<int, Packet>();
    public Dictionary<int, Packet> Responses = new Dictionary<int, Packet>();
	public ClientTest(){
		_client.Connect(IPAddress.Parse("127.0.0.1"),8888);
		Task.Run(()=>{ProcessOutgoingDataAsync();});
        Task.Run(() => { ProcessIncomingDataAsync(); });
	}
    public async Task ProcessOutgoingDataAsync()
    {
        var client = _client;
        while (true)
        {

            var stream = client.GetStream();
            //	byte[] length = new byte[4];
            //	await stream.ReadAsync(length,0,4);
            //	int count = BitConverter.ToInt32(length,0);
            //	//stream.Read(buffer,0,count);

           // Packet[] newpackets = Packet.ArrayFromBytes(buffer);
            if (ToServerData.Count > 0)
            {
                //JunctionedData.AddRange(newpackets.ToList());
                //Console.WriteLine(BitConverter.ToInt32(newpackets[0].data,0)+" "+newpackets.Length);
                var outgoing = Packet.ArrayToBytes(ToServerData.ToArray());
                // JunctionedData.AddRange(_client.OutgoingData);
                //_client.OutgoingData.Clear();
                //stream.Flush();
                WriteData(outgoing);
            }

        }
    }
    public async Task ProcessIncomingDataAsync()
    {
        var client = _client;
        while (true)
        {

            var stream = client.GetStream();
            byte[] length = new byte[4];
            await stream.ReadAsync(length, 0, 4);
            int count = BitConverter.ToInt32(length, 0);
            byte[] buffer = new byte[count];
            stream.Read(buffer, 0, count);

            Packet[] newpackets = Packet.ArrayFromBytes(buffer);

            FromServerData.AddRange(newpackets.ToList());
            //Console.WriteLine(BitConverter.ToInt32(newpackets[0].data,0)+" "+newpackets.Length);
            //var outgoing = Packet.ArrayToBytes(_client.OutgoingData.ToArray());
            // JunctionedData.AddRange(_client.OutgoingData);
            //_client.OutgoingData.Clear();
            stream.Flush();
            //SendData(outgoing,client);


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
public class RequestHandler{
    public Dictionary<int, Request> Requests = new Dictionary<int, Request>();
    public Dictionary<int, Request> Responses = new Dictionary<int, Request>();
    public async Task<Response> GetRequest(Request request)
    {
        while (true)
        {
            if(Responses.Exists)
        }
    }
    
    public struct Request {
        public int Receiver;
        public int Sender;
        public byte[] data;
        public int RequestType;
        public int PacketType;
        public Request(int Receiver, int Sender, byte[] data,int Type, int Requesttype)
        {
            this.PacketType = Type; this.Receiver = Receiver; this.Sender = Sender; this.data = data; RequestType = Requesttype;
        }
        
        public static Packet ToPacket(Request request)
        {
            Packet pack = new Packet(request.Receiver, request.Sender, request.data,request.PacketType);
            byte[] bytes = Packet.ToBytes(pack);
            byte[] yes = new byte[bytes.Length+4];
            Array.Copy(bytes, 0, yes, 0, 8);
            byte[] si = BitConverter.GetBytes(4 + request.data.Length);
            Array.Copy(si, 0, yes, 8, 4);
            Array.Copy(BitConverter.GetBytes(request.RequestType),0,yes,12,4);
            //Array.Copy(data, 0, si, 4, data.Length);
           
            Array.Copy(request.data, 0, yes, 16,request.data.Length);
            return Packet.FromBytes(yes);
        }
        public static Request FromPacket(Packet request)
        {

            byte[] ok = new byte[] { request.data[0], request.data[1], request.data[2], request.data[3] };
            var type = BitConverter.ToInt32(ok, 0);
            return new Request(request.Reciever, request.Sender, request.data,request.PacketType, type);
        }
    }
    public struct Response
    {
        public int Receiver;
        public int Sender;
        public byte[] data;
        public int RequestType;
        public int PacketType;
        public Response(int Receiver, int Sender, byte[] data,int Type, int Requesttype)
        {
            this.PacketType = Type; this.Receiver = Receiver; this.Sender = Sender; this.data = data; RequestType = Requesttype;
        }
        
        public static Packet ToPacket(Response request)
        {
            Packet pack = new Packet(request.Receiver, request.Sender, request.data,request.PacketType);
            byte[] bytes = Packet.ToBytes(pack);
            byte[] yes = new byte[bytes.Length+4];
            Array.Copy(bytes, 0, yes, 0, 8);
            byte[] si = BitConverter.GetBytes(4 + request.data.Length);
            Array.Copy(si, 0, yes, 8, 4);
            Array.Copy(BitConverter.GetBytes(request.RequestType),0,yes,12,4);
            //Array.Copy(data, 0, si, 4, data.Length);
           
            Array.Copy(request.data, 0, yes, 16,request.data.Length);
            return Packet.FromBytes(yes);
        }
        public static Response FromPacket(Packet request)
        {

            byte[] ok = new byte[] { request.data[0], request.data[1], request.data[2], request.data[3] };
            var type = BitConverter.ToInt32(ok, 0);
            return new Request(request.Reciever, request.Sender, request.data,request.PacketType, type);
        }
    }
}
public static class main{
	public static void Main(string[] args){
		var server = new ServerTest();
		var client = new ClientTest();
var client2 = new ClientTest();
var outputs = BitConverter.GetBytes(1337);
Packet testpack = new Packet(-1,1,outputs,52);
var test = Packet.ToBytes(new Packet[]{testpack}[0]);
var test2 = Packet.FromBytes(test);
Console.WriteLine(test2.PacketType);
//RequestHandler.Request testx = new RequestHandler.Request(-1, 1, BitConverter.GetBytes(1231), 32);
  //  Packet test5 = RequestHandler.Request.ToPacket(testx);
  //  Console.WriteLine(RequestHandler.Request.FromPacket(test5).RequestType);
		Console.ReadLine();
//server.SendData(BitConverter.GetBytes(3021),client2._client);
client2.WriteData(Packet.ToBytes(testpack));
Console.ReadLine();
	}
}