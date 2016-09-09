using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
namespace Discord.Commands
{
    [Serializable]
    public class ServerConfig
    {
        public List<ServerConfig.RolePermission> rolePerms = new List<ServerConfig.RolePermission>();
        public List<ServerConfig.CommandPermission> commandPerms = new List<ServerConfig.CommandPermission>();
        public List<ServerConfig.UserPermission> userPerms = new List<ServerConfig.UserPermission>();
        public List<ServerConfig.UserCommandPermission> userCommandPerms = new List<ServerConfig.UserCommandPermission>();
        public List<ServerConfig.ChannelPermission> channelPerms = new List<ServerConfig.ChannelPermission>();
        public List<ServerConfig.RoleCommandPermission> roleCommandPerms = new List<ServerConfig.RoleCommandPermission>();
        public ulong ID;

        public ServerConfig(ulong id)
        {
            this.ID = id;
            this.commandPerms.Add(new ServerConfig.CommandPermission("changename", 69));
            this.commandPerms.Add(new ServerConfig.CommandPermission("permissions", 68));
        }

        public ServerConfig()
        {
        }

        public static bool CheckIfExists(ulong ID)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(ID.ToString() + ".xml");
            }
            catch (FileNotFoundException ex)
            {
                StreamWriter streamWriter = new StreamWriter(ID.ToString() + ".xml");
                new XmlSerializer(typeof(ServerConfig)).Serialize((TextWriter)streamWriter, (object)new ServerConfig(ID));
                streamWriter.Close();
                ex.ToString();
                return false;
            }
            catch (XmlException ex)
            {
                StreamWriter streamWriter = new StreamWriter(ID.ToString() + ".xml");
                new XmlSerializer(typeof(ServerConfig)).Serialize((TextWriter)streamWriter, (object)new ServerConfig(ID));
                streamWriter.Close();
                ex.ToString();
                return false;
            }
            return true;
        }

        public static ServerConfig GetServerConfig(ulong id)
        {
            ServerConfig.CheckIfExists(id);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ServerConfig));
            FileStream fileStream = new FileStream(id.ToString() + ".xml", FileMode.OpenOrCreate);
            ServerConfig serverConfig = (ServerConfig)xmlSerializer.Deserialize((Stream)fileStream);
            fileStream.Close();
            return serverConfig;
        }

        public static void WriteServerConfig(ServerConfig config)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ServerConfig));
            StreamWriter streamWriter = new StreamWriter(config.ID.ToString() + ".xml");
            xmlSerializer.Serialize((TextWriter)streamWriter, (object)config);
            streamWriter.Close();
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
        public struct UserCommandPermission
        {
            public ulong UserID;
            public string Command;
            public bool enabled;

            public UserCommandPermission(ulong UserID, string Level, bool enabled)
            {
                this.UserID = UserID;
                this.Command = Level;
                this.enabled = enabled;
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
        public class ChannelPermission
        {
            public List<ServerConfig.RolePermission> rolePerms = new List<ServerConfig.RolePermission>();
            public List<ServerConfig.CommandPermission> commandPerms = new List<ServerConfig.CommandPermission>();
            public List<ServerConfig.UserPermission> userPerms = new List<ServerConfig.UserPermission>();
            public List<ServerConfig.UserCommandPermission> userCommandPerms = new List<ServerConfig.UserCommandPermission>();
            public List<ServerConfig.RoleCommandPermission> roleCommandPerms = new List<ServerConfig.RoleCommandPermission>();
            public ulong ChannelID;

            public ChannelPermission(ulong channelID)
            {
                this.ChannelID = channelID;
            }

            public ChannelPermission()
            {
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

        [Serializable]
        public struct RoleCommandPermission
        {
            public string Role;
            public string Command;
            public bool enabled;

            public RoleCommandPermission(string Role, string Command, bool enabled)
            {
                this.Role = Role;
                this.Command = Command;
                this.enabled = enabled;
            }
        }
    }
}
