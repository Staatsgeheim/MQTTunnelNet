namespace MQTTunnelNet
{
    public class MQTTunnel
    {
        public enum MQTTunnelLogLevel : int
        {
            None = -1,
            DebugLevel = 0,
            InfoLevel = 1,
            WarnLevel = 2,
            ErrorLevel = 3,
            PanicLevel = 4,
            FatalLevel = 5
        }

        private bool IsServer { get; set; }
        public string ConfigFile { get; private set; }
        public string Control { get; private set; }
        public int LocalPort { get; set; }
        public int RemotePort { get; set; }
        public MQTTunnelLogLevel LogLevel { get; private set; }
        public bool FileBasedConfig { get; private set; }

        private MQTTunnel(string configFile, string control = "", MQTTunnelLogLevel logLevel = MQTTunnelLogLevel.None)
        {
            ConfigFile = configFile;
            LogLevel = logLevel;
            Control = control;
            IsServer = true;
            FileBasedConfig = true;
        }

        private MQTTunnel(string configFile, int localPort, int remotePort, string control = "", MQTTunnelLogLevel logLevel = MQTTunnelLogLevel.None)
        {
            ConfigFile = configFile;
            LogLevel = logLevel;
            Control = control;
            LocalPort = localPort;
            RemotePort = remotePort;
            IsServer = false;
            FileBasedConfig = true;
        }

        private MQTTunnel(string configbuffer, string control = "", MQTTunnelLogLevel logLevel = MQTTunnelLogLevel.None, bool fileBasedConfig = false)
        {
            ConfigFile = configbuffer;
            LogLevel = logLevel;
            Control = control;
            IsServer = true;
            FileBasedConfig = fileBasedConfig;
        }

        private MQTTunnel(string configbuffer, int localPort, int remotePort, string control = "", MQTTunnelLogLevel logLevel = MQTTunnelLogLevel.None, bool fileBasedConfig = false)
        {
            ConfigFile = configbuffer;
            LogLevel = logLevel;
            Control = control;
            LocalPort = localPort;
            RemotePort = remotePort;
            IsServer = false;
            FileBasedConfig = fileBasedConfig;
        }

        public bool Start()
        {
            if(FileBasedConfig)
            {
                if (IsServer)
                    return LibMQTTunnel.StartTunnel(ConfigFile, Control, (int)LogLevel) == 0;
                else
                    return LibMQTTunnel.ConnectTunnel(ConfigFile, Control, LocalPort, RemotePort, (int)LogLevel) == 0;
            }
            else
            {
                if (IsServer)
                    return LibMQTTunnel.StartTunnelMem(ConfigFile, Control, (int)LogLevel) == 0;
                else
                    return LibMQTTunnel.ConnectTunnelMem(ConfigFile, Control, LocalPort, RemotePort, (int)LogLevel) == 0;
            }
            
        }

        private void StartAsyncInternal()
        {
            if (FileBasedConfig)
            {
                if (IsServer)
                    LibMQTTunnel.StartTunnel(ConfigFile, Control, (int)LogLevel);
                else
                    LibMQTTunnel.ConnectTunnel(ConfigFile, Control, LocalPort, RemotePort, (int)LogLevel);
            }
            else
            {
                if (IsServer)
                    LibMQTTunnel.StartTunnelMem(ConfigFile, Control, (int)LogLevel);
                else
                    LibMQTTunnel.ConnectTunnelMem(ConfigFile, Control, LocalPort, RemotePort, (int)LogLevel);
            }            
        }

        public async Task<bool> StartAsync()
        {
            var thread = new Thread(StartAsyncInternal);
            thread.Start();
            return thread.IsAlive;
        }

        public static MQTTunnel CreateServerMem(string configBuffer, string control = "", MQTTunnelLogLevel logLevel = MQTTunnelLogLevel.None) => new MQTTunnel(configBuffer, control, logLevel, false);
        public static MQTTunnel CreateTunnelMem(string configBuffer, int localPort, int remotePort, string control = "", MQTTunnelLogLevel logLevel = MQTTunnelLogLevel.None) => new MQTTunnel(configBuffer, localPort, remotePort, control, logLevel, false);
        public static MQTTunnel CreateServer(string configFile, string control = "", MQTTunnelLogLevel logLevel = MQTTunnelLogLevel.None) => new MQTTunnel(configFile, control, logLevel);
        public static MQTTunnel CreateTunnel(string configFile, int localPort, int remotePort, string control = "", MQTTunnelLogLevel logLevel = MQTTunnelLogLevel.None) => new MQTTunnel(configFile, localPort, remotePort, control, logLevel);
        
    }
}
