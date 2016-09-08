
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
namespace Chat{
	public static class Program{
		static Server server;
		static Client client;
		public static void Main(string[] args){




			if(args.Length>0){
				server = new Server();
				Console.ReadLine();
			}
			else{
				Console.WriteLine("NAME: ");
				client = new Client(Console.ReadLine());
				ClientConsole();
			}
		
		}
	
		public static void ClientConsole(){

			string s= Console.ReadLine();
			string[] args = s.Split(" ".ToCharArray());
			
			switch(args[0]){
				case "CONNECT":
					try{
						Console.Clear();
						client.Connect(args[1],Int32.Parse(args[2]));
					}
					catch{Console.WriteLine("Error");}
				break;
				case "DISCONNECT":
					client.Disconnect();
					break;
				default:
					string message = client.clientinfo.Name+": "+s;
					Console.SetCursorPosition(0,Console.CursorTop-1);
					Console.Write(new String(' ',Console.BufferWidth-1));
					Console.SetCursorPosition(0,Console.CursorTop);
					Console.WriteLine(message);
					//Console.WriteLine(message);
					client.WriteData(Packet.ToBytes(new Packet(-2,-1,Encoding.ASCII.GetBytes(message),0)));
				break;
			}
			ClientConsole();
		}	
	}
		
}