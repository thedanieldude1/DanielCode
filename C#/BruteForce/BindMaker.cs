using System;
using System.IO;
using System.Collections.Generic;
public static class BindMaker
{
    public static bool IsNothing(string does)
    {
        
        char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_=+[{]}".ToCharArray();
        for(int i =0;i<chars.Length;i++){
            if(does.Contains(chars[i].ToString())){
            return false;
            }
        }
        return true;
    }
    public static T[] RemoveAt<T>(this T[] source, int index)
    {
        T[] dest = new T[source.Length - 1];
        if (index > 0)
            Array.Copy(source, 0, dest, 0, index);

        if (index < source.Length - 1)
            Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

        return dest;
    }
    public static void Main(string[] args)
    {
        string quote = '"'.ToString();
        try
        {
            string[] lines = File.ReadAllLines(@"input.txt");
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "" || lines[i] == "  "||IsNothing(lines[i]))
                {
                   lines= lines.RemoveAt(i);
                }
            }
            File.WriteAllText(@"output.txt", String.Empty);
            using (StreamWriter writer = new StreamWriter(@"output.txt"))
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    writer.WriteLine("alias " + "bind" + i + " "+quote+"say " + lines[i] + quote);
                }
                int amount = 0;
                int end = 0;
                for (int i = 0; i < (int)Math.Ceiling((double)lines.Length / 5); i++)
                {
                    writer.Write("alias " + "bindSet" + i + " "+quote);
                    for (int k = 0; k < 5; k++)
                    {
                        if (amount >= lines.Length) { break; }
                        writer.Write("bind"+amount+";wait 300;");
                        amount++;
                    }
                    if(i+1<(int)Math.Ceiling((double)lines.Length / 5)){
                    writer.Write("bindSet"+(i+1));
                    }
                    else
                    {
                        end = i;
                    }
                    writer.WriteLine(quote);
                }
                writer.Write("bind KP_5 bindSet0");
            }
        }



        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.ReadLine();
        }

    }
}
