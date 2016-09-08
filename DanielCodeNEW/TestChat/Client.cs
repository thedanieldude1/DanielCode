using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.IO;
namespace Chat{
	public class Client{
		public TcpClient client;
		public ClientInfo clientinfo;
		bool _isConnected{
			get{
				return client.GetState()==TcpState.Established;
			}
		}
		public Client(string name){

			client= new TcpClient();
			clientinfo = new ClientInfo(client,name);
		}
		public async Task ProcessIncomingInformationAsync(){
			while(_isConnected){
				var stream = client.GetStream();
            			byte[] length = new byte[4];
            			await stream.ReadAsync(length, 0, 4);
				Console.WriteLine("Data Incoming");
            			int count = BitConverter.ToInt32(length, 0);
            			byte[] buffer = new byte[count];
            			stream.Read(buffer, 0, count);
				Packet[] newpackets = Packet.ArrayFromBytes(buffer);
				Task.Run(async ()=>{await ProcessPacketsAsync(newpackets);});
				stream.Flush();
			}
		}
		public async Task ProcessPacketsAsync(Packet[] packet){

			foreach(Packet p in packet){
				switch(p.PacketType){
					case 0:
						
						
						Console.WriteLine(Encoding.ASCII.GetString(p.data));
						
						break;
					default:
						Console.WriteLine("Unrecognized Packet Recieved");
						break;
				}
			}
			
		}
		public void Connect(string ip, int port){
			if(!_isConnected){
			client.Connect(IPAddress.Parse(ip),port);
			WriteData(Packet.ToBytes(new Packet(-1,-2,Encoding.ASCII.GetBytes(clientinfo.Name),1)));
			Console.WriteLine(_isConnected);
			Task.Run(async ()=>{await ProcessIncomingInformationAsync();});
			}
		}
		public void Disconnect(){
			if(_isConnected){
			WriteData(Packet.ToBytes(new Packet(-1,-2,new byte[1],2)));
			
			}
			else{
				Console.WriteLine("Not connected");
			}
		}
		public void WriteData(byte[] data){
		if(_isConnected){
		var stream = client.GetStream();
		byte[] buffer = new byte[data.Length+4];
		byte[] count = BitConverter.GetBytes(data.Length);
		Array.Copy(count,buffer,count.Length);
		Array.Copy(data,0,buffer,4,data.Length);
		stream.Write(buffer,0,buffer.Length);
		}
	}
	}
}