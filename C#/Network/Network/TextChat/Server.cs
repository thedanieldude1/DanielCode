using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Specialized;
using System.Net.NetworkInformation;
namespace Chat{
	public class Server{
		public List<ClientInfo> clients = new List<ClientInfo>();
		public TcpListener server = new TcpListener(IPAddress.Any,8888);
		public List<string> log = new List<string>();
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
                Console.WriteLine("Listening For Clients");
                clients.Add(clientinfo);
				Console.WriteLine("SERVER: " +clientinfo.Name+" has joined");
				int g = clients.IndexOf(clientinfo);
                
                //BroadcastData(Packet.ToBytes(new Packet(-3, -1, Encoding.ASCII.GetBytes(clientinfo.Name + " has joined"), 0)), null);// (x)=>c!=clients.IndexOf(x));
				
				Task.Run( async ()=>{ int c = g; await ProcessIncomingDataAsync(c);});
                BroadcastData(Packet.ToBytes(new Packet(0, 0, Encoding.ASCII.GetBytes(clientinfo.Name + " has joined"), 0)), null);
                List<Packet> packets = new List<Packet>();
				for(int i = 0;i<log.Count; i++){
                    var s = log[i];
                    var p = new Packet(-2, -1, Encoding.ASCII.GetBytes(s), 0);

                    packets.Add(p);
                    //Console.WriteLine(s + " " +Packet.ToBytes(p).Length);

                }
				log.Add(clientinfo.Name+" has joined");
                if (packets.Count > 0)
                {
                    SendData(Packet.ArrayToBytes(packets.ToArray()), client);

                    //Console.WriteLine("Sending Log of length "+log.Count);
                }
			}
		}
		public void SendData(byte[] data,TcpClient _client){
			var stream = _client.GetStream();
			byte[] buffer = new byte[data.Length+4];
			byte[] count = BitConverter.GetBytes(data.Length);
			Array.Copy(count,buffer,count.Length);
            Console.WriteLine("Sending Data to " + clients.IndexOf(clients.Find((x) => x.Client == _client)) + " of size " + data.Length);
            Array.Copy(data,0,buffer,4,data.Length);
            
			stream.Write(buffer,0,buffer.Length);
		}
		public async Task ProcessIncomingDataAsync(int meme){
            var info = clients[meme];
            var client = info.Client;
			while(client.GetState()!=TcpState.Closed){
				var stream = client.GetStream();
            	byte[] length = new byte[4];
                await stream.ReadAsync(length, 0, 4);
            	int count = BitConverter.ToInt32(length, 0);
            	byte[] buffer = new byte[count];
               // stream.Read(buffer, 0, count);
                Console.WriteLine("Receiving Data of size: " + stream.Read(buffer, 0, count));
                Packet[] newpackets = Packet.ArrayFromBytes(buffer);
                stream.Flush();
                ProcessPacketsAsync(newpackets,client);
				
			}
            Disconnect(info);
        }
        public void ProcessPacketsAsync(Packet[] packet, TcpClient info){
			foreach(Packet p in packet){
				switch(p.PacketType){
					case 0:
						var strea = Encoding.ASCII.GetString(p.data);
						Console.WriteLine(strea);
						log.Add(strea);
						//int c = clients.IndexOf(info);
                        //int g = c;
                        foreach (ClientInfo x in clients)
                        {
                            if (info != x.Client)
                            {
                                SendData(Packet.ToBytes(new Packet(0, 0, Encoding.ASCII.GetBytes(strea), 0)),x.Client);
                            }
                        }
						break;
					case 2:
                       //var clientinfo = clients.Find((x) => x.Client == info);
                       // Disconnect(clientinfo);
						break;
					default:
						Console.WriteLine("Unrecognized Packet Recieved");
						break;
				}
			}
		}
        public void Disconnect(ClientInfo clientinfo)
        {
            BroadcastData(Packet.ToBytes(new Packet(-3, -1, Encoding.ASCII.GetBytes(clientinfo.Name + " has disconnected"), 0)));
            log.Add(clientinfo.Name + " has disconnected");
            Console.WriteLine(clientinfo.Name + " has disconnected");
            clientinfo.Client.Client.Close();
            clients.Remove(clientinfo);
        }
		public void BroadcastData(byte[] data,Func<ClientInfo,bool> filter = null){


			if(filter!=null){
                byte[] buffer = new byte[data.Length + 4];
                byte[] count = BitConverter.GetBytes(data.Length);
                Array.Copy(count, buffer, count.Length);
                Array.Copy(data, 0, buffer, 4, data.Length);
                foreach (ClientInfo x in clients){if(filter(x)) x.Client.GetStream().Write(buffer,0,buffer.Length);}
			}
			else{

                for (int i = 0; i < clients.Count; i++)
                {
                    SendData(data, clients[i].Client);
                    
                }
				}
				
			}
		}
	}

