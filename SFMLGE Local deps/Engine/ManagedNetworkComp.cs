using SFML_Game_Engine.System;

namespace SFML_Game_Engine
{
    /// <summary>
    /// A Component that syncs the attached gameObjects position with other clients.
    /// </summary>
    public class ManagedNetworkComp : Component
    {
        public Vector2 targetPosition = Vector2.zero;
        public bool Owned { get; set; } = false;

        public NetworkingManager Manager { get; private set; }

        public ManagedNetworkComp(NetworkingManager manager) 
        { 
            this.Manager = manager;
        }

        string positionUpdate()
        {
            // syncEvent,SceneName,GameObjectName,WorldPosition

            return $"syncEvent,{Scene.Name},{gameObject.name},{gameObject.transform.WorldPosition.x}:{gameObject.transform.WorldPosition.y}";
        }

        public override void Start()
        {
            Manager.OnStringRecieve += (FromPeer, Str) =>
            {
                if (Owned) { return; }
                string[] dat = Str.Split(',');
                if(dat.Length <= 1) { return; }
                if (dat[0] == "syncEvent")
                {
                    if (dat[1] == Scene.Name)
                    {
                        if (dat[2] == gameObject.name)
                        {
                            string[] pos = dat[3].Split(':');
                            targetPosition = new Vector2(float.Parse(pos[0]), float.Parse(pos[1]));
                            Console.WriteLine("Updated target pos");
                        }
                    }
                }
            };

            Manager.NetworkingUpdate += () => {
                if (Owned)
                {
                    Manager.EchoToAll(positionUpdate());
                }
            };
        }

        public override void Update()
        {
            if (!Owned)
            {
                gameObject.transform.WorldPosition = Vector2.Lerp(gameObject.transform.WorldPosition, targetPosition, MathGE.Clamp(DeltaTime * 5, 0, 1));
            }
        }
    }
}
