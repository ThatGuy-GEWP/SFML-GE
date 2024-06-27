using SFML.Graphics;
using SFML_Game_Engine.Components;
using SFML_Game_Engine.GUI;
using SFML_Game_Engine.Resources;
using SFML_Game_Engine.System;
using SFMLGE_Local_deps.Engine.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.Scripts
{
    internal class CardScript : NetworkComponent
    {
        static bool interactedWithCard = false;

        Sprite2D cardSprite = null!;
        TextureResource cardText = null!;
        TextureResource cardBack = null!;
        Vector2 size = Vector2.zero;

        bool flipped = false;

        float animationTime = 0.1f;

        float animationTimer = 0f;
        float cooldown = 0.15f;

        public override void Start()
        {
            base.Start();
            cardBack = Project.GetResource<TextureResource>("cardback");
            cardSprite = gameObject.GetComponent<Sprite2D>()!;
            cardText = cardSprite.Texture!;
            size = cardSprite.size;
            deliveryMethod = LiteNetLib.DeliveryMethod.ReliableSequenced;
        }

        bool swappedImg = false;

        protected override string SyncToServer()
        {
            doUpdate = false;
            return flipped.ToString();
        }

        protected override void OnSyncUpdate(string data)
        {
            bool targFlip = bool.Parse(data);
            if (targFlip != flipped)
            {
                flipped = targFlip;
                animationTimer = animationTime;
            }
        }

        bool changedInteractionState = false;

        public override void Update()
        {
            if(animationTimer > 0f)
            {
                float t = 1f - MathGE.Map(animationTimer, 0.0f, animationTime, 0.0f, 1.0f);

                if (changedInteractionState && interactedWithCard)
                {
                    interactedWithCard = false;
                    changedInteractionState = false;
                }

                if (t < 0.5f)
                {
                    cardSprite.size = Vector2.Lerp(cardSprite.size, new Vector2(0, size.y), (t / 2f));
                    swappedImg = false;
                } 
                else
                {
                    if (!swappedImg)
                    {
                        swappedImg = true;
                        cardSprite.size = new Vector2(0, 0);
                        if (flipped)
                        {
                            cardSprite.Texture = cardBack;
                        } else
                        {
                            cardSprite.Texture = cardText;
                        }
                    }
                    cardSprite.size = Vector2.Lerp(new Vector2(0, size.y), size, (t - 0.5f) * 2f);
                }

            } else { cardSprite.size = size; }

            if(animationTimer > 0f) { animationTimer -= DeltaTime; return; }

            if(cooldown > 0f) { cooldown -= DeltaTime; return; }

            BoundBox box = new BoundBox(new FloatRect(gameObject.transform.LocalPosition - (size / 2f), size));
            box = box.Rotate(gameObject.transform.LocalPosition, gameObject.transform.rotation);

            Vector2 mousePos = Scene.GetMouseWorldPosition();

            if(box.WithinBoundsAccurate(mousePos))
            {
                if (Project.IsMouseButtonPressed(SFML.Window.Mouse.Button.Right) && !PlayerCursor.holdingSomething && !interactedWithCard)
                {
                    interactedWithCard = true;
                    changedInteractionState = true;
                    Console.WriteLine("Flip!");
                    TakeOwnership();
                    flipped = !flipped;
                    doUpdate = true;
                    animationTimer = animationTime;
                }
            }
        }

    }
}
