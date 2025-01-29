using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MQTTunnelNet
{
#if NET48_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
    internal static class CommonPlatformDetection
    {
        public static CommonPlatformDetection.OSKind GetOSKind()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return CommonPlatformDetection.OSKind.Windows;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return CommonPlatformDetection.OSKind.Linux;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return CommonPlatformDetection.OSKind.MacOSX;
            }
            return CommonPlatformDetection.OSKind.Unknown;
        }

        public static CommonPlatformDetection.CpuArchitecture GetProcessArchitecture()
        {
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X86:
                    return CommonPlatformDetection.CpuArchitecture.X86;
                case Architecture.X64:
                    return CommonPlatformDetection.CpuArchitecture.X64;
                case Architecture.Arm64:
                    return CommonPlatformDetection.CpuArchitecture.Arm64;
            }
            return CommonPlatformDetection.CpuArchitecture.Unknown;
        }

        public enum OSKind
        {
            Unknown,
            Windows,
            Linux,
            MacOSX
        }

        public enum CpuArchitecture
        {
            Unknown,
            X86,
            X64,
            Arm64
        }
    }
#endif
}
