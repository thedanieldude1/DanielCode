using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
namespace XmlSerialization
{
    [Serializable]
    public struct Dict {
        public dynamic Key;
        public dynamic Value;
        public Dict(dynamic s, dynamic t) {
            Key = s;
            Value = t;
        }
    }
    class Program
    {
        static void console() {

            string[] command = Console.ReadLine().Split(" ".ToCharArray());
            switch (command[0])
            {
                case "Write":
                    try
                    {
                        XmlSerializer xml1 = new XmlSerializer(typeof(List<Dict>));
                        FileStream file1 = new FileStream("test.xml", FileMode.OpenOrCreate);
                        List<Dict> list1 = (List<Dict>)xml1.Deserialize(file1);
                        file1.Close();
                        list1.Add(new Dict(command[1], command[2]));
                        StreamWriter stream = new StreamWriter("test.xml");
                        xml1.Serialize(stream, list1);
                        stream.Close();
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        Console.WriteLine("Error, Please retry command");
                    }
                    console();
                    break;
                case "Read":
                    XmlSerializer xml = new XmlSerializer(typeof(List<Dict>));
                    FileStream file = new FileStream("test.xml", FileMode.Open);
                    List<Dict> list = (List<Dict>)xml.Deserialize(file);
                    for (int i = 0; i < list.Count(); i++)
                    {
                        Console.WriteLine(list[i].Key + " " + list[i].Value);
                    }
                    file.Close();
                    console();
                    break;
            }
        }
        static void Main(string[] args)
        {
            var doc = new XmlDocument();
            try
            {
                doc.Load("test.xml");
            }
            catch (FileNotFoundException e) {
                StreamWriter stream = new StreamWriter("test.xml");
                XmlSerializer xml1 = new XmlSerializer(typeof(List<Dict>));
                xml1.Serialize(stream, new List<Dict>());
                stream.Close();
            }
            catch (XmlException e)
            {

                StreamWriter stream = new StreamWriter("test.xml");
                XmlSerializer xml1 = new XmlSerializer(typeof(List<Dict>));
                xml1.Serialize(stream, new List<Dict>());
                stream.Close();
            }

            console();
            //int test= 10;
            //string varname =GetNameOfVariable(new {test});
            //Dict dict = new Dict(varname,test);
            //XmlSerializer xml = new XmlSerializer(typeof(Dict));
            //StreamWriter file = new StreamWriter("test.xml");
           // xml.Serialize(file, dict);
            
            //FileStream stream = new FileStream("test.xml", FileMode.Open);
            //Dict testA = (Dict)xml.Deserialize(stream);
            //Console.WriteLine(testA.Key + " " + testA.Value);
            //Console.ReadLine();
        }
        public static string GetNameOfVariable<T>(T variable) where T : class {
            return typeof(T).GetProperties()[0].Name;
        }
    }
}
