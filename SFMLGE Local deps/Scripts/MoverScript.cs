using SFML.Graphics;
using SFML_Game_Engine.GUI;
using SFML_Game_Engine.Resources;
using SFML_Game_Engine.System;

namespace SFML_Game_Engine.Scripts
{
    internal class MoverScript : Component
    {
        NetworkedTransform managedNetworkComp;
        bool dragging = false;
        Vector2 size;

        public MoverScript(NetworkedTransform managedNetworkComp, Vector2 size)
        {
            this.managedNetworkComp = managedNetworkComp;
            this.size = size;
        }

        public override void Start()
        {
            base.Start();
            managedNetworkComp.syncRoatation = false;
        }

        public override void Update()
        {
            BoundBox box = new BoundBox(new FloatRect(gameObject.transform.WorldPosition - size/2f, size));
            box = box.Rotate(gameObject.transform.WorldPosition, gameObject.transform.rotation);

            Vector2 mousePos = Scene.GetMouseWorldPosition();

            if(box.WithinBoundsAccurate(mousePos))
            {
                if(Project.IsMouseButtonPressed(0) && !PlayerCursor.holdingSomething)
                {
                    dragging = true;
                    PlayerCursor.holdingSomething = true;
                    Scene.AudioManager.PlaySound(Project.GetResource<SoundResource>("pickup"), 25);
                }
            }

            if (dragging)
            {
                if (!Project.IsMouseButtonHeld(0)) { dragging = false; Scene.AudioManager.PlaySound(Project.GetResource<SoundResource>("drop"), 25); PlayerCursor.holdingSomething = false; return; }
                managedNetworkComp.TakeOwnership();
                gameObject.transform.WorldPosition = Vector2.Lerp(gameObject.transform.WorldPosition, mousePos, 15f * DeltaTime);
                Vector2 dirVec = mousePos - gameObject.transform.LocalPosition;

                if(MathF.Abs(dirVec.x) > 5f)
                {
                    if (dirVec.x < 0)
                    {
                        gameObject.transform.rotation = -35f * (MathF.Abs(dirVec.x) * 0.01f);
                    }
                    else
                    {
                        gameObject.transform.rotation = 35f * (MathF.Abs(dirVec.x) * 0.01f);
                    }
                }
            }

            gameObject.transform.rotation = MathGE.Lerp(gameObject.transform.rotation, 0f, MathGE.Clamp(15f * DeltaTime, 0.0f, 1.0f));
        }
    }
}
