using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
namespace Chat{
	public class ClientInfo{
		public TcpClient Client;
		public string Name;
		
		public ClientInfo(TcpClient client,string name){
			Name=name;Client=client;
		}
		public static Packet ToPacket(ClientInfo client){
			Packet pack = new Packet(-1,-2,null,5);
			byte[] name = Encoding.ASCII.GetBytes(client.Name);
			pack.data=name;
			return pack;
		}
		public static ClientInfo FromPacket(Packet packet,TcpClient client){
			string name = Encoding.ASCII.GetString(packet.data);
			return new ClientInfo(client,name);
		}
}
}