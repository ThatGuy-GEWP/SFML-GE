using LiteNetLib;
using LiteNetLib.Utils;
using SFML_Game_Engine.System;
using System.Diagnostics;
using System.Net;
using System.Threading.Channels;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace SFML_Game_Engine.Networking
{
    public class NetworkingManager
    {
        public Project Project;

        public EventBasedNetListener listener = new EventBasedNetListener();

        public NetManager handler = null!;
        public NetPeer serverPeer = null!;

        string NetKey = "default";

        public string IP;
        public int port;

        public int maxPeers = 3;

        public int TicksPerSecond = 40;

        public List<NetPeer> peers = new List<NetPeer>();

        public List<GameObject> syncedObjects = new List<GameObject>();

        readonly Stopwatch tickTimer = new Stopwatch();
        readonly Stopwatch connectTimer = Stopwatch.StartNew();

        public bool isClient = false;

        /// <summary>
        /// -1 is the server, any other number is a client.
        /// </summary>
        public int MyID = -1;

        public event Action<NetPeer, string> OnStringRecieve = null!;
        public event Action NetworkingUpdate = null!;
        public event Action PeerListChanged = null!;

        /// <summary>
        /// Called when connected to a server (if <see cref="isClient"/> <c>== true</c>)
        /// </summary>
        public event Action OnConnected = null!;

        /// <summary>
        /// Called when disconnected to a server (if <see cref="isClient"/> <c>== true</c>)
        /// </summary>
        public event Action OnDisconnected = null!;

        public bool Started { get; private set; } = false;
        public bool Connected { get; private set; } = false;
        bool closed = false;

        public NetworkingManager(Project Project, string IP, int port) // for my sanity, this will be client side code only!
        {
            this.Project = Project;
            this.IP = IP;
            this.port = port;
        }

        public void Start(bool asHost, string ip = "nil", int port = -1)
        {
            if(ip != "nil") { this.IP = ip; }
            if(port != -1) { this.port = port; }

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
            tickTimer.Start();
            Console.WriteLine("Networking manager setup finished.");
        }

        public void Subscribe()
        {
            Console.WriteLine("Did and done");
        }

        //replicateUpdate,sceneName,prefabName,gameObjectNewName,pos.x:pos.y
        /// <summary>
        /// Creates a prefab on the server and replicates it to every client connected.
        /// </summary>
        public GameObject? InstanceSharedPrefab(string prefabName, string gameObjectName)
        {
            string objFin = "replicateUpdate," + Project.ActiveScene!.Name + "," + prefabName + "," + gameObjectName + ",0:0";
            if (isClient)
            {
                GameObject inst = ReplicateObject(objFin)!;
                TakeOwnership(inst);
                SendToAll(objFin);
                return inst;
            }
            else
            {
                GameObject inst = ReplicateObject(objFin)!;
                TakeOwnership(inst);
                SendToAll(objFin);
                return inst;
            }
        }

        public GameObject? InstanceSharedPrefab(string prefabName, string gameObjectName, Vector2 atPos)
        {
            string objFin = "replicateUpdate," + Project.ActiveScene!.Name + "," + prefabName + "," + gameObjectName + "," + atPos.x + ":" + atPos.y;
            if (isClient)
            {
                GameObject inst = ReplicateObject(objFin)!;
                TakeOwnership(inst);
                SendToAll(objFin);
                return inst;
            }
            else
            {
                GameObject inst = ReplicateObject(objFin)!;
                TakeOwnership(inst);
                SendToAll(objFin);
                return inst;
            }
        }

        void TakeOwnership(GameObject of)
        {
            Component[] comps = of.GetComponentsOfDecendents().ToArray();
            foreach (Component comp in comps)
            {
                if (comp.GetType().IsAssignableTo(typeof(NetworkComponent)))
                {
                    Console.WriteLine("Contains networkComps");
                    NetworkComponent netComp = (NetworkComponent)comp;
                    netComp.SetOwnerID(MyID);
                    if (!netComp.gameObject.started || !netComp.Started)
                    {
                        netComp.OnStarted += (ncomp) =>
                        {
                            ncomp.TakeOwnership();
                        };
                    }
                }
            }
        }

        public bool CanGetID()
        {
            if (isClient)
            {
                if (Connected && Started)
                {
                    return true;
                }
            }
            else
            {
                if (Started)
                {
                    return true;
                }
            }
            return false;
        }

        void ClientReceive(NetPeer fromPeer, NetPacketReader dataReader, byte deliveryMethod, DeliveryMethod chanel)
        {
            bool isString = dataReader.TryGetString(out string result);

            if (isString)
            {
                if (result.StartsWith("conAccept"))
                {
                    Console.WriteLine("Connected to server!");
                    MyID = fromPeer.RemoteId;
                    serverPeer = fromPeer;
                    Connected = true;
                    OnConnected?.Invoke();
                }
                if (result.StartsWith("peerUpdate"))
                {
                    PeerListChanged?.Invoke();
                    return;
                }
                if (result.StartsWith("replicateUpdate"))
                {
                    ReplicateObject(result);
                }
                if (result.StartsWith("echo"))
                {
                    return;
                }
                OnStringRecieve?.Invoke(fromPeer, result);
            }

            dataReader.Recycle();
        }

        // replicateUpdate,sceneName,prefabName,gameObjectNewName,pos.x:pos.y
        GameObject? ReplicateObject(string dat)
        {
            string[] splitData = dat.Split(',');

            if(Project.ActiveScene!.Name == splitData[1])
            {
                Scene scn = Project.ActiveScene!;
                GameObject prefabInst = scn.InstancePrefab(Project.GetResource<Prefab>(splitData[2]));
                prefabInst.name = splitData[3];
                string[] vec = splitData[4].Split(':');
                Vector2 newPos = new Vector2(float.Parse(vec[0]), float.Parse(vec[1]));
                prefabInst.transform.LocalPosition = newPos;
                return prefabInst;
            }
            return null;
        }

        void PeerJoinedOrLeft()
        {
            NetDataWriter updateWriter = new NetDataWriter();

            updateWriter.Put("peerUpdate");

            foreach (NetPeer peer in peers)
            {
                peer.Send(updateWriter, DeliveryMethod.ReliableOrdered);
            }

            PeerListChanged?.Invoke();
        }

        void StartAsClient()
        {
            handler = new NetManager(listener);
            handler.Start();

            listener.NetworkReceiveEvent += ClientReceive;

            listener.PeerDisconnectedEvent += (peer, info) =>
            {
                Console.WriteLine("Peer disconnected");
                Connected = false;
            };

            Started = true;


            NetDataWriter testWriter = new NetDataWriter();
            testWriter.Put("ping");
        }

        void StartAsServer()
        {
            handler = new NetManager(listener);
            handler.Start(port);

            listener.ConnectionRequestEvent += request =>
            {
                if(handler.ConnectedPeersCount < maxPeers)
                {
                    request.AcceptIfKey(NetKey);
                } else
                {
                    Console.WriteLine("Peer tried to connect but we hit the max peers ({0}).", maxPeers);
                    request.Reject();
                }
            };

            listener.PeerConnectedEvent += peer =>
            {
                peers.Add(peer);
                Console.WriteLine("Peer {0} connected", peer.Address);
                Console.WriteLine(peer.Id);
                NetDataWriter writer = new NetDataWriter();
                writer.Put("conAccept");
                peer.Send(writer, DeliveryMethod.ReliableOrdered);

                PeerJoinedOrLeft();
            };

            listener.PeerDisconnectedEvent += (peer, discInfo) =>
            {
                Console.WriteLine(discInfo.Reason);
                peers.Remove(peer);

                PeerJoinedOrLeft();
            };

            listener.NetworkReceiveUnconnectedEvent += (a, b, c) =>
            {
                Console.WriteLine("Got unconnected!");
                try
                {
                    string str = b.GetString(100);
                    Console.WriteLine(str);
                } catch
                {
                    Console.WriteLine("Got unknown unconnected packet");
                }

            };

            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
            {
                bool isString = dataReader.TryGetString(out string result);
                if (isString)
                {
                    if (result.StartsWith("echo"))
                    {
                        string toEcho = result.Remove(0, 4);
                        if (toEcho.StartsWith("replicateUpdate"))
                        {
                            SendToClients(toEcho, fromPeer);
                        }
                        else
                        {
                            SendToClients(toEcho);
                        }
                        OnStringRecieve?.Invoke(fromPeer, toEcho);
                        return;
                    }
                    OnStringRecieve?.Invoke(fromPeer, result);
                }

                dataReader.Recycle();
            };

            OnStringRecieve += (fromPeer, result) =>
            {
                if (result.StartsWith("replicateUpdate"))
                {
                    ReplicateObject(result);
                }
            };

            Started = true;
        }
        
        /// <summary>
        /// Sends a message to every peer connected except this one.
        /// </summary>
        /// <param name="message"></param>
        public void SendToAll(string message, DeliveryMethod method = DeliveryMethod.ReliableOrdered)
        {
            if(isClient && !Connected) { return; }
            if (!Started) { return; }

            if (isClient)
            {
                //Console.WriteLine("telling server {0}", message);
                SendToSever("echo"+message, method);
            } else
            {
                SendToClients(message, method);
            }
        }

        /// <summary>
        /// Sends a message to the connected server
        /// </summary>
        /// <param name="message"></param>
        public void SendToSever(string message, DeliveryMethod method = DeliveryMethod.ReliableOrdered)
        {
            if (Started && Connected)
            {
                NetDataWriter writer = new NetDataWriter();
                writer.Put(message);
                serverPeer.Send(writer, method);
            }
        }

        /// <summary>
        /// Sends a message to all clients of this server.
        /// </summary>
        /// <param name="message"></param>
        public void SendToClients(string message, DeliveryMethod method = DeliveryMethod.ReliableOrdered)
        {
            if (Started)
            {
                NetDataWriter writer = new NetDataWriter();
                writer.Put(message);
                foreach(NetPeer peer in peers)
                {
                    peer.Send(writer, method);
                }
            }
        }

        public void SendToClients(string message, NetPeer excluded, DeliveryMethod method = DeliveryMethod.ReliableOrdered)
        {
            if (Started)
            {
                NetDataWriter writer = new NetDataWriter();
                writer.Put(message);
                foreach (NetPeer peer in peers)
                {
                    if(peer == excluded) { continue; }
                    peer.Send(writer, method);
                }
            }
        }

        public void TryServerConnect(string IP, int port, string NetKey)
        {
            Console.WriteLine("Attemping to connect to {0}:{1} with NetKey '{2}'", IP, port, NetKey);
            this.IP = IP;
            this.port = port;
            this.NetKey = NetKey;
            handler.Connect(IP, port, NetKey);
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

            if (!Connected && isClient && connectTimer.ElapsedMilliseconds * 0.001f > 0.5f)
            {
                TryServerConnect(IP, port, NetKey);
                connectTimer.Restart();
                return;
            }

            if (tickTimer.ElapsedMilliseconds * 0.001f > (1f / TicksPerSecond))
            {
                NetworkingUpdate?.Invoke();
                handler.PollEvents();
                tickTimer.Restart();
            }
        }

    }
}
