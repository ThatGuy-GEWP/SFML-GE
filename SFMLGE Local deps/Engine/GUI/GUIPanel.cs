using SFML.Audio;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.GUI
{
    public class GUIPanel : GUIComponent
    {
        public Color backgroundColor = GUIComponent.defaultBackground;
        public Color outlineColor = GUIComponent.defaultSecondary;

        public float outlineThickness = 5.0f;

        public bool indentedCorners = false;

        RectangleShape panelRect = new RectangleShape();

        RectangleShape outlineRect = new RectangleShape();

        CircleShape cornerCircle = new CircleShape(32, 32);

        public GUIPanel()
        {
            transform.size = new Vector2(150, 50);
        }

        public GUIPanel(Vector2 size)
        {
            transform.size = size;
        }
        public override void Update()
        {
            if (!visible) return;

            panelRect.Position = transform.WorldPosition;
            panelRect.Size = transform.size;

            panelRect.FillColor = backgroundColor;
            cornerCircle.FillColor = outlineColor;
            outlineRect.FillColor = outlineColor;

            cornerCircle.Radius = outlineThickness;
            cornerCircle.Origin = new Vector2(outlineThickness, outlineThickness);
        }

        void DrawCorners(RenderTarget rt)
        {
            FloatRect bounds = panelRect.GetGlobalBounds();

            cornerCircle.Position = new Vector2(bounds.Left, bounds.Top);
            rt.Draw(cornerCircle);
            cornerCircle.Position = new Vector2(bounds.Left + bounds.Width, bounds.Top + 1);
            rt.Draw(cornerCircle);
            cornerCircle.Position = new Vector2(bounds.Left, bounds.Top + bounds.Height);
            rt.Draw(cornerCircle);
            cornerCircle.Position = new Vector2(bounds.Left + bounds.Width, bounds.Top + bounds.Height);
            rt.Draw(cornerCircle);
        }
        public override void OnRender(RenderTarget rt)
        {
            if (!indentedCorners)
            {
                DrawCorners(rt);
            }

            outlineRect.Size = (Vector2)panelRect.Size + new Vector2(0, outlineThickness);
            outlineRect.Position = transform.WorldPosition;
            rt.Draw(outlineRect);

            outlineRect.Position = transform.WorldPosition - new Vector2(0, outlineThickness);
            rt.Draw(outlineRect);

            outlineRect.Size = (Vector2)panelRect.Size + new Vector2(outlineThickness, 0);
            outlineRect.Position = transform.WorldPosition;
            rt.Draw(outlineRect);

            outlineRect.Position = transform.WorldPosition - new Vector2(outlineThickness, 0);
            rt.Draw(outlineRect);

            rt.Draw(panelRect);

            if (indentedCorners)
            {
                DrawCorners(rt);
            }
        }


    }
}
