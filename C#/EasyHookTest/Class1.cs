using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyHook;
using EasyHook.RemoteHooking;
using System.Runtime.Remoting;
namespace TestInject
{
    struct Vector{
        public float x, y, z;
    }
    unsafe struct Entity_t {
        private fixed byte _0x0000[0x2]; //0x0
        public short Valid; //0x2 (0xADF8C2)
        private fixed byte _0x0004[0x10]; //0x4
        public Vector Origin; //0x14 (0xADF8D4)
        public Vector Angles; //0x20 (0xADF8E0)
        private fixed byte _0x002C[0x3C];
        public int Flags;
        private fixed byte _0x006C[0xC];
        public Vector OldOrigin;
        private fixed byte _0x0084[0x18];
        public Vector OldAngles;
        private fixed byte _0x00A8[0x28];
        public int ClientNum;
        short Type;
        private fixed byte _0x00D6[0x12];
        public Vector NewOrigin;
        private fixed byte _0x00F4[0x1C];
        public Vector NewAngles;
        private fixed byte _0x011C[0x7C];
        public byte WeaponID;
        private fixed byte _0x0199[0x37];
        public int IsAlive;
        private fixed byte _0x01D4[0x24];
    }
    public class Main : EasyHook.IEntryPoint
    {
        static string channelName;
        RemoteMon Interface;
        LocalHook CreateFileWHook;
        public Main(IContext InContext, String InChannelName) {
            Interface = IpcConnectCLient<RemoteMon>(InChannelName);
            channelName = InChannelName;
            Interface.IsInstalled(GetCurrentProcessId());
        }
        public void Run(IContext InContext, String InChannelName){
            unsafe{
            private Entity_t*
            }
        }
    }
}
