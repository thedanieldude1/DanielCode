using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
namespace Chat{
public static class Extensions{
public static TcpState GetState(this TcpClient tcpClient)
{
  var foo = IPGlobalProperties.GetIPGlobalProperties()
    .GetActiveTcpConnections()
    .SingleOrDefault(x => x.LocalEndPoint.Equals(tcpClient.Client.LocalEndPoint));
  return foo != null ? foo.State : TcpState.Unknown;
}
}
}