using System;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
public static class Stuff
{
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
    private const uint ButtonID = 0x79;
    public static void Main(string[] args)
    {
        Process test = Process.GetProcessesByName("AppTest")[0];
        IntPtr hwnChild = FindWindowEx(test.MainWindowHandle, IntPtr.Zero, null, null);


        IntPtr temp;
        SendMessageTimeout(hwnChild,BM_CLICK,IntPtr.Zero,IntPtr.Zero,SendMessageTimeoutFlags.SMTO_NORMAL,100,out temp);

        IntPtr hwnChild2 = FindWindow("#32770", null);
        Console.WriteLine(hwnChild + " " + hwnChild2);



        SendMessage(hwnChild2, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        Console.ReadLine();
    }
}