// Licensed under the BSD license
// See the LICENSE file in the project root for more information

using System.Net.Sockets;
using System.Runtime.InteropServices;
using NLog.Targets.Syslog.Settings;

using Debug = System.Diagnostics.Debug;

namespace NLog.Targets.Syslog.MessageSend
{
    internal abstract class SocketInitialization
    {
        public static SocketInitialization ForCurrentOs()
        {
            string rt = RuntimeInformation.OSDescription;

            bool isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

            Debug.WriteLine($"===== NLog.Targets.Syslog detected OS: {rt}");
            Debug.WriteLine($"isMac: {isMac}");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new SocketInitializationForWindows();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ||
                rt.ToLower().Contains("linux"))
            {
                return new SocketInitializationForLinux();
            }

            return new SocketInitializationForOsx();
        }

        public abstract void DisableAddressSharing(Socket socket);

        public void DiscardPendingDataOnClose(Socket socket)
        {
            socket.LingerState = new LingerOption(true, 0);
        }

        public void SetKeepAlive(Socket socket, KeepAliveConfig keepAliveConfig)
        {
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, keepAliveConfig.Enabled);
            if (keepAliveConfig.Enabled)
                ApplyKeepAliveValues(socket, keepAliveConfig);
        }

        protected abstract void ApplyKeepAliveValues(Socket socket, KeepAliveConfig keepAliveConfig);
    }
}