using SFML.Audio;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.GUI
{
    /// <summary>
    /// A fancy panel, acts as a background base for other components, but can be used by itself.
    /// </summary>
    public class GUIPanel : GUIComponent
    {
        public Color backgroundColor = GUIComponent.defaultBackground;
        public Color outlineColor = GUIComponent.defaultSecondary;

        public float outlineThickness = 2.0f;

        public bool indentedCorners = false;

        public TextureResource panelContent = null!;

        RectangleShape panelRect = new RectangleShape();

        RectangleShape outlineRect = new RectangleShape();


        CircleShape cornerCircle = new CircleShape(32, 32);

        public GUIPanel(GUIContext context) : base(context)
        {
            transform.size = new Vector2(150, 50);
            transform.origin = new Vector2(0.5f, 0.5f);
        }

        public GUIPanel(GUIContext context, Vector2 size) : base(context)
        {
            transform.size = size;
        }

        public GUIPanel(GUIContext context, Vector2 size, Vector2 position) : base(context)
        {
            transform.size = size;
            transform.WorldPosition = position;
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
            cornerCircle.Position = new Vector2(transform.WorldPosition.x, transform.WorldPosition.y);
            rt.Draw(cornerCircle);
            cornerCircle.Position = new Vector2(transform.WorldPosition.x + transform.size.x, transform.WorldPosition.y);
            rt.Draw(cornerCircle);
            cornerCircle.Position = new Vector2(transform.WorldPosition.x, transform.WorldPosition.y + transform.size.y);
            rt.Draw(cornerCircle);
            cornerCircle.Position = new Vector2(transform.WorldPosition.x + transform.size.x, transform.WorldPosition.y + transform.size.y);
            rt.Draw(cornerCircle);
        }
        public override void OnRender(RenderTarget rt)
        {
            if (!indentedCorners)
            {
                DrawCorners(rt);
            }

            if(panelContent == null)
            {
                panelContent = context.project.GetResource<TextureResource>("DefaultSprite");
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

            panelRect.Texture = panelContent.Resource;
            //panelRect.TextureRect = new IntRect(0, (int)(transform.size.y / 2f), (int)panelContent.Resource.Size.X, (int)((int)panelContent.Resource.Size.Y - transform.size.y));

            rt.Draw(panelRect);

            if (indentedCorners)
            {
                DrawCorners(rt);
            }
        }


    }
}
