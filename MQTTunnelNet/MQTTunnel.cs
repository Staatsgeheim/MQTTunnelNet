namespace MQTTunnelNet
{
    public class MQTTunnel
    {
        private bool IsServer { get; set; }
        public string ConfigFile { get; private set; }
        public string Control { get; private set; }
        public int LocalPort { get; set; }
        public int RemotePort { get; set; }
        public bool Debug { get; private set; }

        private MQTTunnel(string configFile, string control = "", bool debug = false)
        {
            ConfigFile = configFile;
            Debug = debug;
            Control = control;
            IsServer = true;
        }

        private MQTTunnel(string configFile, int localPort, int remotePort, string control = "", bool debug = false)
        {
            ConfigFile = configFile;
            Debug = debug;
            Control = control;
            LocalPort = localPort;
            RemotePort = remotePort;
            IsServer = false;
        }

        public bool Start()
        {
            if (IsServer)
                return LibMQTTunnel.StartTunnel(ConfigFile, Control, Debug ? 1 : 0) == 0;
            else
                return LibMQTTunnel.ConnectTunnel(ConfigFile, Control, LocalPort, RemotePort, Debug ? 1 : 0) == 0;
        }

        private void StartAsyncInternal()
        {
            if (IsServer)
                LibMQTTunnel.StartTunnel(ConfigFile, Control, Debug ? 1 : 0);
            else
                LibMQTTunnel.ConnectTunnel(ConfigFile, Control, LocalPort, RemotePort, Debug ? 1 : 0);
        }

        public async Task<bool> StartAsync()
        {
            var thread = new Thread(StartAsyncInternal);
            thread.Start();
            return thread.IsAlive;
        }

        public static MQTTunnel CreateServer(string configFile, string control = "", bool debug = false) => new MQTTunnel(configFile, control, debug);
        public static MQTTunnel CreateTunnel(string configFile, int localPort, int remotePort, string control = "", bool debug = false) => new MQTTunnel(configFile, localPort, remotePort, control, debug);
    }
}
