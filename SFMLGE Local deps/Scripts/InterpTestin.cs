using SFML.Graphics;
using SFML_Game_Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFMLGE_Local_deps.Scripts
{
    internal class InterpTestin : Component, IRenderable
    {
        public sbyte ZOrder { get; set; } = 5;
        public bool Visible { get; set; } = true;
        public bool AutoQueue { get; set; } = true;
        public RenderQueueType QueueType { get; set; } = RenderQueueType.DefaultQueue;

        float ypos = 0;
        int mode = 0;

        public InterpTestin(float yPos, int mode)
        {
            ypos = yPos;
            this.mode = mode;
        }

        float time = 0;

        float from = 100;
        float to = 300;

        float cur = 100;

        public override void Start()
        {
            gameObject.Position = new Vector2(100, ypos);
        }

        public override void Update()
        {
            time += deltaTime;
            gameObject.Position = new Vector2(cur, ypos);
            if(time >= 1.5f) { time = 0; }
            
            switch (mode)
            {
                case 0:
                    cur = MathGE.Lerp(from, to, time > 1.0f ? 1.0f : time);
                    return;
                case 1:
                    cur = MathGE.Interpolation.ElasticOut(from, to, time > 1.0f ? 1.0f : time);
                    return;
                case 2:
                    cur = MathGE.Interpolation.SmoothStep(from, to, time > 1.0f ? 1.0f : time);
                    return;
                case 3:
                    cur = MathGE.Interpolation.QuadraticEaseOut(from, to, time > 1.0f ? 1.0f : time);
                    return;
                case 4:
                    cur = MathGE.Interpolation.QuadraticEaseIn(from, to, time > 1.0f ? 1.0f : time);
                    return;
            }
        }

        CircleShape CircleShape = new CircleShape(5f, 32);
        public void OnRender(RenderTarget rt)
        {
            CircleShape.Position = gameObject.Position;
            rt.Draw(CircleShape);
        }
    }
}
