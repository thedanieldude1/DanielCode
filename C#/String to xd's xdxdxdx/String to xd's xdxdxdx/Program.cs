using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

    class Program
    {
    public static void Write(string input)
    {
        File.Delete("Output.txt");
        byte[] byterep = Encoding.ASCII.GetBytes(input);
        StringBuilder output = new StringBuilder();
        foreach (byte t in byterep)
        {
            BitArray b = new BitArray(new byte[] { t });
            for (int i = 0; i < 8; i++)
            {
                char x = b[i] ? 'x' : 'd';
                output.Append(x);
            }
            if (byterep.ToList().IndexOf(t)!=byterep.Length-1)
            {
                output.Append(" ");
            }
            
        }
        using (StreamWriter writer = new StreamWriter("Output.txt"))
        {
            writer.Write(output);
        }
    }
    public static string Read()
    {
        String xd;
        using(StreamReader reader = new StreamReader("Output.txt"))
        {
            xd = reader.ReadToEnd();
        }
        string[] c = xd.Split(' ');
        List<byte> outputs = new List<byte>();
        foreach(string x in c)
        {
            char[] d = x.ToCharArray();
            BitArray Out = new BitArray(8);
            if (d.Length != 8)
            {
                continue;
            }
            for(int i = 0; i < 8; i++)
            {
                Out[i] = d[i] == 'x';
            }
            outputs.Add(ConvertToByte(Out));
        }
        return Encoding.ASCII.GetString(outputs.ToArray());
    }
    public static byte ConvertToByte(BitArray bits)
    {
        if (bits.Count != 8)
        {
            throw new ArgumentException("illegal number of bits");
        }

        byte b = 0;
        if (bits.Get(0)) b++;
        if (bits.Get(1)) b += 2;
        if (bits.Get(2)) b += 4;
        if (bits.Get(3)) b += 8;
        if (bits.Get(4)) b += 16;
        if (bits.Get(5)) b += 32;
        if (bits.Get(6)) b += 64;
        if (bits.Get(7)) b += 128;
        return b;
    }
    public static void console()
    {
        string inputs = Console.ReadLine();
        string[] input = inputs.Split(' ');
        switch (input[0].ToLower())
        {
            case "translatetoxd":
                char[] x = inputs.ToCharArray();
                List<char> c = x.ToList();

                    c.RemoveRange(0, 14);
                
                Write(new String(c.ToArray()));
                break;
            case "translatefromxd":
                Console.WriteLine(Read());
                break;
            case "translatefromfile":
                String xd;
                using (StreamReader reader = new StreamReader("Output.txt"))
                {
                    xd = reader.ReadToEnd();
                }
                char[] r = xd.ToCharArray();
                List<char> g = r.ToList();

                g.RemoveRange(0, 14);

                Write(new String(g.ToArray()));
                break;
        }
        console();
    }
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Comand");
        //string input = Console.ReadLine();
            console();
        }
    }

