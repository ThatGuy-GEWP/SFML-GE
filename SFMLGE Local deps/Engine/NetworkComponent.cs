using LiteNetLib;
using SFML_Game_Engine.System;

namespace SFML_Game_Engine
{
    /// <summary>
    /// A Component that acts as a hook into the networking layer, meant to be used with other components.
    /// </summary>
    public class NetworkComponent : Component
    {
        /// <summary>
        /// True if this client owns the NetworkingComponent
        /// </summary>
        public bool Owned { get { return ownerID == manager.MyID; } }

        int ownerID = -1; // -1 is the server, anything else isnt!

        /// <summary>
        /// If True, this networkComponent will remind other clients every other tick if this client owns it.
        /// </summary>
        public bool ownerEnforcment = true;

        public DeliveryMethod deliveryMethod = DeliveryMethod.Unreliable;

        bool enforcmentSwitch = false;

        /// <summary>
        /// The Channel ID this networkComponent operates on.
        /// Every network component signs its packets with its channelID and will only recieve packets with said channelID
        /// </summary>
        protected int channelID = 0;

        /// <summary>
        /// The Manager this NetworkingManager is attached too.
        /// </summary>
        protected NetworkingManager manager = null!;

        /// <summary>
        /// If False, this NetworkComponent wont call SyncToServer() while owned.
        /// </summary>
        public bool doUpdate = true;

        string SyncUpdateBase()
        {
            //$"syncEvent,{Scene.Name},{gameObject.name},{gameObject.transform.WorldPosition.x}:{gameObject.transform.WorldPosition.y}"
            return $"syncEvent,{Scene.Name},{gameObject.name},{channelID},";
        }

        string OwnershipUpdate()
        {
            return $"ownerChange,{Scene.Name},{gameObject.name},{manager.MyID}";
        }

        string DestroyUpdate()
        {
            return $"destroyTarget,{Scene.Name},{gameObject.name}";
        }

        public void TakeOwnership(DeliveryMethod method = DeliveryMethod.ReliableOrdered)
        {
            manager.SendToAll(OwnershipUpdate(), method);
            ownerID = manager.MyID;
        }

        public void SyncedDestroy()
        {
            if (Owned)
            {
                manager.SendToAll(DestroyUpdate());
            }
        }

        public void SetOwnerID(int to)
        {
            ownerID = to;
        }

        /// <summary>
        /// Called whenever the server sends a syncEvent packet addressed to this gameObject.
        /// </summary>
        protected virtual void OnSyncUpdate(string data)
        {
            return;
        }

        /// <summary>
        /// Called when the gameObject is owned by us, and needs to update data to the server.
        /// </summary>
        /// <returns></returns>
        protected virtual string SyncToServer()
        {
            return "";
        }

        protected virtual void OnOwnershipChanged(int from, int to)
        {
            return;
        }

        public Action<NetworkComponent> OnStarted = null!;

        public override void Start()
        {
            manager = Project.networkingManager;
            Project.useNetworking = true;
            manager.OnStringRecieve += (FromPeer, Str) =>
            {
                if (!manager.CanGetID()) { return; }
                string[] dat = Str.Split(',');
                if(dat.Length <= 1) { return; }
                string toRemove = string.Empty;
                if (dat[0] == "syncEvent")
                {
                    toRemove += dat[0] + ",";
                    if (dat[1] == Scene.Name)
                    {
                        toRemove += dat[1] + ",";
                        if (dat[2] == gameObject.name)
                        {
                            toRemove += dat[2] + ",";
                            if (int.Parse(dat[3]) == channelID)
                            {
                                toRemove += dat[3] + ",";
                                OnSyncUpdate(Str.Replace(toRemove, string.Empty));
                            }
                        }
                    }
                }
                if (dat[0] == "ownerChange")
                {
                    if (dat[1] == Scene.Name)
                    {
                        if (dat[2] == gameObject.name)
                        {
                            int oldOwner = ownerID;
                            ownerID = int.Parse(dat[3]);
                            OnOwnershipChanged(oldOwner, int.Parse(dat[3]));
                        }
                    }
                }
                if (dat[0] == "destroyTarget")
                {
                    if (dat[1] == Scene.Name)
                    {
                        if (dat[2] == gameObject.name)
                        {
                            if (!Owned)
                            {
                                gameObject.Destroy();
                                doUpdate = false;
                                Enabled = false;
                                return;
                            }
                        }
                    }
                }
            };

            manager.NetworkingUpdate += () => {
                if (!manager.CanGetID()) { return; }
                if (Owned && doUpdate)
                {
                    manager.SendToAll(SyncUpdateBase() + SyncToServer(), deliveryMethod);
                }
            };

            OnStarted?.Invoke(this);
        }
    }
}
