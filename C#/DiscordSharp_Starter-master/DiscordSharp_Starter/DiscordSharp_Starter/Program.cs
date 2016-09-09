//  If you have any questions or just want to talk, join my server!
//  https://discord.gg/0oZpaYcAjfvkDuE4
using DiscordSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordSharp_Starter
{
    class Program
    {
        static void Main(string[] args)
        {
            // First of all, a DiscordClient will be created, and the email and password will be defined.
            Console.WriteLine("Defining variables");
            DiscordClient client = new DiscordClient();
            client.ClientPrivateInformation.Email = "";
            client.ClientPrivateInformation.Password = "";

            // Then, we are going to set up our events before connecting to discord, to make sure nothing goes wrong.

            Console.WriteLine("Defining Events");
            client.Connected += (sender, e) => // Client is connected to Discord
            {
                Console.WriteLine("Connected! User: " + e.User.Username);
                // If the bot is connected, this message will show.
                // Changes to client, like playing game should be called when the client is connected,
                // just to make sure nothing goes wrong.
                client.UpdateCurrentGame("Bot online!"); // This will display at "Playing: "
                //Whoops! i messed up here. (original: Bot online!\nPress any key to close this window.)
            };


            client.PrivateMessageReceived += (sender, e) => // Private message has been received
            {
                if (e.Message == "help")
                {
                    e.Author.SendMessage("This is a private message!");
                    // Because this is a private message, the bot should send a private message back
                    // A private message does NOT have a channel
                }
                if (e.Message.StartsWith("join "))
                {
                    string inviteID = e.Message.Substring(e.Message.LastIndexOf('/') + 1);
                    // Thanks to LuigiFan (Developer of DiscordSharp) for this line of code!
                    client.AcceptInvite(inviteID);
                    e.Author.SendMessage("Joined your discord server!");
                    Console.WriteLine("Got join request from " + inviteID);
                }
            };


            client.MessageReceived += (sender, e) => // Channel message has been received
            {
                if (e.MessageText == "help")
                {
                    e.Channel.SendMessage("This is a public message!");
                    // Because this is a public message, 
                    // the bot should send a message to the channel the message was received.
                }
            };

            //  Below: some things that might be nice?

            //  This sends a message to every new channel on the server
            client.ChannelCreated += (sender, e) =>
                {
                    e.ChannelCreated.SendMessage("Nice! a new channel has been created!");
                };

            //  When a user joins the server, send a message to them.
            client.UserAddedToServer += (sender, e) =>
                {
                    e.AddedMember.SendMessage("Welcome to my server! rules:");
                    e.AddedMember.SendMessage("1. be nice!");
                    e.AddedMember.SendMessage("- Your name!");
                };

            //  Don't want messages to be removed? this piece of code will
            //  Keep messages for you. Remove if unused :)
            client.MessageDeleted += (sender, e) =>
                {
                    e.Channel.SendMessage("Removing messages has been disabled on this server!");
                    e.Channel.SendMessage("<@" + e.DeletedMessage.Author.ID + "> sent: " +e.DeletedMessage.Content.ToString());
                };

            // Now, try to connect to Discord.
            try{ 
                // Make sure that IF something goes wrong, the user will be notified.
                // The SendLoginRequest should be called after the events are defined, to prevent issues.
                Console.WriteLine("Sending login request");
                client.SendLoginRequest();
                Console.WriteLine("Connecting client in separate thread");
                Thread connect = new Thread(client.Connect);
                connect.Start();
                 // Login request, and then connect using the discordclient i just made.
                Console.WriteLine("Client connected!");
            }catch(Exception e){
                Console.WriteLine("Something went wrong!\n" + e.Message + "\nPress any key to close this window.");
            }

            // Done! your very own Discord bot is online!


            // Now to make sure the console doesnt close:
            Console.ReadKey(); // If the user presses a key, the bot will shut down.
            Environment.Exit(0); // Make sure all threads are closed.
        }
    }
}
