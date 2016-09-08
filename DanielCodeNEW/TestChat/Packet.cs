using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
namespace Chat{
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
}