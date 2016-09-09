using System;
using System.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.Commands.Permissions;
using Discord.Commands.Permissions.Levels;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args)
        {
            (new Program()).Start();
        }
        public Dictionary<string,CommandBuilder> commandBuilders = new Dictionary<string, CommandBuilder>();
        public string[] forbodens = new string[] { "help", "8ball", "addcustomcommand", "changename", "delcustomcommand","permissions" };
        private DiscordClient _client;
        public void Start()
        {

            _client = new DiscordClient(x=> {
                x.AppName = "CancerBot";
                x.MessageCacheSize = 10;
                x.EnablePreUpdateEvents = true;
            });
            _client.UsingPermissionLevels((u,c)=>  (int)CheckPermLevel(u,c));
            _client.UsingCommands(x => {
                x.PrefixChar = '>';
                x.HelpMode = HelpMode.Public;
                x.ErrorHandler += HandleError;
                x.AllowMentionPrefix = false;
            });

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
            }

            catch (XmlException e)
            {

                StreamWriter stream = new StreamWriter("commands.xml");
                XmlSerializer xml1 = new XmlSerializer(typeof(List<CustomCommand>));
                xml1.Serialize(stream, new List<CustomCommand>());
                stream.Close();
            }
            CommandService commandService = _client.GetService<CommandService>();
            List<CustomCommand> commands = new List<CustomCommand>();
            XmlSerializer xml = new XmlSerializer(typeof(List<CustomCommand>));

            FileStream file = new FileStream("commands.xml", FileMode.OpenOrCreate);

            List<CustomCommand> list = (List<CustomCommand>)xml.Deserialize(file);
            file.Close();
            foreach(CustomCommand x in list)
            {
                if (forbodens.Contains<string>(x.Name)) { continue; }
                CommandBuilder b = _client.GetService<CommandService>().CreateCommand(x.Name.ToLower()). Description(x.Contents).Parameter("Cuntents", ParameterType.Unparsed); b.Do(async e =>
                {
                    
                    double deltatime = 5;
                    if (LastCommandTime.ContainsKey(e.User.Id))
                    {
                        deltatime = (DateTime.Now.Second - LastCommandTime[e.User.Id]);
                    }
                    if (deltatime >= 5)
                    {
                        await e.Channel.SendMessage(x.Contents);
                       
                    }
                    if (LastCommandTime.ContainsKey(e.User.Id))
                    {
                        LastCommandTime[e.User.Id] = DateTime.Now.Second;
                    }
                    else
                    {
                        LastCommandTime[e.User.Id] = DateTime.Now.Second;
                    }
                });
                try
                {
                    b._command.IsCustom = true;
                    commandBuilders.Add(x.Name.ToLower(), b);

                }

                catch { Console.WriteLine($"Command with name {x.Name} already exists!"); };
            }
            _client.GetService<CommandService>().CreateCommand("test23").Description("Test Command").Do(async e =>
            {
                await e.Channel.SendMessage("test");
            });
            //_client.Ready += (s, e) =>
            //{
            //    _client.FindServers("Discord Bots").First().FindChannels("testing-aloha").First().SendMessage("I'm alive!");
            //};
            commandService.CreateCommand("changename").Description("Changes Name").Parameter("Name", ParameterType.Unparsed).MinPermissions((int)PermissionLevel.ServerOwner)
            .Do(async e =>
            {

                await _client.CurrentUser.Edit("", e.GetArg("Name"));
                
            });
            
           // _client.UserBanned += async (s, e) =>
           // {

                //await e.Server.DefaultChannel.SendMessage(e.User.Name + " was banned forever LUUL");
            //};
           // _client.UserLeft += async (s, e) =>
           // {

                //await e.Server.DefaultChannel.SendMessage(e.User.Name + " left FeelsBadMan");
          //  };
            _client.GetService<CommandService>().CreateCommand("8ball").Description("Simple 8ball command").Parameter("memes", ParameterType.Unparsed).Do(async e => {
                Random rand = new Random();
                string output = "";
                switch (rand.Next(6))
                {
                    case 0:
                        output = "I don't know faggot";
                        break;
                    case 1:
                        output = "Yes";
                        break;
                    case 2:
                        output = "No";
                        break;
                    case 3:
                        output = "Kys";
                        break;
                    case 4:
                        output = "Ask me later";
                        break;
                    case 5:
                        output = "Maybe";
                        break;
                    default: break;
                }
                await e.Channel.SendMessage(output);
            });
            Channel Log = _client.GetChannel(192699690473488384);
            _client.MessageReceived += async (s, e) => {
              //  if (!e.Message.IsAuthor)
              //  {
              //      if (e.Message.Attachments.Length == 0)
                 //   {
                      //  await e.Channel.SendMessage(DateTime.Now + " UTC - " + e.User.Name + " (" + e.User.Id + "): " + e.Message.Text);
                 //   }
                 //   else {
                      //  await e.Channel.SendMessage(DateTime.Now + " UTC - " + e.User.Name + " (" + e.User.Id + "): " + e.Message.Text+" | Message Contained Attachment: "+e.Message.Attachments[0].Filename+" "+e.Message.Attachments[0].Size+" "+e.Message.Attachments[0].ProxyUrl);
                  //  }
                //}
                
                if (e.Message.IsMentioningMe()&&!e.Message.IsAuthor)
                {
                    if(e.Message.RawText.ToLower().Contains("hi")|| e.Message.RawText.ToLower().Contains("hello"))
                    {
                        await e.Channel.SendMessage($"Hello, <@{e.User.Id}>!");
                    }
                    if (e.Message.RawText.ToLower().Contains("kys"))
                    {
                        if (e.User.Id == 142291646824841217)
                        {
                            await e.Channel.SendMessage("ok");
                            System.Threading.Thread.Sleep(1000);
                            _client.Disconnect();
                            System.Environment.Exit(0);
                            
                        }
                        else
                        {
                            await e.Channel.SendMessage("no u");
                        }
                    }
                }
            };
            _client.GetService<CommandService>().CreateCommand("addcustomcommand").Description("Adds Custom Command").Parameter("Name").Parameter("Contents",ParameterType.Unparsed).Do(async e =>
            {
                if (e.Message.RawText.Contains("@everyone")|| e.Message.RawText.Contains("@here"))
                {
                    await e.Channel.SendMessage("Don't ping everyone you faggot");
                    return;
                }
                if (e.Message.Attachments.Length > 0)
                {
                    await e.Channel.SendMessage("I don't support files you retard.");
                    return;
                }
                XmlSerializer xml1 = new XmlSerializer(typeof(List<CustomCommand>));
                FileStream file1 = new FileStream("commands.xml", FileMode.OpenOrCreate);
                List<CustomCommand> list1 = (List<CustomCommand>)xml1.Deserialize(file1);
                file1.Close();
                if (list1.Contains(new CustomCommand(e.GetArg("Name").ToLower(), e.GetArg("Contents"))) || forbodens.Contains<string>(e.GetArg("Name")))
                {
                    await e.Channel.SendMessage("That already exists.");
                    return;
                }
                if (commandBuilders.ContainsKey(e.GetArg("Name").ToLower()))
                {
                    List<CustomCommand> list2 = list1.Where<CustomCommand>(t => t.Name.ToLower() == e.GetArg("Name").ToLower()).ToList<CustomCommand>();
                    foreach (CustomCommand g in list2)
                    {
                        list1.Remove(g);
                        list1.Add(new CustomCommand(e.GetArg("Name").ToLower(), e.GetArg("Contents")));
                    }
                }
                else
                {
                    list1.Add(new CustomCommand(e.GetArg("Name").ToLower(), e.GetArg("Contents")));
                }
                StreamWriter stream = new StreamWriter("commands.xml");
                xml1.Serialize(stream, list1);
                stream.Close();

                if (commandBuilders.ContainsKey(e.GetArg("Name").ToLower())) {
                    commandBuilders[e.GetArg("Name").ToLower()].Hide();
                    Command test = commandBuilders[e.GetArg("Name").ToLower()]._command;//(Command)typeof(CommandBuilder).GetField("_command", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(commandBuilders[e.GetArg("Name").ToLower()]);
                    Action<CommandEventArgs> tes1 = (x =>
                    {
                    });
                    typeof(Command).GetField("_runFunc", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(test, TaskHelper.ToAsync(tes1));
                    test.IsHidden = true;
                    _client.GetService<CommandService>()._map._items.Remove(e.GetArg("Name").ToLower());
                    _client.GetService<CommandService>()._categories.FirstOrDefault().Value._items.ToList().ForEach(t => {
                        if (t.Value.Name == test.Text)
                        {
                            _client.GetService<CommandService>()._categories.FirstOrDefault().Value._items.Remove(t.Value.Name);
                        }
                    });
                    commandBuilders[e.GetArg("Name").ToLower()].Description(e.GetArg("Contents"));
                    Command test1 = commandBuilders[e.GetArg("Name").ToLower()]._command;//(Command)typeof(CommandBuilder).GetField("_command", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(commandBuilders[e.GetArg("Name").ToLower()]);
                    Func<CommandEventArgs,Task> tes11 = (async x =>
                    {
                        double deltatime = 5;
                        if (LastCommandTime.ContainsKey(e.User.Id))
                        {
                            deltatime = (DateTime.Now.Second - LastCommandTime[e.User.Id]);
                        }
                        if (deltatime >= 5) { 
                        await x.Channel.SendMessage(e.GetArg("Contents"));
                       
                    }
                        if (LastCommandTime.ContainsKey(e.User.Id))
                        {
                            LastCommandTime[e.User.Id] = DateTime.Now.Second;
                        }
                        else
                        {
                            LastCommandTime[e.User.Id] = DateTime.Now.Second;
                        }
                    });
                    commandBuilders[e.GetArg("Name").ToLower()].Do(tes11);
                }
                else
                {
                    CommandBuilder b = _client.GetService<CommandService>().CreateCommand(e.GetArg("Name")).Description(e.GetArg("Contents")).Parameter("Cuntents", ParameterType.Unparsed); b.Do(async x =>
                     {
                         await x.Channel.SendMessage(e.GetArg("Contents"));
                     });
                    b._command.IsCustom = true;
                    commandBuilders.Add(e.GetArg("Name").ToLower(), b);

                }
                await e.Channel.SendMessage("Command Made Successfully!");
            });
            _client.GetService<CommandService>().CreateCommand("delcustomcommand").Description("Deletes a Custom Command").Parameter("Name").Do(async e =>
            {

                XmlSerializer xml1 = new XmlSerializer(typeof(List<CustomCommand>));
                FileStream file1 = new FileStream("commands.xml", FileMode.OpenOrCreate);
                List<CustomCommand> list1 = (List<CustomCommand>)xml1.Deserialize(file1);
                file1.Close();

                List<CustomCommand> list2 = list1.Where<CustomCommand>(t => t.Name.ToLower() == e.GetArg("Name").ToLower()).ToList<CustomCommand>();
                if (list2.Count == 0)
                {
                    await e.Channel.SendMessage("Command does not exist!");
                    return;
                }
                foreach(CustomCommand x in list2)
                {
                    list1.Remove(x);
                }
                StreamWriter stream = new StreamWriter("commands.xml");
                xml1.Serialize(stream, list1);
                stream.Close();
                foreach (Server s in _client.Servers)
                {
                    ServerConfig config = ServerConfig.GetServerConfig(s.Id);
                    if (config.commandPerms.Where(i => i.Command == e.GetArg("Name")).ToList().Count > 0)
                    {
                        config.commandPerms.Remove(config.commandPerms.Where(i => i.Command == e.GetArg("Name")).FirstOrDefault());
                        ServerConfig.WriteServerConfig(config);
                    }
                }
                commandBuilders[e.GetArg("Name").ToLower()].Hide();
                Command test = commandBuilders[e.GetArg("Name").ToLower()]._command;//(Command)typeof(CommandBuilder).GetField("_command", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(commandBuilders[e.GetArg("Name").ToLower()]);
                Action<CommandEventArgs> tes1 = (x =>
                {
                });
                typeof(Command).GetField("_runFunc", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(test, TaskHelper.ToAsync(tes1));
                test.IsHidden = true;
                _client.GetService<CommandService>()._map._items.Remove(e.GetArg("Name").ToLower());
                _client.GetService<CommandService>()._categories.FirstOrDefault().Value._items.ToList().ForEach(t=> {
                    if (t.Value.Name == test.Text) { _client.GetService<CommandService>()._categories.FirstOrDefault().Value._items.Remove(t.Value.Name); 
                }
                });
                commandBuilders.Remove(e.GetArg("Name"));
                //Console.WriteLine(_client.GetService<CommandService>()._categories.FirstOrDefault().Value._items.FirstOrDefault().);
                await e.Channel.SendMessage("Command removed Successfully!");
            });
            _client.GetService<CommandService>().CreateGroup("permissions",e=> {
                e.CreateGroup("editserver", t => {
                    t.CreateCommand("command").Description("Sets Permission Level for Command").Parameter("Command").Parameter("Level").Do(async u => {

                        ServerConfig config = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.CommandPermission commandperm;
                        if (config.commandPerms.Where(i => i.Command == u.GetArg("Command")).ToList().Count > 0)
                        {
                            commandperm = config.commandPerms.Where(i => i.Command == u.GetArg("Command")).FirstOrDefault();
                            commandperm.Level = Int32.Parse(u.GetArg("Level"));
                            config.commandPerms[config.commandPerms.IndexOf(config.commandPerms.Where(i => i.Command == u.GetArg("Command")).FirstOrDefault())] = commandperm;
                        }
                        else
                        {
                            config.commandPerms.Add(new ServerConfig.CommandPermission(u.GetArg("Command"), Int32.Parse(u.GetArg("Level"))));

                        }
                        ServerConfig.WriteServerConfig(config);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("removecommand").Description("Removes Permission Level for Command").Parameter("Command").Do(async u => {

                        ServerConfig config = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.CommandPermission commandperm;
                        if (config.commandPerms.Where(i => i.Command == u.GetArg("Command")).ToList().Count > 0)
                        {
                            //commandperm = config.commandPerms.Where(i => i.Command == u.GetArg("Command")).FirstOrDefault();
                            //commandperm.Level = Int32.Parse(u.GetArg("Level"));
                            config.commandPerms.Remove(config.commandPerms.Where(i => i.Command == u.GetArg("Command")).FirstOrDefault());
                        }
                        else
                        {
                            //config.commandPerms.Add(new ServerConfig.CommandPermission(u.GetArg("Command"), Int32.Parse(u.GetArg("Level"))));
                            await u.Channel.SendMessage("Permissions UnSucksecksful");
                        }
                        ServerConfig.WriteServerConfig(config);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("role").Description("Sets Permission Level for Role (has to be EXACT name of role)").Parameter("Command").Parameter("Level").Do(async u => {

                        ServerConfig config = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.RolePermission commandperm;
                        if (config.rolePerms.Where(i => i.Role == u.GetArg("Command")).ToList().Count > 0)
                        {
                            commandperm = config.rolePerms.Where(i => i.Role == u.GetArg("Command")).FirstOrDefault();
                            commandperm.Level = Int32.Parse(u.GetArg("Level"));
                            config.rolePerms[config.rolePerms.IndexOf(config.rolePerms.Where(i => i.Role == u.GetArg("Command")).FirstOrDefault())] = commandperm;

                        }
                        else
                        {
                            config.rolePerms.Add(new ServerConfig.RolePermission(u.GetArg("Command"), Int32.Parse(u.GetArg("Level"))));

                        }
                        ServerConfig.WriteServerConfig(config);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("removerole").Description("Removes Permission Level for Role (has to be EXACT name of role)").Parameter("Command").Do(async u => {

                        ServerConfig config = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.RolePermission commandperm;
                        if (config.rolePerms.Where(i => i.Role == u.GetArg("Command")).ToList().Count > 0)
                        {

                            config.rolePerms.Remove(config.rolePerms.Where(i => i.Role == u.GetArg("Command")).FirstOrDefault());

                        }
                        else
                        {
                            //config.rolePerms.Add(new ServerConfig.RolePermission(u.GetArg("Command"), Int32.Parse(u.GetArg("Level"))));
                            await u.Channel.SendMessage("Permissions UnSucksecksful");
                        }
                        ServerConfig.WriteServerConfig(config);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("user").Description("Sets Permission Level for User (has to be mention)").Parameter("Command").Parameter("Level").Do(async u => {
                        List<char> input = u.GetArg("Command").ToCharArray().ToList();
                        input.RemoveAt(0);
                        input.RemoveAt(0);

                        input.RemoveAt(input.Count - 1);
                        ulong id = ulong.Parse(String.Concat(input));
                        ServerConfig config = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.UserPermission commandperm;
                        if (config.userPerms.Where(i => i.UserID == id).ToList().Count > 0)
                        {
                            commandperm = config.userPerms.Where(i => i.UserID == id).FirstOrDefault();
                            commandperm.Level = Int32.Parse(u.GetArg("Level"));
                            config.userPerms[config.userPerms.IndexOf(config.userPerms.Where(i => i.UserID == id).FirstOrDefault())] = commandperm;

                        }
                        else
                        {
                            config.userPerms.Add(new ServerConfig.UserPermission(id, Int32.Parse(u.GetArg("Level"))));

                        }
                        ServerConfig.WriteServerConfig(config);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("removeuser").Description("Remove Permission Level for User (has to be mention)").Parameter("Command").Do(async u => {
                        List<char> input = u.GetArg("Command").ToCharArray().ToList();
                        input.RemoveAt(0);
                        input.RemoveAt(0);

                        input.RemoveAt(input.Count - 1);
                        ulong id = ulong.Parse(String.Concat(input));
                        ServerConfig config = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.UserPermission commandperm;
                        if (config.userPerms.Where(i => i.UserID == id).ToList().Count > 0)
                        {
                            commandperm = config.userPerms.Where(i => i.UserID == id).FirstOrDefault();

                            config.userPerms.Remove(commandperm);

                        }
                        else
                        {
                            //config.userPerms.Add(new ServerConfig.UserPermission(id, Int32.Parse(u.GetArg("Level"))));
                            await u.Channel.SendMessage("Permissions UnSucksecksful");
                        }
                        ServerConfig.WriteServerConfig(config);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("commanduser").Description("Blacklists/Whitelists commands for certain user").Parameter("User").Parameter("Command").Parameter("Enabled").Do(async u => {
                        List<char> input = u.GetArg("User").ToCharArray().ToList();
                        input.RemoveAt(0);
                        input.RemoveAt(0);

                        input.RemoveAt(input.Count - 1);
                        ulong id = ulong.Parse(String.Concat(input));
                        ServerConfig config = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.UserCommandPermission commandperm;
                        if (config.userCommandPerms.Where(i => (i.Command == u.GetArg("Command") && i.UserID == id)).ToList().Count > 0)
                        {
                            commandperm = config.userCommandPerms.Where(i => i.Command == u.GetArg("Command") && i.UserID == id).FirstOrDefault();
                            commandperm.UserID = id;
                            commandperm.enabled = bool.Parse(u.GetArg("Enabled"));
                            config.userCommandPerms[config.userCommandPerms.IndexOf(config.userCommandPerms.Where(i => i.Command == u.GetArg("Command") && i.UserID == id).FirstOrDefault())] = commandperm;
                        }
                        else
                        {
                            config.userCommandPerms.Add(new ServerConfig.UserCommandPermission(id, u.GetArg("Command"), bool.Parse(u.GetArg("Enabled"))));

                        }
                        ServerConfig.WriteServerConfig(config);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("removecommanduser").Description("Removes a Blacklist/Whitelist command for certain user").Parameter("User").Parameter("Command").Do(async u => {
                        List<char> input = u.GetArg("User").ToCharArray().ToList();
                        input.RemoveAt(0);
                        input.RemoveAt(0);

                        input.RemoveAt(input.Count - 1);
                        ulong id = ulong.Parse(String.Concat(input));
                        ServerConfig config = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.UserCommandPermission commandperm;
                        if (config.userCommandPerms.Where(i => (i.Command == u.GetArg("Command") && i.UserID == id)).ToList().Count > 0)
                        {
                            commandperm = config.userCommandPerms.Where(i => i.Command == u.GetArg("Command") && i.UserID == id).FirstOrDefault();

                            config.userCommandPerms.Remove(commandperm);
                        }
                        else
                        {

                            await u.Channel.SendMessage("Permissions UnSucksecksful");
                        }
                        ServerConfig.WriteServerConfig(config);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("commandrole").Description("Blacklists/Whitelists commands for certain role").Parameter("User").Parameter("Command").Parameter("Enabled").Do(async u => {
                    //    List<char> input = u.GetArg("User").ToCharArray().ToList();
                     //   input.RemoveAt(0);
                     //   input.RemoveAt(0);

                     //   input.RemoveAt(input.Count - 1);
                     //   ulong id = ulong.Parse(String.Concat(input));
                        ServerConfig config = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.RoleCommandPermission commandperm;
                        if (config.roleCommandPerms.Where(i => (i.Command == u.GetArg("Command")&&i.Role==u.GetArg("User"))).ToList().Count > 0)
                        {
                            commandperm = config.roleCommandPerms.Where(i => i.Command == u.GetArg("Command") && i.Role == u.GetArg("User")).FirstOrDefault();
                            commandperm.Role = u.GetArg("User");
                            commandperm.enabled = bool.Parse(u.GetArg("Enabled"));
                            config.roleCommandPerms[config.roleCommandPerms.IndexOf(config.roleCommandPerms.Where(i => i.Command == u.GetArg("Command") && i.Role == u.GetArg("User")).FirstOrDefault())] = commandperm;
                        }
                        else
                        {
                            config.roleCommandPerms.Add(new ServerConfig.RoleCommandPermission(u.GetArg("User"), u.GetArg("Command"), bool.Parse(u.GetArg("Enabled"))));

                        }
                        ServerConfig.WriteServerConfig(config);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("removecommandrole").Description("Remove a Blacklist/Whitelist command for certain role").Parameter("User").Parameter("Command").Do(async u => {
                        //    List<char> input = u.GetArg("User").ToCharArray().ToList();
                        //   input.RemoveAt(0);
                        //   input.RemoveAt(0);

                        //   input.RemoveAt(input.Count - 1);
                        //   ulong id = ulong.Parse(String.Concat(input));
                        ServerConfig config = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.RoleCommandPermission commandperm;
                        if (config.roleCommandPerms.Where(i => (i.Command == u.GetArg("Command") && i.Role == u.GetArg("User"))).ToList().Count > 0)
                        {
                            commandperm = config.roleCommandPerms.Where(i => i.Command == u.GetArg("Command") && i.Role == u.GetArg("User")).FirstOrDefault();
                            config.roleCommandPerms.Remove(commandperm);
                        }
                        else
                        {
                            await u.Channel.SendMessage("Permissions UnSucksecksful");

                        }
                        ServerConfig.WriteServerConfig(config);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                });
                
                
                e.CreateGroup("editchannel", t => {
                    t.CreateCommand("command").Description("Sets Permission Level for Command").Parameter("Command").Parameter("Level").Do(async u => {

                        ServerConfig config1 = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.ChannelPermission config = config1.channelPerms.Where(i => i.ChannelID==u.Channel.Id).FirstOrDefault();
                        if (config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).ToList().Count == 0)
                        {
                            config = new ServerConfig.ChannelPermission(u.Channel.Id);
                            config1.channelPerms.Add(config);
                        }
                        ServerConfig.CommandPermission commandperm;
                        if (config.commandPerms.Where(i => i.Command == u.GetArg("Command")).ToList().Count > 0)
                        {
                            commandperm = config.commandPerms.Where(i => i.Command == u.GetArg("Command")).FirstOrDefault();
                            commandperm.Level = Int32.Parse(u.GetArg("Level"));
                            config.commandPerms[config.commandPerms.IndexOf(config.commandPerms.Where(i => i.Command == u.GetArg("Command")).FirstOrDefault())] = commandperm;
                        }
                        else
                        {
                            config.commandPerms.Add(new ServerConfig.CommandPermission(u.GetArg("Command"), Int32.Parse(u.GetArg("Level"))));

                        }
                        config1.channelPerms[config1.channelPerms.FindIndex(y => y.ChannelID == u.Channel.Id)] = config;
                        ServerConfig.WriteServerConfig(config1);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("removecommand").Description("Removes Permission Level for Command").Parameter("Command").Do(async u => {

                        ServerConfig config1 = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.ChannelPermission config = config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).FirstOrDefault();
                        if (config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).ToList().Count == 0)
                        {
                            config = new ServerConfig.ChannelPermission(u.Channel.Id);
                            config1.channelPerms.Add(config);
                        }
                        ServerConfig.CommandPermission commandperm;
                        if (config.commandPerms.Where(i => i.Command == u.GetArg("Command")).ToList().Count > 0)
                        {
                            commandperm = config.commandPerms.Where(i => i.Command == u.GetArg("Command")).FirstOrDefault();

                            config.commandPerms.Remove(commandperm);
                        }
                        else
                        {
                            await u.Channel.SendMessage("Permissions UnSucksecksful");

                        }
                        config1.channelPerms[config1.channelPerms.FindIndex(y => y.ChannelID == u.Channel.Id)] = config;
                        ServerConfig.WriteServerConfig(config1);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("role").Description("Sets Permission Level for Role (has to be EXACT name of role)").Parameter("Command").Parameter("Level").Do(async u => {

                        ServerConfig config1 = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.ChannelPermission config = config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).FirstOrDefault();
                        if (config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).ToList().Count == 0)
                        {
                            config = new ServerConfig.ChannelPermission(u.Channel.Id);
                            config1.channelPerms.Add(config);
                        }
                        ServerConfig.RolePermission commandperm;
                        if (config.rolePerms.Where(i => i.Role == u.GetArg("Command")).ToList().Count > 0)
                        {
                            commandperm = config.rolePerms.Where(i => i.Role == u.GetArg("Command")).FirstOrDefault();
                            commandperm.Level = Int32.Parse(u.GetArg("Level"));
                            config.rolePerms[config.rolePerms.IndexOf(config.rolePerms.Where(i => i.Role == u.GetArg("Command")).FirstOrDefault())] = commandperm;

                        }
                        else
                        {
                            config.rolePerms.Add(new ServerConfig.RolePermission(u.GetArg("Command"), Int32.Parse(u.GetArg("Level"))));

                        }
                        config1.channelPerms[config1.channelPerms.FindIndex(y => y.ChannelID == u.Channel.Id)] = config;
                        ServerConfig.WriteServerConfig(config1);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("removerole").Description("Removes Permission Level for Role (has to be EXACT name of role)").Parameter("Command").Do(async u => {

                        ServerConfig config1 = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.ChannelPermission config = config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).FirstOrDefault();
                        if (config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).ToList().Count == 0)
                        {
                            config = new ServerConfig.ChannelPermission(u.Channel.Id);
                            config1.channelPerms.Add(config);
                        }
                        ServerConfig.RolePermission commandperm;
                        if (config.rolePerms.Where(i => i.Role == u.GetArg("Command")).ToList().Count > 0)
                        {
                            commandperm = config.rolePerms.Where(i => i.Role == u.GetArg("Command")).FirstOrDefault();

                            config.rolePerms.Remove(commandperm);

                        }
                        else
                        {
                            await u.Channel.SendMessage("Permissions UnSucksecksful");

                        }
                        config1.channelPerms[config1.channelPerms.FindIndex(y => y.ChannelID == u.Channel.Id)] = config;
                        ServerConfig.WriteServerConfig(config1);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("user").Description("Sets Permission Level for User (has to be mention)").Parameter("Command").Parameter("Level").Do(async u => {
                        List<char> input = u.GetArg("Command").ToCharArray().ToList();
                        input.RemoveAt(0);
                        input.RemoveAt(0);

                        input.RemoveAt(input.Count - 1);
                        ulong id = ulong.Parse(String.Concat(input));
                        ServerConfig config1 = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.ChannelPermission config = config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).FirstOrDefault();
                        if (config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).ToList().Count == 0)
                        {
                            config = new ServerConfig.ChannelPermission(u.Channel.Id);
                            config1.channelPerms.Add(config);
                        }
                        ServerConfig.UserPermission commandperm;
                        if (config.userPerms.Where(i => i.UserID == id).ToList().Count > 0)
                        {
                            commandperm = config.userPerms.Where(i => i.UserID == id).FirstOrDefault();
                            commandperm.Level = Int32.Parse(u.GetArg("Level"));
                            config.userPerms[config.userPerms.IndexOf(config.userPerms.Where(i => i.UserID == id).FirstOrDefault())] = commandperm;

                        }
                        else
                        {
                            config.userPerms.Add(new ServerConfig.UserPermission(id, Int32.Parse(u.GetArg("Level"))));

                        }
                        config1.channelPerms[config1.channelPerms.FindIndex(y => y.ChannelID == u.Channel.Id)] = config;
                        ServerConfig.WriteServerConfig(config1);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });

                    t.CreateCommand("removeuser").Description("Removes Permission Level for User (has to be mention)").Parameter("Command").Do(async u => {
                        List<char> input = u.GetArg("Command").ToCharArray().ToList();
                        input.RemoveAt(0);
                        input.RemoveAt(0);

                        input.RemoveAt(input.Count - 1);
                        ulong id = ulong.Parse(String.Concat(input));
                        ServerConfig config1 = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.ChannelPermission config = config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).FirstOrDefault();
                        if (config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).ToList().Count == 0)
                        {
                            config = new ServerConfig.ChannelPermission(u.Channel.Id);
                            config1.channelPerms.Add(config);
                        }
                        ServerConfig.UserPermission commandperm;
                        if (config.userPerms.Where(i => i.UserID == id).ToList().Count > 0)
                        {
                            commandperm = config.userPerms.Where(i => i.UserID == id).FirstOrDefault();
                            config.userPerms.Remove(commandperm);

                        }
                        else
                        {
                            await u.Channel.SendMessage("Permissions UnSucksecksful");

                        }
                        config1.channelPerms[config1.channelPerms.FindIndex(y => y.ChannelID == u.Channel.Id)] = config;
                        ServerConfig.WriteServerConfig(config1);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("commanduser").Description("Blacklists/Whitelists commands for certain user").Parameter("User").Parameter("Command").Parameter("Enabled").Do(async u => {
                        List<char> input = u.GetArg("User").ToCharArray().ToList();
                        input.RemoveAt(0);
                        input.RemoveAt(0);

                        input.RemoveAt(input.Count - 1);
                        ulong id = ulong.Parse(String.Concat(input));
                        ServerConfig config1 = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.ChannelPermission config = config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).FirstOrDefault();
                        if (config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).ToList().Count == 0)
                        {
                            config = new ServerConfig.ChannelPermission(u.Channel.Id);
                            config1.channelPerms.Add(config);
                        }
                        ServerConfig.UserCommandPermission commandperm;
                        if (config.userCommandPerms.Where(i => (i.Command == u.GetArg("Command") && i.UserID == id)).ToList().Count > 0)
                        {
                            commandperm = config.userCommandPerms.Where(i => i.Command == u.GetArg("Command") && i.UserID == id).FirstOrDefault();
                            commandperm.UserID = id;
                            commandperm.enabled = bool.Parse(u.GetArg("Enabled"));
                            config.userCommandPerms[config.userCommandPerms.IndexOf(config.userCommandPerms.Where(i => i.Command == u.GetArg("Command") && i.UserID == id).FirstOrDefault())] = commandperm;
                        }
                        else
                        {
                            config.userCommandPerms.Add(new ServerConfig.UserCommandPermission(id, u.GetArg("Command"), bool.Parse(u.GetArg("Enabled"))));

                        }
                        config1.channelPerms[config1.channelPerms.FindIndex(y => y.ChannelID == u.Channel.Id)] = config;
                        ServerConfig.WriteServerConfig(config1);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("removecommanduser").Description("Removes a Blacklist/Whitelist command for certain user").Parameter("User").Parameter("Command").Do(async u => {
                        List<char> input = u.GetArg("User").ToCharArray().ToList();
                        input.RemoveAt(0);
                        input.RemoveAt(0);

                        input.RemoveAt(input.Count - 1);
                        ulong id = ulong.Parse(String.Concat(input));
                        ServerConfig config1 = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.ChannelPermission config = config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).FirstOrDefault();
                        if (config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).ToList().Count == 0)
                        {
                            config = new ServerConfig.ChannelPermission(u.Channel.Id);
                            config1.channelPerms.Add(config);
                        }
                        ServerConfig.UserCommandPermission commandperm;
                        if (config.userCommandPerms.Where(i => (i.Command == u.GetArg("Command") && i.UserID == id)).ToList().Count > 0)
                        {
                            commandperm = config.userCommandPerms.Where(i => i.Command == u.GetArg("Command") && i.UserID == id).FirstOrDefault();

                            config.userCommandPerms.Remove(commandperm);
                        }
                        else
                        {
                            await u.Channel.SendMessage("Permissions UnSucksecksful");

                        }
                        config1.channelPerms[config1.channelPerms.FindIndex(y => y.ChannelID == u.Channel.Id)] = config;
                        ServerConfig.WriteServerConfig(config1);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("commandrole").Description("Blacklists/Whitelists commands for certain role").Parameter("User").Parameter("Command").Parameter("Enabled").Do(async u => {
                        //    List<char> input = u.GetArg("User").ToCharArray().ToList();
                        //   input.RemoveAt(0);
                        //   input.RemoveAt(0);

                        //   input.RemoveAt(input.Count - 1);
                        //   ulong id = ulong.Parse(String.Concat(input));
                        ServerConfig config1 = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.ChannelPermission config = config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).FirstOrDefault();
                        if(config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).ToList().Count == 0)
                        {
                            config = new ServerConfig.ChannelPermission(u.Channel.Id);
                            config1.channelPerms.Add(config);
                        }
                        ServerConfig.RoleCommandPermission commandperm;

                            if (config.roleCommandPerms.Where(i => (i.Command == u.GetArg("Command") && i.Role == u.GetArg("User"))).ToList().Count > 0)
                            {
                                commandperm = config.roleCommandPerms.Where(i => i.Command == u.GetArg("Command") && i.Role == u.GetArg("User")).FirstOrDefault();
                                commandperm.Role = u.GetArg("User");
                                commandperm.enabled = bool.Parse(u.GetArg("Enabled"));
                                config.roleCommandPerms[config.roleCommandPerms.IndexOf(config.roleCommandPerms.Where(i => i.Command == u.GetArg("Command") && i.Role == u.GetArg("User")).FirstOrDefault())] = commandperm;
                            }
                        
                        else
                        {
                            config.roleCommandPerms.Add(new ServerConfig.RoleCommandPermission(u.GetArg("User"), u.GetArg("Command"), bool.Parse(u.GetArg("Enabled"))));

                        }
                        config1.channelPerms[config1.channelPerms.FindIndex(y => y.ChannelID == u.Channel.Id)] = config;
                        ServerConfig.WriteServerConfig(config1);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                    t.CreateCommand("removecommandrole").Description("Removes a Blacklist/Whitelist command for certain role").Parameter("User").Parameter("Command").Do(async u => {
                        //    List<char> input = u.GetArg("User").ToCharArray().ToList();
                        //   input.RemoveAt(0);
                        //   input.RemoveAt(0);

                        //   input.RemoveAt(input.Count - 1);
                        //   ulong id = ulong.Parse(String.Concat(input));
                        ServerConfig config1 = ServerConfig.GetServerConfig(u.Channel.Server.Id);
                        ServerConfig.ChannelPermission config = config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).FirstOrDefault();
                        if (config1.channelPerms.Where(i => i.ChannelID == u.Channel.Id).ToList().Count == 0)
                        {
                            config = new ServerConfig.ChannelPermission(u.Channel.Id);
                            config1.channelPerms.Add(config);
                        }
                        ServerConfig.RoleCommandPermission commandperm;

                        if (config.roleCommandPerms.Where(i => (i.Command == u.GetArg("Command") && i.Role == u.GetArg("User"))).ToList().Count > 0)
                        {
                            commandperm = config.roleCommandPerms.Where(i => i.Command == u.GetArg("Command") && i.Role == u.GetArg("User")).FirstOrDefault();
                            config.roleCommandPerms.Remove(commandperm);
                        }

                        else
                        {
                            await u.Channel.SendMessage("Permissions UnSucksecksful");

                        }
                        config1.channelPerms[config1.channelPerms.FindIndex(y => y.ChannelID == u.Channel.Id)] = config;
                        ServerConfig.WriteServerConfig(config1);
                        await u.Channel.SendMessage("Permissions Sucksecksful");
                    });
                });
            });
            _client.ExecuteAndWait(async ()=>{
                await _client.Connect("MTcxNzEyMTAwMzE2NjEwNTYx.CfbSCw.saNJTaBcw4EN8nZjjlzhzuHzJFI");
                //"mcdanny720@hotmail.com", "mushroom12345"
            });


        }
        public Dictionary<double, double> LastCommandTime = new Dictionary<double, double>();
        public async void HandleError(object sender, CommandErrorEventArgs e) {
            if (e.ErrorType == CommandErrorType.Exception)
            {
                await e.Channel.SendMessage("I don't know what you just did but something just went wrong. ");
                Console.WriteLine(e.Exception.ToString());
            }
            if (e.ErrorType == CommandErrorType.BadPermissions)
            {
                await e.Channel.SendMessage("You don't have permission to do that loser.");
            }
            if (e.ErrorType == CommandErrorType.UnknownCommand)
            {
                if (e.Message.IsMentioningMe()) { return; }
                //await e.Channel.SendMessage("I don't know what that is, It probably doesn't exist. Wow you really messed up.");
            }
        }
        public PermissionLevel CheckPermLevel(User user,Channel channel)
        {
            if (user.Id == 142291646824841217)
            {
                return PermissionLevel.BotOwner;
            }
            if (user == channel.Server.Owner)
            {
                return PermissionLevel.ServerOwner;
            }
            else
            {
                return PermissionLevel.Peasant;
            }
       }
    }
    public enum PermissionLevel
    {
        BotOwner = 69,
        ServerOwner=68,
        Peasant=0,
    }
    
}
