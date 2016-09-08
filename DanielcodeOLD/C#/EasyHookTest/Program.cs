using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
namespace ConsoleApplication1
{
    class RemoteMon : MarshalByRefObject
    {
        public void IsInstalled(int InClientPID) {
            Console.WriteLine("Success! "+InClientPID);
        }
    }
}
