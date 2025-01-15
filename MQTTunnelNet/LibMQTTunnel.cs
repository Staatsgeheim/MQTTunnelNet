using System.Runtime.InteropServices;

namespace MQTTunnelNet
{
    internal static class LibMQTTunnel
    {
        [DllImport("libmqttunnel.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int StartTunnel([MarshalAs(UnmanagedType.LPStr)] string config, [MarshalAs(UnmanagedType.LPStr)] string control, int logLevel);

        [DllImport("libmqttunnel.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ConnectTunnel([MarshalAs(UnmanagedType.LPStr)] string config, [MarshalAs(UnmanagedType.LPStr)] string control, int localPort, int remotePort, int logLevel);

        [DllImport("libmqttunnel.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int StartTunnelMem([MarshalAs(UnmanagedType.LPStr)] string configBuffer, [MarshalAs(UnmanagedType.LPStr)] string control, int logLevel);

        [DllImport("libmqttunnel.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ConnectTunnelMem([MarshalAs(UnmanagedType.LPStr)] string configBuffer, [MarshalAs(UnmanagedType.LPStr)] string control, int localPort, int remotePort, int logLevel);
    }
}
