using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MQTTunnelNet
{
#if NET48_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
    internal static class PlatformApis
    {
        private const string UnityEngineAssemblyName = "UnityEngine";

        private const string UnityEngineApplicationClassName = "UnityEngine.Application";

        private const string UnityIPhonePlayer = "IPhonePlayer";

        private const string XamarinAndroidObjectClassName = "Java.Lang.Object, Mono.Android";

        private const string XamarinIOSObjectClassName = "Foundation.NSObject, Xamarin.iOS";

        private static readonly bool isLinux;

        private static readonly bool isMacOSX;

        private static readonly bool isWindows;

        private static readonly bool isMono;

        private static readonly bool isNet5OrHigher;

        private static readonly bool isNetCore;

        private static readonly string frameworkDescription;

        private static readonly string clrVersion;

        private static readonly string unityApplicationPlatform;

        private static readonly bool isXamarin;

        private static readonly bool isXamarinIOS;

        private static readonly bool isXamarinAndroid;

        public static bool IsLinux => isLinux;

        public static bool IsMacOSX => isMacOSX;

        public static bool IsWindows => isWindows;

        public static bool IsMono => isMono;

        /// <summary>
        /// true if running on Unity platform.
        /// </summary>
        public static bool IsUnity => unityApplicationPlatform != null;

        /// <summary>
        /// true if running on Unity iOS, false otherwise.
        /// </summary>
        public static bool IsUnityIOS => unityApplicationPlatform == "IPhonePlayer";

        /// <summary>
        /// true if running on a Xamarin platform (either Xamarin.Android or Xamarin.iOS),
        /// false otherwise.
        /// </summary>
        public static bool IsXamarin => isXamarin;

        /// <summary>
        /// true if running on Xamarin.iOS, false otherwise.
        /// </summary>
        public static bool IsXamarinIOS => isXamarinIOS;

        /// <summary>
        /// true if running on Xamarin.Android, false otherwise.
        /// </summary>
        public static bool IsXamarinAndroid => isXamarinAndroid;

        /// <summary>
        /// true if running on .NET 5+, false otherwise.
        /// </summary>
        public static bool IsNet5OrHigher => isNet5OrHigher;

        /// <summary>
        /// Contains <c>RuntimeInformation.FrameworkDescription</c> if the property is available on current TFM.
        /// <c>null</c> otherwise.
        /// </summary>
        public static string FrameworkDescription => frameworkDescription;

        /// <summary>
        /// Contains the version of common language runtime obtained from <c>Environment.Version</c>
        /// if the property is available on current TFM. <c>null</c> otherwise.
        /// </summary>
        public static string ClrVersion => clrVersion;

        /// <summary>
        /// true if running on .NET Core (CoreCLR) or NET 5+, false otherwise.
        /// </summary>
        public static bool IsNetCore => isNetCore;

        public static bool Is64Bit => IntPtr.Size == 8;

        public static CommonPlatformDetection.CpuArchitecture ProcessArchitecture => CommonPlatformDetection.GetProcessArchitecture();

        static PlatformApis()
        {
            CommonPlatformDetection.OSKind oSKind = CommonPlatformDetection.GetOSKind();
            isLinux = oSKind == CommonPlatformDetection.OSKind.Linux;
            isMacOSX = oSKind == CommonPlatformDetection.OSKind.MacOSX;
            isWindows = oSKind == CommonPlatformDetection.OSKind.Windows;
            isNet5OrHigher = Environment.Version.Major >= 5;
            isNetCore = isNet5OrHigher || RuntimeInformation.FrameworkDescription.StartsWith(".NET Core");
            frameworkDescription = TryGetFrameworkDescription();
            clrVersion = TryGetClrVersion();
            isMono = Type.GetType("Mono.Runtime") != null;
            unityApplicationPlatform = TryGetUnityApplicationPlatform();
            isXamarinIOS = Type.GetType("Foundation.NSObject, Xamarin.iOS") != null;
            isXamarinAndroid = Type.GetType("Java.Lang.Object, Mono.Android") != null;
            isXamarin = isXamarinIOS || isXamarinAndroid;
        }

        public static void CheckState(bool condition, string errorMessage)
        {
            if (!condition)
            {
                Throw(errorMessage);
            }
            static void Throw(string errorMessage)
            {
                throw new InvalidOperationException(errorMessage);
            }
        }

        /// <summary>
        /// Returns <c>UnityEngine.Application.platform</c> as a string.
        /// See https://docs.unity3d.com/ScriptReference/Application-platform.html for possible values.
        /// Value is obtained via reflection to avoid compile-time dependency on Unity.
        /// This method should only be called if <c>IsUnity</c> is <c>true</c>.
        /// </summary>
        public static string GetUnityApplicationPlatform()
        {
            CheckState(IsUnity, "Not running on Unity.");
            return unityApplicationPlatform;
        }

        /// <summary>
        /// Returns <c>UnityEngine.Application.platform</c> as a string or <c>null</c>
        /// if not running on Unity.
        /// Value is obtained via reflection to avoid compile-time dependency on Unity.
        /// </summary>
        private static string TryGetUnityApplicationPlatform()
        {
            PropertyInfo platformProperty = (AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly assembly) => assembly.GetName().Name == "UnityEngine")?.GetType("UnityEngine.Application"))?.GetTypeInfo().GetProperty("platform", BindingFlags.Static | BindingFlags.Public);
            try
            {
                return platformProperty?.GetValue(null)?.ToString();
            }
            catch (TargetInvocationException)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns description of the framework this process is running on.
        /// Value is based on <c>RuntimeInformation.FrameworkDescription</c>.
        /// </summary>
        private static string TryGetFrameworkDescription()
        {
            return RuntimeInformation.FrameworkDescription;
        }

        /// <summary>
        /// Returns version of the common language runtime this process is running on.
        /// Value is based on <c>Environment.Version</c>.
        /// </summary>
        private static string TryGetClrVersion()
        {
            return Environment.Version.ToString();
        }
    }
#endif
}
