using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;
using System.IO;
namespace ConsoleApplication1
{
    class Program
    {
        public static SerialPort port;
        public static void console() {
            string[] command = Console.ReadLine().Split(" ".ToCharArray());
            switch (command[0]) {
                case "ON":
                    port.Write("ON");
                    console();
                    break;
                case "OFF":
                    port.Write("OFF");
                    console();
                    break;
                case "READ":
                    Read();
                    console();
                    break;
            }
        }
        public static void Main(string[] args)
        {
            port = new SerialPort("COM2", 9600);
            port.Open();
            Program main = new Program();
            console();

        }
        public static void Read()
        {
            var recieved = "";
            var isReading = true;
            while (isReading)
            {
                recieved += port.ReadExisting();
                if (recieved.Contains('!'));
                isReading = false;
            }
            Console.WriteLine(recieved);
        }
    }
}
