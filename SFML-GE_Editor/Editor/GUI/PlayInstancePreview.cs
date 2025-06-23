using SFML.Graphics;
using SFML_GE.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_GE_Editor.Editor.GUI
{
    public class PlayInstancePreview : Component, IRenderable
    {
        public int ZOffset { get; set; } = 0;
        public bool Visible { get; set; } = true;
        public bool AutoQueue { get; set; } = true;
        public RenderQueueType QueueType { get; set; } = RenderQueueType.DefaultQueue;


        PlayInstance inst = null!;

        public PlayInstancePreview(PlayInstance instance)
        {
            inst = instance;
        }

        RenderTexture renderTexture;
        Sprite spr = new Sprite();
        RectangleShape rect = new RectangleShape();
        ScaleConstraint sc = new ScaleConstraint();

        public override void Start()
        {
            gameObject.AddComponent(sc);

            sc.objectSize = new Vector2(800, 600);

            sc.anchor = new Vector2(0.5f, 0f);
            sc.position_scale = new Vector2(0.5f, 0f);
            sc.offset = new Vector2(0, 30f);


            renderTexture = new RenderTexture(800, 600);
            spr = new Sprite();
            rect.Size = new Vector2(800, 600);
            rect.FillColor = Color.White;
            rect.OutlineThickness = 1;
            rect.OutlineColor = Color.White;
        }

        public void OnRender(RenderTarget rt)
        {
            if(spr == null || rect == null || renderTexture == null) return;

            renderTexture.Clear();
            inst.Render(renderTexture);
            renderTexture.Display();

            spr.Texture = renderTexture.Texture;

            rect.Position = gameObject.transform.GlobalPosition;
            rt.Draw(rect);

            spr.Position = gameObject.transform.GlobalPosition;
            rt.Draw(spr);
        }
    }
}
