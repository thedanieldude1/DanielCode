using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace BruteForce{

public class BruteGen{
	public static int mode=1;
	List<int> Letters = new List<int>();
    public static char[] Chars2 = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890,./?;:'\|]}[{=+-_)(*&^%$#@!~` ".ToCharArray();
    public static char[] Chars3 = "0123456789".ToCharArray();
    public static char[] Chars1 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
    public static char[] Chars;
	public static int charCount;
    public int length;

	public BruteGen(){
		Letters.Add(0);
		length=1;

        switch (BruteGen.mode)
        {
            case 1:
                BruteGen.Chars = BruteGen.Chars1;
                break;
            case 2:
                BruteGen.Chars = BruteGen.Chars2;
                break;
            case 3:
                BruteGen.Chars = BruteGen.Chars3;
                break;
        }
        charCount = Chars.Length - 1;
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
    public void Flush()
    {
        Letters.Clear();
        Letters.Add(0);
        length = 1;
        switch (mode)
        {
            case 1:
                Chars = Chars1;
                break;
            case 2:
                Chars = Chars2;
                break;
            case 3:
                Chars = Chars3;
                break;
        }
    }
            public void Flush(int longth){
                Letters.Clear() ;
        Letters.Add(0);
		length=longth;
                switch(mode){
            case 1:
                Chars=Chars1;
                break;
            case 2:
                Chars=Chars2;
                break;
            case 3:
                Chars=Chars3;
                break;
        }
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
    [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);
    [Flags]
    public enum SendMessageTimeoutFlags : uint
    {
        SMTO_NORMAL = 0x0,
        SMTO_BLOCK = 0x1,
        SMTO_ABORTIFHUNG = 0x2,
        SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
        SMTO_ERRORONEXIT = 0x0020
    }
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessageTimeout(
        IntPtr windowHandle,
        uint Msg,
        IntPtr wParam,
        IntPtr lParam,
        SendMessageTimeoutFlags flags,
        uint timeout,
        out IntPtr result);
    const int WM_GETTEXT = 0x000D;
    const int WM_GETTEXTLENGTH = 0x000E;
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32.dll")]
    static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);
    public const uint BM_CLICK = 0x00F5;
    private const uint WM_COMMAND = 0x0111;
    private const int BN_CLICKED = 245;
    private const int WM_CLOSE = 0x10;
    const uint WM_SETTEXT = 0x000C;
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetActiveWindow(IntPtr hWnd);
    private const uint ButtonID = 0x79;
    public static void console(string commandles=" "){
		string[] command=Console.ReadLine().Split(" ".ToCharArray());
		switch(commandles!=" "? commandles:command[0].ToLower()){
 
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
                    if (command.Length >= 4)
                    {
                        if (command[3].ToLower() == "true")
                        {
                            Console.WriteLine(main.Combo);
                        }
                    }

				}
				else{
                    main.Combo = command[1];
                    
				
				}
			}
			catch(IndexOutOfRangeException e){
				Console.WriteLine("Error");
			}
			console();
			break;
            case "mode":
                try{
                BruteGen.mode=Int32.Parse(command[1]);
                main.gen = new BruteGen();
                }
                catch(Exception e){
                    Console.WriteLine("ERROR TRY AGAIN FAGGOT AYY LMAO");
                }
                console();
            break;
			case "crack":
			try{
                main.gen.Flush();
                if (command.Length >= 2)
                {
                    main.gen = new BruteGen(Int32.Parse(command[1]));
                }
                Process test = Process.GetProcessesByName("AppTest")[0];
                SetActiveWindow(test.MainWindowHandle);
                IntPtr hwnChild = FindWindowEx(test.MainWindowHandle, IntPtr.Zero, null, "Click Me");
                 IntPtr text = FindWindowEx(test.MainWindowHandle, IntPtr.Zero, "WindowsForms10.EDIT.app.0.2bf8098_r11_ad1", null);


                //IntPtr temp;
                //SendMessageTimeout(hwnChild, BM_CLICK, IntPtr.Zero, IntPtr.Zero, SendMessageTimeoutFlags.SMTO_NORMAL, 100, out temp);

                
            DateTime originaltime = DateTime.Now;
			bool found = false;
			while(found==false){
			main.gen.Tick();
            SendMessage(text,WM_SETTEXT,0,new StringBuilder(main.gen.ToString()));
            IntPtr temp;
            SendMessageTimeout(hwnChild, BM_CLICK, IntPtr.Zero, IntPtr.Zero, SendMessageTimeoutFlags.SMTO_NORMAL, 10, out temp);
            IntPtr hwnChild2 = FindWindow("#32770", null);
            IntPtr hwnOutput = FindWindowEx(hwnChild2, IntPtr.Zero, "Static", null);
            int length = SendMessage(hwnOutput, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero).ToInt32();
            StringBuilder output = new StringBuilder(length + 1);
            SendMessage(hwnOutput,WM_GETTEXT,output.Capacity,output);
           // Console.WriteLine(output.ToString());
            if (output.ToString()=="Login Successful") {
                found = true;
                DateTime time = DateTime.Now;
                Console.WriteLine("Combo Found: \"{0}\"! Took {1} seconds.", main.gen, (time - originaltime).TotalSeconds);
            }
            
            SendMessage(hwnChild2, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
			//Console.WriteLine(main.gen);
		//	if(main.gen.ToString()==main.Combo){
		//	found=true;
            
           // Console.WriteLine("Combo Found: \"{0}\"! Took {1} seconds.", main.gen, (time - originaltime).TotalSeconds);
			}
		        
			//}
			}
			
			catch(IndexOutOfRangeException e){
				Console.WriteLine("Error in the thing: {0}",e.Message);
			}
			console();
			break;
            case "help":
            Console.Write("combo <combination>-sets the target combo to combination \r\ncombo random <length> <printcombo>-creates a random combo of length <length>.\r\n If printcombo is true it will print the generated combination. \r\nmode <mode>-Sets which characters the generator uses. 1=All numbers and uppercase and lowercase letters. \r\n2=Every key on the keyboard except quotation and space. \r\n3=Only Numbers.\r\n");
            console();
            break;
		default:
            Console.WriteLine("Unknown Command, type help for information");
		console();
		break;
		
		}
	}
}
public static class main{
	public static string Combo="daniel1";
                        
    public static BruteGen gen;
	public static void Main(string[] args){

        gen = new BruteGen();

		Program.console();

}}
}