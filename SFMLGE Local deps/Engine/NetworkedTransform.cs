using SFML.Graphics;
using SFML_Game_Engine.System;
using SFMLGE_Local_deps.Engine.System;
using System.Diagnostics;

namespace SFML_Game_Engine
{
    /// <summary>
    /// Syncs the <see cref="System.Transform"/> of the gameObject its attached too with the server and other clients.
    /// </summary>
    public class NetworkedTransform : NetworkComponent
    {
        public bool SmoothMovment = false;

        public bool syncRoatation = true;
        public bool syncPosition = true;

        public Vector2 targetPos = Vector2.zero;
        public float targetRot = 0f;

        Vector2 lastPos = Vector2.zero;
        float lastRot = 0f;

        Stopwatch staticTimer = new Stopwatch();

        protected override string SyncToServer()
        {
            return gameObject.transform.LocalPosition.x + ":" + gameObject.transform.LocalPosition.y + "," + gameObject.transform.rotation;
        }

        protected override void OnSyncUpdate(string data)
        {
            string[] transformData = data.Split(',');
            string[] posDat = transformData[0].Split(":");
            targetPos = new Vector2(float.Parse(posDat[0]), float.Parse(posDat[1]));
            targetRot = float.Parse(transformData[1]);
        }

        protected override void OnOwnershipChanged(int from, int to)
        {
            // prevents jumping around when ownership is lost from server, or new client
            if(from == manager.MyID)
            {
                targetPos = gameObject.transform.LocalPosition;
                targetRot = gameObject.transform.rotation;
                doUpdate = true;
            }
        }

        public override void Start()
        {
            channelID = -404;
            base.Start();
            targetPos = gameObject.transform.LocalPosition;
            targetRot = gameObject.transform.rotation;
            manager.PeerListChanged += () =>
            {
                staticTimer.Restart();
                doUpdate = true;
            };
        }

        public override void Update()
        {
            if (Owned)
            {
                if(lastPos != gameObject.transform.LocalPosition || lastRot == gameObject.transform.rotation)
                {
                    staticTimer.Restart();
                    lastPos = gameObject.transform.LocalPosition;
                    lastRot = gameObject.transform.rotation;
                    doUpdate = true;
                }

                if (staticTimer.ElapsedMilliseconds * 0.01f > 0.5f)
                {
                    doUpdate = false;
                }
            }

            if (!Owned)
            {
                doUpdate = false;
                if (syncPosition)
                {
                    gameObject.transform.LocalPosition = Vector2.Lerp(
                        gameObject.transform.LocalPosition,
                        targetPos,
                        manager.TicksPerSecond * DeltaTime
                        );
                }
                if (syncRoatation)
                {
                    gameObject.transform.rotation = MathGE.Lerp(
                        gameObject.transform.rotation, 
                        targetRot, 
                        MathGE.Clamp(manager.TicksPerSecond * DeltaTime, 0.0f, 1.0f)
                        );
                }
            }
        }

        public override void OnDestroy(GameObject gameObject)
        {
            if(Owned)
            {
                SyncedDestroy();
            }
            base.OnDestroy(gameObject);
        }
    }
}
