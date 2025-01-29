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

#if NET45_OR_GREATER && !NET48_OR_GREATER
            if (IntPtr.Size == 4)
            {
                libmqttunnel = WINDOWS.LoadLibrary(Path.Combine(libmqttunnelPath, WINDOWS.LibraryName_X86));
            }
            else
            {
                libmqttunnel =  WINDOWS.LoadLibrary(Path.Combine(libmqttunnelPath, WINDOWS.LibraryName_X64));
            }

            if (libmqttunnel == IntPtr.Zero)
                return;
            
            StartTunnel = GetMethodDelegate<DStartTunnel>(WINDOWS.GetProcAddress(libmqttunnel, "StartTunnel"));
            ConnectTunnel = GetMethodDelegate<DConnectTunnel>(WINDOWS.GetProcAddress(libmqttunnel, "ConnectTunnel"));
            StartTunnelMem = GetMethodDelegate<DStartTunnelMem>(WINDOWS.GetProcAddress(libmqttunnel, "StartTunnelMem"));
            ConnectTunnelMem = GetMethodDelegate<DConnectTunnelMem>(WINDOWS.GetProcAddress(libmqttunnel, "ConnectTunnelMem"));
#endif
#if NETSTANDARD2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NET48_OR_GREATER
            if (PlatformApis.IsWindows)
            {
                if (PlatformApis.ProcessArchitecture == CommonPlatformDetection.CpuArchitecture.X64)
                {
                    libmqttunnel = WINDOWS.LoadLibrary(Path.Combine(libmqttunnelPath, WINDOWS.LibraryName_X64));
                    
                }
                else if (PlatformApis.ProcessArchitecture == CommonPlatformDetection.CpuArchitecture.X86)
                {
                    libmqttunnel = WINDOWS.LoadLibrary(Path.Combine(libmqttunnelPath, WINDOWS.LibraryName_X86));
                }
                if (libmqttunnel == IntPtr.Zero)
                    return;

                StartTunnel = GetMethodDelegate<DStartTunnel>(WINDOWS.GetProcAddress(libmqttunnel, "StartTunnel"));
                ConnectTunnel = GetMethodDelegate<DConnectTunnel>(WINDOWS.GetProcAddress(libmqttunnel, "ConnectTunnel"));
                StartTunnelMem = GetMethodDelegate<DStartTunnelMem>(WINDOWS.GetProcAddress(libmqttunnel, "StartTunnelMem"));
                ConnectTunnelMem = GetMethodDelegate<DConnectTunnelMem>(WINDOWS.GetProcAddress(libmqttunnel, "ConnectTunnelMem"));
            }
#endif
#if NETSTANDARD2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            else if (PlatformApis.IsLinux)
            {
                if (PlatformApis.ProcessArchitecture == CommonPlatformDetection.CpuArchitecture.X64)
                {
                    if(PlatformApis.IsNetCore)
                        libmqttunnel = LINUX.CoreCLR.dlopen(Path.Combine(libmqttunnelPath, LINUX.LibraryName_X64), 9);
                    else
                        libmqttunnel = LINUX.dlopen(Path.Combine(libmqttunnelPath, LINUX.LibraryName_X64), 9);

                }
                else if (PlatformApis.ProcessArchitecture == CommonPlatformDetection.CpuArchitecture.X86)
                {
                    if (PlatformApis.IsNetCore)
                        libmqttunnel = LINUX.CoreCLR.dlopen(Path.Combine(libmqttunnelPath, LINUX.LibraryName_X86), 9);
                    else
                        libmqttunnel = LINUX.dlopen(Path.Combine(libmqttunnelPath, LINUX.LibraryName_X86), 9);
                }
                else if (PlatformApis.ProcessArchitecture == CommonPlatformDetection.CpuArchitecture.Arm64)
                {
                    if (PlatformApis.IsNetCore)
                        libmqttunnel = LINUX.CoreCLR.dlopen(Path.Combine(libmqttunnelPath, LINUX.LibraryName_ARM64), 9);
                    else
                        libmqttunnel = LINUX.dlopen(Path.Combine(libmqttunnelPath, LINUX.LibraryName_ARM64), 9);
                }
                if (libmqttunnel == IntPtr.Zero)
                    return;

                if (PlatformApis.IsNetCore)
                {
                    StartTunnel = GetMethodDelegate<DStartTunnel>(LINUX.CoreCLR.dlsym(libmqttunnel, "StartTunnel"));
                    ConnectTunnel = GetMethodDelegate<DConnectTunnel>(LINUX.CoreCLR.dlsym(libmqttunnel, "ConnectTunnel"));
                    StartTunnelMem = GetMethodDelegate<DStartTunnelMem>(LINUX.CoreCLR.dlsym(libmqttunnel, "StartTunnelMem"));
                    ConnectTunnelMem = GetMethodDelegate<DConnectTunnelMem>(LINUX.CoreCLR.dlsym(libmqttunnel, "ConnectTunnelMem"));
                }
                else
                {
                    StartTunnel = GetMethodDelegate<DStartTunnel>(LINUX.dlsym(libmqttunnel, "StartTunnel"));
                    ConnectTunnel = GetMethodDelegate<DConnectTunnel>(LINUX.dlsym(libmqttunnel, "ConnectTunnel"));
                    StartTunnelMem = GetMethodDelegate<DStartTunnelMem>(LINUX.dlsym(libmqttunnel, "StartTunnelMem"));
                    ConnectTunnelMem = GetMethodDelegate<DConnectTunnelMem>(LINUX.dlsym(libmqttunnel, "ConnectTunnelMem"));
                }
                
            }
            else if (PlatformApis.IsMacOSX)
            {
                if (PlatformApis.ProcessArchitecture == CommonPlatformDetection.CpuArchitecture.X64)
                {
                    libmqttunnel = MACOS.dlopen(Path.Combine(libmqttunnelPath, MACOS.LibraryName_X64), 9);

                }
                else if (PlatformApis.ProcessArchitecture == CommonPlatformDetection.CpuArchitecture.Arm64)
                {
                    libmqttunnel = MACOS.dlopen(Path.Combine(libmqttunnelPath, MACOS.LibraryName_ARM64), 9);
                }
                if (libmqttunnel == IntPtr.Zero)
                    return;

                StartTunnel = GetMethodDelegate<DStartTunnel>(MACOS.dlsym(libmqttunnel, "StartTunnel"));
                ConnectTunnel = GetMethodDelegate<DConnectTunnel>(MACOS.dlsym(libmqttunnel, "ConnectTunnel"));
                StartTunnelMem = GetMethodDelegate<DStartTunnelMem>(MACOS.dlsym(libmqttunnel, "StartTunnelMem"));
                ConnectTunnelMem = GetMethodDelegate<DConnectTunnelMem>(MACOS.dlsym(libmqttunnel, "ConnectTunnelMem"));
            }
#endif
#if NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#if NETCOREAPP3_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            if (PlatformApis.IsWindows) {
#else
            if (OperatingSystem.IsWindows()) {
#endif
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64) {
                    libmqttunnel = NativeLibrary.Load(Path.Combine(libmqttunnelPath, WINDOWS.LibraryName_X64));
                } else if (RuntimeInformation.ProcessArchitecture == Architecture.X86) {
                    libmqttunnel = NativeLibrary.Load(Path.Combine(libmqttunnelPath, WINDOWS.LibraryName_X86));
                }
#if NETCOREAPP3_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            } else if (PlatformApis.IsMacOSX) {
#else
            } else if (OperatingSystem.IsMacOS()) {
#endif
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64) {
                    libmqttunnel = NativeLibrary.Load(Path.Combine(libmqttunnelPath, MACOS.LibraryName_X64));
                }
                else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64) {
                    libmqttunnel = NativeLibrary.Load(Path.Combine(libmqttunnelPath, MACOS.LibraryName_ARM64));
                }
#if NETCOREAPP3_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            } else if (PlatformApis.IsLinux) {
#else
            } else if (OperatingSystem.IsLinux()) {
#endif
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64) {
                    libmqttunnel = NativeLibrary.Load(Path.Combine(libmqttunnelPath, LINUX.LibraryName_X64));
                } else if (RuntimeInformation.ProcessArchitecture == Architecture.X86) {
                    libmqttunnel = NativeLibrary.Load(Path.Combine(libmqttunnelPath, LINUX.LibraryName_X86));
                } else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64) {
                    libmqttunnel = NativeLibrary.Load(Path.Combine(libmqttunnelPath, LINUX.LibraryName_ARM64));
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

        static class MACOS
        {
            internal const string LibraryName_X64 = "libmqttunnel_x64.dylib";
            internal const string LibraryName_ARM64 = "libmqttunnel_arm64.dylib";

            [DllImport("libSystem.dylib")]
            internal static extern IntPtr dlopen(string filename, int flags);

            [DllImport("libSystem.dylib")]
            internal static extern IntPtr dlerror();

            [DllImport("libSystem.dylib")]
            internal static extern IntPtr dlsym(IntPtr handle, string symbol);
        }

        static class WINDOWS
        {
            internal const string LibraryName_X86 = "libmqttunnel_x86.dll";
            internal const string LibraryName_X64 = "libmqttunnel_x64.dll";


            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern IntPtr LoadLibrary(string filename);

            [DllImport("kernel32.dll")]
            internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        }

        static class LINUX
        {
            internal const string LibraryName_X64 = "libmqttunnel_x64.so";
            internal const string LibraryName_X86 = "libmqttunnel_x86.so";
            internal const string LibraryName_ARM64 = "libmqttunnel_arm64.so";

            internal static class CoreCLR
            {
                [DllImport("libcoreclr.so")]
                internal static extern IntPtr dlopen(string filename, int flags);

                [DllImport("libcoreclr.so")]
                internal static extern IntPtr dlerror();

                [DllImport("libcoreclr.so")]
                internal static extern IntPtr dlsym(IntPtr handle, string symbol);
            }

            [DllImport("libdl.so")]
            internal static extern IntPtr dlopen(string filename, int flags);

            [DllImport("libdl.so")]
            internal static extern IntPtr dlerror();

            [DllImport("libdl.so")]
            internal static extern IntPtr dlsym(IntPtr handle, string symbol);
        }
    }
}