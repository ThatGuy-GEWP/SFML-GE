using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.GUI
{
    public class GUIContext : Component, IRenderable
    {
        public sbyte ZOrder { get; set; } = sbyte.MaxValue;
        public bool Visible { get; set; } = true;
        public bool AutoQueue { get; set; } = true;
        public RenderQueueType QueueType { get; set; } = RenderQueueType.OverlayQueue;

        public RenderTexture guiTexture;

        List<GUIComponent> components = new List<GUIComponent>();


        public GUIContext(Vector2 size)
        {
            guiTexture = new RenderTexture((uint)size.x, (uint)size.y);
        }

        public GUIContext(uint sizeX, uint sizeY)
        {
            guiTexture = new RenderTexture(sizeX, sizeY);
        }


        public T AddComponent<T>(T component) where T : GUIComponent
        {
            component.context = this;
            components.Add(component);
            component.OnAdd();

            components.Sort((x, y) => { return x.transform.zOrder - y.transform.zOrder; });

            return component;
        }


        public override void Update()
        {
            if (!AutoQueue) { return; }
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Update();
            }
        }


        public void Hide()
        {
            Visible = false;
            AutoQueue = false;
        }

        public void Show()
        {
            Visible = true;
            AutoQueue = true;
        }

        Sprite renderSprite = new Sprite();

        public void OnRender(RenderTarget rt)
        {
            if (components.Count > 0)
            {
                for (int i = 0; i < components.Count; i++)
                {
                    if (!components[i].visible) { continue; }
                    components[i].OnRender(guiTexture);
                }
            }
            guiTexture.Display();
            renderSprite.Texture = guiTexture.Texture;
            renderSprite.Color = new Color(255, 255, 255, 255);
            rt.Draw(renderSprite);

            guiTexture.Clear(Color.Transparent);
        }
    }
}
