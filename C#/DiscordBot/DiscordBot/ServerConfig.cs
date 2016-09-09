using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.Commands.Permissions;
using Discord.Commands.Permissions.Levels;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
namespace DiscordBot1
{
    [Serializable]
    public class ServerConfig
    {
        public ulong ID;
        ServerConfig(ulong id)
        {
            ID = id;
        }
        public static bool CheckIfExists(ulong ID)
        {
            var doc = new XmlDocument();
            try
            {
                doc.Load("commands.xml");
            }
            catch (FileNotFoundException e)
            {
                StreamWriter stream = new StreamWriter("commands.xml");
                XmlSerializer xml1 = new XmlSerializer(typeof(List<CustomCommand>));
                xml1.Serialize(stream, new List<CustomCommand>());
                stream.Close();
                return false;
            }

            catch (XmlException e)
            {

                StreamWriter stream = new StreamWriter("commands.xml");
                XmlSerializer xml1 = new XmlSerializer(typeof(List<CustomCommand>));
                xml1.Serialize(stream, new List<CustomCommand>());
                stream.Close();
                return false;
            }
            return true;
        }
        [Serializable]
        public struct UserPermission
        {
            public ulong UserID;
            public int Level;
            public UserPermission(ulong UserID, int Level)
            {
                this.UserID = UserID;
                this.Level = Level;
            }

        }
        [Serializable]
        public struct CommandPermission
        {
            public string Command;
            public int Level;
            public CommandPermission(string Command, int Level)
            {
                this.Command = Command;
                this.Level = Level;
            }

        }
        [Serializable]
        public struct RolePermission
        {
            public string Role;
            public int Level;
            public RolePermission(string Role, int Level)
            {
                this.Role = Role;
                this.Level = Level;
            }

        }
    }
    [Serializable]
    public struct CustomCommand
    {
        public string Name;
        public string Contents;
        public CustomCommand(string name, string contents)
        {
            Name = name;
            Contents = contents;
        }
        public static CustomCommand GetCustomCommand(string Name)
        {
            XmlSerializer xml1 = new XmlSerializer(typeof(List<CustomCommand>));
            FileStream file1 = new FileStream("commands.xml", FileMode.OpenOrCreate);
            List<CustomCommand> list1 = (List<CustomCommand>)xml1.Deserialize(file1);

            file1.Close();
            return list1.Where(e => e.Name == Name).FirstOrDefault();
        }
        public static void WriteCustomCommand(CustomCommand command)
        {
            XmlSerializer xml1 = new XmlSerializer(typeof(List<CustomCommand>));
            FileStream file1 = new FileStream("commands.xml", FileMode.OpenOrCreate);
            List<CustomCommand> list1 = (List<CustomCommand>)xml1.Deserialize(file1);
            var list2 = list1.Where(e => command.Name == e.Name);
            if (list2.ToList().Count > 0)
            {
                list1 = list2.ToList().Except(list1).ToList();
            }
            list1.Add(command);
            StreamWriter stream = new StreamWriter("commands.xml");
            xml1.Serialize(stream, list1);
            stream.Close();
        }
    }
}
