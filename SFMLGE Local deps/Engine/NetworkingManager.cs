using LiteNetLib;
using LiteNetLib.Utils;
using SFML_Game_Engine.System;
using System.Diagnostics;
using System.Threading.Channels;
using Windows.Storage.Streams;

namespace SFML_Game_Engine
{
    public class NetworkingManager
    {
        public Project Project;

        public EventBasedNetListener listener = new EventBasedNetListener();

        public NetManager handler;
        public NetPeer serverPeer;

        string NetKey = "default";

        public string IP;
        public int port;

        int maxPeers = 32;

        public float pollRate = 1 / 30f;
        public float positionUpdateRate = 1 / 20f;

        public List<NetPeer> peers = new List<NetPeer>();

        public List<GameObject> syncedObjects = new List<GameObject>();

        Stopwatch pollTimer = Stopwatch.StartNew();
        Stopwatch posTimer = new Stopwatch();

        Stopwatch connectTimer = Stopwatch.StartNew();

        public bool isClient = false;

        public event Action<NetPeer, string> OnStringRecieve;
        public event Action NetworkingUpdate;

        public bool Started { get; private set; } = false;
        bool closed = false;

        public NetworkingManager(Project Project, string IP, int port) // for my sanity, this will be client side code only!
        {
            this.Project = Project;
            this.IP = IP;
            this.port = port;
        }

        public void Start(bool asHost)
        {
            if(Started) return;
            isClient = !asHost;
            Console.WriteLine("NetworkingManager starting");
            if (isClient)
            {
                Console.WriteLine("Starting as client.");
                StartAsClient();
            } else
            {
                Console.WriteLine("Starting as server/host");
                StartAsServer();
            }
            posTimer.Start();
            Console.WriteLine("Networking manager setup finished.");
        }

        void StartAsClient()
        {
            handler = new NetManager(listener);
            handler.Start();
            Started = true;

            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, chanel) =>
            {
                Console.WriteLine("Got something");
                bool isString = dataReader.TryGetString(out string result);

                if (isString)
                {
                    if (result.StartsWith("echo"))
                    {
                        return;
                    }
                    OnStringRecieve?.Invoke(fromPeer, result);
                }

                dataReader.Recycle();
            };

            serverPeer = TryServerConnect(IP, port, NetKey);
        }

        void StartAsServer()
        {
            handler = new NetManager(listener);
            handler.Start(port);

            listener.ConnectionRequestEvent += request =>
            {
                Console.WriteLine("REQUESTED");
                if(handler.ConnectedPeersCount < maxPeers)
                {
                    request.AcceptIfKey(NetKey);
                } else
                {
                    request.Reject();
                }
            };

            listener.PeerConnectedEvent += peer =>
            {
                peers.Add(peer);
                Console.WriteLine("Peer {0} connected", peer.Address);
            };

            listener.PeerDisconnectedEvent += (peer, discInfo) =>
            {
                Console.WriteLine(discInfo.Reason);
                peers.Remove(peer);
            };

            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
            {
                bool isString = dataReader.TryGetString(out string result);
                Console.WriteLine("Got message");
                if (isString)
                {
                    if (result.StartsWith("echo"))
                    {
                        string toEcho = result.Remove(0, 4);
                        SendToClients(toEcho);
                        Console.WriteLine("GotEcho");
                        OnStringRecieve?.Invoke(fromPeer, toEcho);
                        return;
                    }
                    OnStringRecieve?.Invoke(fromPeer, result);
                }

                dataReader.Recycle();
            };

            Started = true;
        }
        
        public void EchoToAll(string message)
        {
            if (isClient)
            {
                SendToSever("echo"+message);
            } else
            {
                SendToClients(message);
            }
        }

        public void SendToSever(string message)
        {
            if (Started)
            {
                NetDataWriter writer = new NetDataWriter();
                writer.Put(message);
                serverPeer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }

        public void SendToClients(string message)
        {
            if (Started)
            {
                NetDataWriter writer = new NetDataWriter();
                writer.Put(message);
                foreach(NetPeer peer in peers)
                {
                    peer.Send(writer, DeliveryMethod.ReliableOrdered);
                }
            }
        }

        void SyncToClients()
        {
            foreach(GameObject go in syncedObjects)
            {
                NetDataWriter writer = new NetDataWriter();

                //formatting as follows:
                // {syncEvent,SceneName,GameObjectName,WorldPosition}
                writer.Put($"syncEvent,{Project.ActiveScene!.Name},{go.name},{go.transform.WorldPosition.x}:{go.transform.WorldPosition.y}");
                foreach (NetPeer peer in peers)
                {
                    peer.Send(writer, DeliveryMethod.Unreliable);
                }
            }
        }

        public NetPeer TryServerConnect(string IP, int port, string NetKey)
        {
            Console.Write("Attemping to connect to {0}:{1} with NetKey '{2}'....", IP, port, NetKey);
            this.IP = IP;
            this.port = port;
            this.NetKey = NetKey;
            return handler.Connect(IP, port, NetKey);
        }

        public void Close()
        {
            if (!closed)
            {
                closed = true;
                handler.Stop();
            }
        }

        public void Update()
        {
            if (closed) { return; }
            if (!Started) { return; }

            if (posTimer.ElapsedMilliseconds * 0.001f > positionUpdateRate)
            {
                NetworkingUpdate?.Invoke();
                handler.PollEvents();
                posTimer.Restart();
            }
        }

    }
}
