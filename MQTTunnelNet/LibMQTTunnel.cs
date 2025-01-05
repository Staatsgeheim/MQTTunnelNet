using System.Runtime.InteropServices;

namespace MQTTunnelNet
{
    internal static class LibMQTTunnel
    {
        [DllImport("libmqttunnel.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int StartTunnel([MarshalAs(UnmanagedType.LPStr)] string config, [MarshalAs(UnmanagedType.LPStr)] string control, int debug);

        [DllImport("libmqttunnel.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ConnectTunnel([MarshalAs(UnmanagedType.LPStr)] string config, [MarshalAs(UnmanagedType.LPStr)] string control, int localPort, int remotePort, int debug);
    }
}
