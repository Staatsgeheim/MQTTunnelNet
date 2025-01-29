using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MQTTunnelNet
{
    internal static class LibMQTTunnel
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int DStartTunnel([MarshalAs(UnmanagedType.LPStr)] string config, [MarshalAs(UnmanagedType.LPStr)] string control, int logLevel);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int DConnectTunnel([MarshalAs(UnmanagedType.LPStr)] string config, [MarshalAs(UnmanagedType.LPStr)] string control, int localPort, int remotePort, int logLevel);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int DStartTunnelMem([MarshalAs(UnmanagedType.LPStr)] string configBuffer, [MarshalAs(UnmanagedType.LPStr)] string control, int logLevel);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int DConnectTunnelMem([MarshalAs(UnmanagedType.LPStr)] string configBuffer, [MarshalAs(UnmanagedType.LPStr)] string control, int localPort, int remotePort, int logLevel);

        internal static DStartTunnel StartTunnel { get; set; }
        internal static DConnectTunnel ConnectTunnel { get; set; }
        internal static DStartTunnelMem StartTunnelMem { get; set; }
        internal static DConnectTunnelMem ConnectTunnelMem { get; set; }

        static LibMQTTunnel()
        {
            var libmqttunnelPath = Environment.GetEnvironmentVariable("LIBMQTTUNNEL_PATH") ?? "";
            var libmqttunnel = IntPtr.Zero;

#if NET45_OR_GREATER || NETSTANDARD2_0 || NETSTANDARD2_1
            if (IntPtr.Size == 4)
            {
                libmqttunnel = WINDOWS_X86.LoadLibrary(Path.Combine(libmqttunnelPath, WINDOWS_X86.LibraryName));
            }
            else
            {
                libmqttunnel =  WINDOWS_X64.LoadLibrary(Path.Combine(libmqttunnelPath, WINDOWS_X64.LibraryName));
            }

            if (libmqttunnel == IntPtr.Zero)
                return;
            
            StartTunnel = GetMethodDelegate<DStartTunnel>(WINDOWS_X86.GetProcAddress(libmqttunnel, "StartTunnel"));
            ConnectTunnel = GetMethodDelegate<DConnectTunnel>(WINDOWS_X86.GetProcAddress(libmqttunnel, "ConnectTunnel"));
            StartTunnelMem = GetMethodDelegate<DStartTunnelMem>(WINDOWS_X86.GetProcAddress(libmqttunnel, "StartTunnelMem"));
            ConnectTunnelMem = GetMethodDelegate<DConnectTunnelMem>(WINDOWS_X86.GetProcAddress(libmqttunnel, "ConnectTunnelMem"));
#endif
#if NET5_0_OR_GREATER || NETCOREAPP3_0 || NETCOREAPP3_1
#if NETCOREAPP3_0 || NETCOREAPP3_1
            if (RuntimeInformation.OSDescription.Contains("Microsoft Windows")) {
#else
            if (OperatingSystem.IsWindows()) {
#endif
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64) {
                    libmqttunnel = NativeLibrary.Load(Path.Combine(libmqttunnelPath, WINDOWS_X64.LibraryName));
                } else if (RuntimeInformation.ProcessArchitecture == Architecture.X86) {
                    libmqttunnel = NativeLibrary.Load(Path.Combine(libmqttunnelPath, WINDOWS_X86.LibraryName));
                }
#if NETCOREAPP3_0 || NETCOREAPP3_1
            } else if (RuntimeInformation.OSDescription.Contains("Darwin")) {
#else
            } else if (OperatingSystem.IsMacOS()) {
#endif
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64) {
                    libmqttunnel = NativeLibrary.Load(Path.Combine(libmqttunnelPath, MACOS_X64.LibraryName));
                }
                else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64) {
                    libmqttunnel = NativeLibrary.Load(Path.Combine(libmqttunnelPath, MACOS_ARM64.LibraryName));
                }
#if NETCOREAPP3_0 || NETCOREAPP3_1
            } else {
#else
            } else if (OperatingSystem.IsLinux()) {
#endif
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64) {
                    libmqttunnel = NativeLibrary.Load(Path.Combine(libmqttunnelPath, LINUX_X64.LibraryName));
                } else if (RuntimeInformation.ProcessArchitecture == Architecture.X86) {
                    libmqttunnel = NativeLibrary.Load(Path.Combine(libmqttunnelPath, LINUX_X86.LibraryName));
                } else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64) {
                    libmqttunnel = NativeLibrary.Load(Path.Combine(libmqttunnelPath, LINUX_ARM64.LibraryName));
                }
            }
            if(libmqttunnel == IntPtr.Zero)
                return;

            StartTunnel = GetMethodDelegate<DStartTunnel>(NativeLibrary.GetExport(libmqttunnel, "StartTunnel"));
            ConnectTunnel = GetMethodDelegate<DConnectTunnel>(NativeLibrary.GetExport(libmqttunnel, "ConnectTunnel"));
            StartTunnelMem = GetMethodDelegate<DStartTunnelMem>(NativeLibrary.GetExport(libmqttunnel, "StartTunnelMem"));
            ConnectTunnelMem = GetMethodDelegate<DConnectTunnelMem>(NativeLibrary.GetExport(libmqttunnel, "ConnectTunnelMem"));
#endif
            }

        private static T GetMethodDelegate<T>(IntPtr functionPtr) where T : class
        {
            return Marshal.GetDelegateForFunctionPointer(functionPtr, typeof(T)) as T;
        }

        static class MACOS_X64
        {
            internal const string LibraryName = "libmqttunnel_x64.dylib";
        }

        static class MACOS_ARM64
        {
            internal const string LibraryName = "libmqttunnel_arm64.dylib";
        }

        static class WINDOWS_X64
        {
            internal const string LibraryName = "libmqttunnel_x64.dll";

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern IntPtr LoadLibrary(string filename);

            [DllImport("kernel32.dll")]
            internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        }

        static class WINDOWS_X86
        {
            internal const string LibraryName = "libmqttunnel_x86.dll";

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern IntPtr LoadLibrary(string filename);

            [DllImport("kernel32.dll")]
            internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        }

        static class LINUX_X64
        {
            internal const string LibraryName = "libmqttunnel_x64.so";
        }

        static class LINUX_X86
        {
            internal const string LibraryName = "libmqttunnel_x86.so";
        }

        static class LINUX_ARM64
        {
            internal const string LibraryName = "libmqttunnel_arm64.so";
        }
    }
}