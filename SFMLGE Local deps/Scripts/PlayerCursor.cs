using SFML.Window;
using SFML_Game_Engine.Components;
using SFML_Game_Engine.Resources;
using SFML_Game_Engine.System;

namespace SFML_Game_Engine.Scripts
{
    public class PlayerCursor : NetworkComponent
    {
        public static bool holdingSomething = false;
        public static bool interactingWithSomething = false;

        public int ownerID = -2;

        public bool useGrabCursor = false;

        bool owned = false;

        string imgName = "";

        public PlayerCursor(string imgName, int ownerID){ this.imgName = imgName; this.ownerID = ownerID; }


        protected override void OnSyncUpdate(string data)
        {
            bool grabbin = bool.Parse(data);
            CursorImgUpdate(grabbin);
        }

        protected override string SyncToServer()
        {
            return useGrabCursor.ToString();
        }

        void CursorImgUpdate(bool grabState)
        {
            if(grabState != useGrabCursor)
            {
                useGrabCursor = grabState;

                if (useGrabCursor)
                {
                    cursorSprite.Texture = Project.GetResource<TextureResource>(imgName.Replace("cursor", "grab"));
                    cursorSprite.anchor = new Vector2(0.5f, 0.5f);
                }
                else
                {
                    cursorSprite.Texture = Project.GetResource<TextureResource>(imgName);
                    cursorSprite.anchor = Vector2.zero;
                }

            }
        }

        Sprite2D cursorSprite = null!;

        public override void Start()
        {
            base.Start();
            NetworkedTransform networkedPosition = new NetworkedTransform();
            gameObject.AddComponent(networkedPosition);

            gameObject.ZOrder = 500;

            cursorSprite = new Sprite2D(Project.GetResource<TextureResource>(imgName));
            cursorSprite.QueueType = SFMLGE_Local_deps.Engine.System.RenderQueueType.OverlayQueue;
            cursorSprite.anchor = Vector2.zero;

            gameObject.AddComponent(cursorSprite);
        }

        public override void Update()
        {
            if (!Project.networkingManager.CanGetID()) { return; }

            if (Project.networkingManager.Started)
            {
                if(Project.networkingManager.MyID == ownerID && !owned)
                {
                    owned = true;
                    gameObject.GetComponent<NetworkedTransform>()!.TakeOwnership();
                    TakeOwnership();
                    Console.WriteLine("Taking control of player {0}!", ownerID);
                }
            }

            if (owned)
            {
                gameObject.transform.WorldPosition = Scene.GetMouseWorldPosition();

                if (Project.IsMouseButtonHeld(0) && holdingSomething)
                {
                    CursorImgUpdate(true);
                }
                else
                {
                    CursorImgUpdate(false);
                }
            }
        }
    }
}
