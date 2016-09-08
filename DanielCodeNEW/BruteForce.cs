using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualBasic;
namespace BruteForce{

public class BruteGen{

	List<int> Letters = new List<int>();
	public static char[] Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray(); //1234567890,./?><;:'[{]}=+-_)(*&^%$#@!
	int  charCount;
	int length;
	public BruteGen(){
		Letters.Add(0);
		length=1;
		charCount=Chars.Length-1;
	}
	public BruteGen(int gength){
		length=gength;
		for(int i =0;i<gength;i++){
		Letters.Add(0);
		}
		charCount=Chars.Length-1;
	}
	static public implicit operator string(BruteGen gen){
		return gen.ToString();
	}
	public void Tick(){
	int carry=1;
	for(int i =length-1;i>=0;i--){
		Letters[i]+=carry;
		if(Letters[i]>charCount){
			Letters[i]=0;
			carry=1;
		}
		else{
			carry=0;
		}
	
	}
	if(carry==1){
	
		for(int i =0;i<length-1;i++){
			Letters[i]=0;
		}
		length++;
		Letters.Add(0);
	}
}
	public override string ToString(){
		char[] thing=new char[length];
		for(int i=length-1;i>=0;i--){
			
			thing[i]=Chars[Letters[i]];
		}
		return new string(thing);
	}

}
public static class Program{
	public static void console(){
		string[] command=Console.ReadLine().Split(" ".ToCharArray());
		switch(command[0].ToLower()){
		case "combo":
			try{
				if(command[1]=="random"){
					int chars=BruteGen.Chars.Length-1;
					int len = Int32.Parse(command[2]);
					char[] test = new char[len];
					Random r = new Random();
					for(int i =0;i<len;i++){
						test[i]=BruteGen.Chars[r.Next(0,chars)];
					}
					main.Combo=new string(test);
					File.WriteAllText(@"test.txt",String.Empty);
					using(StreamWriter writer = new StreamWriter("test.txt")){
						
						writer.Write(main.Combo);
					}
					Console.WriteLine(main.Combo);
				}
				else{
				main.Combo=command[1];
				}
			}
			catch(IndexOutOfRangeException e){
				Console.WriteLine("Error");
			}
			console();
			break;
			case "crack":
			try{

			File.WriteAllText(@"tost.txt",String.Empty);
						StreamWriter writer = new StreamWriter("tost.txt");
						
						
					
			bool found = false;
			while(found==false){
			main.gen.Tick();
			
			writer.WriteLine(main.gen.ToString());
			
			
			if(main.gen.ToString()==main.Combo){

			found=true;
			Console.WriteLine("Combo Found! {0}",main.gen);
			writer.Close();
		        }
			}
			}
			
			catch(IndexOutOfRangeException e){
				Console.WriteLine("Error");
			}
			console();
			break;
		default:
		console();
		break;
		
		}
	}
}
public static class main{
	public static string Combo="daniel1";

	public static BruteGen gen= new BruteGen();
	public static void Main(string[] args){

	

		Program.console();

}}
}