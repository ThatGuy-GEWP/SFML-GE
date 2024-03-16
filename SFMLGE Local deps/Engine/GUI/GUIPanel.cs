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

        public float outlineThickness = 1.0f;

        public bool roundedCorners = false;

        public TextureResource panelContent = null!;

        static RectangleShape panelRect = new RectangleShape();
        static RectangleShape outlineRect = new RectangleShape();

        static Texture cornerText = null!;

        public GUIPanel(GUIContext context, bool roundedCorners = false) : base(context)
        {
            transform.size = new Vector2(150, 50);
            this.roundedCorners = roundedCorners;
        }

        public GUIPanel(GUIContext context, Vector2 size, bool roundedCorners = false) : base(context)
        {
            transform.size = size;
            this.roundedCorners = roundedCorners;
        }

        public GUIPanel(GUIContext context, Vector2 size, Vector2 position, bool roundedCorners = false) : base(context)
        {
            transform.size = size;
            transform.WorldPosition = position;
            this.roundedCorners = roundedCorners;
        }

        void generateCornerTexture()
        {
            RenderTexture rt = new RenderTexture(512, 512);
            CircleShape tempCircle = new CircleShape(512, 64);

            tempCircle.Position = new Vector2(512, 512);
            tempCircle.Origin = new Vector2(512, 512);

            rt.Draw(tempCircle);
            rt.Display();
            cornerText = new Texture(rt.Texture); // without this, rt.dispose would also take away the texture
            cornerText.Repeated = false;

            rt.Dispose();
            tempCircle.Dispose();
        }

        public override void Update()
        {
            if (!visible) return;
        }

        RectangleShape recShape = new RectangleShape();
        void DrawCorners(RenderTarget rt, float outlineThickness)
        {
            recShape.FillColor = outlineColor;
            recShape.Texture = cornerText;
            recShape.Size = new Vector2(outlineThickness, outlineThickness) * 2f;

            recShape.Position = new Vector2(transform.WorldPosition.x, transform.WorldPosition.y);
            recShape.Origin = new Vector2(outlineThickness, outlineThickness) * 2f;
            recShape.Rotation = 0f;
            rt.Draw(recShape);

            recShape.Position = new Vector2(transform.WorldPosition.x + transform.size.x, transform.WorldPosition.y);
            recShape.Origin = new Vector2(outlineThickness, outlineThickness) * 2f;
            recShape.Rotation = 90f;
            rt.Draw(recShape);

            recShape.Position = new Vector2(transform.WorldPosition.x, transform.WorldPosition.y + transform.size.y);
            recShape.Origin = new Vector2(outlineThickness, outlineThickness) * 2f;
            recShape.Rotation = -90f;
            rt.Draw(recShape);

            recShape.Position = new Vector2(transform.WorldPosition.x + transform.size.x, transform.WorldPosition.y + transform.size.y);
            recShape.Origin = new Vector2(outlineThickness, outlineThickness) * 2f;
            recShape.Rotation = 180f;
            rt.Draw(recShape);
        }

        /// <summary> Draws the lines actually connecting the rounded corners </summary>
        void DrawCornerConnectors(RenderTarget rt, float outlineThickness)
        {
            outlineRect.FillColor = outlineColor;


            outlineRect.Size = new Vector2(panelRect.Size.X, outlineThickness * 2f);
            outlineRect.Position = transform.WorldPosition - new Vector2(0, outlineThickness * 2f);
            rt.Draw(outlineRect);

            outlineRect.Size = new Vector2(panelRect.Size.X, (outlineThickness * 2f));
            outlineRect.Position = transform.WorldPosition + new Vector2(0, transform.size.y);
            rt.Draw(outlineRect);

            outlineRect.Size = new Vector2(outlineThickness * 2f, panelRect.Size.Y);
            outlineRect.Position = transform.WorldPosition - new Vector2(outlineThickness * 2f, 0);
            rt.Draw(outlineRect);

            outlineRect.Size = new Vector2(outlineThickness * 2f, panelRect.Size.Y);
            outlineRect.Position = transform.WorldPosition + new Vector2(transform.size.x - 0, 0);
            rt.Draw(outlineRect);
        }

        public override void OnRender(RenderTarget rt)
        {
            if (!context.Started) { return; }

            if (cornerText == null)
            {
                generateCornerTexture();
            }

            if (panelContent == null)
            {
                panelContent = context.project.GetResource<TextureResource>("DefaultSprite");
            }

            if (roundedCorners)
            {
                panelRect.OutlineThickness = 0;
                DrawCorners(rt, outlineThickness / 2f);
                DrawCornerConnectors(rt, outlineThickness / 2f);
            }
            else
            {
                panelRect.OutlineThickness = outlineThickness;
                panelRect.OutlineColor = outlineColor;
            }

            panelRect.Position = transform.WorldPosition;
            panelRect.Size = transform.size;

            panelRect.FillColor = backgroundColor;
            outlineRect.FillColor = outlineColor;

            panelRect.Texture = panelContent.Resource;
            //panelRect.TextureRect = new IntRect(0, (int)(transform.size.y / 2f), (int)panelContent.Resource.Size.X, (int)((int)panelContent.Resource.Size.Y - transform.size.y));

            rt.Draw(panelRect);
        }


    }
}
