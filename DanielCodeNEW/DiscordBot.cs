using System;
using DiscordSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class DiscordBot{
	static void Main(string[] args){
		DiscordClient client = new DiscordClient();
		client.ClientPrivateInformation.Email= "danielshemesh99@gmail.com";
		client.ClientPrivateInformation.Password="mushroom12345";
		client.MessageReceived += (s,e) => {
			if(e.MessageText == "!test"){
				e.Channel.SendMessage("Ayy");
			}};
		client.SendLoginRequest();
		Thread connect = new Thread(client.Connect);
		connect.Start();
}
}