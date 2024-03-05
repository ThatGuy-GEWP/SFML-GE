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

        float _outlineThickness = 2.0f;

        public float OutlineThickness
        {
            get { return _outlineThickness; }
            set { if (value != _outlineThickness) { GenerateCornerTexture(value); }  _outlineThickness = value; }
        }

        Sprite cornerSprite = new Sprite();

        RenderTexture cornerDrawBuff = new RenderTexture(64, 64);

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

        public void GenerateCornerTexture(float radius)
        {
            cornerCircle.Radius = radius;
            cornerCircle.Origin = new Vector2(radius, radius);
            cornerCircle.Position = new Vector2(64, 64);
            cornerCircle.FillColor = Color.White;

            cornerDrawBuff.Clear(Color.Transparent);
            cornerDrawBuff.Draw(cornerCircle);
            cornerDrawBuff.Display();
        }

        public override void OnAdd()
        {
            GenerateCornerTexture(OutlineThickness);
        }

        public override void Update()
        {
            if (!visible) return;

            panelRect.Position = transform.WorldPosition;
            panelRect.Size = transform.size;

            panelRect.FillColor = backgroundColor;
            cornerSprite.Color = outlineColor;
            outlineRect.FillColor = outlineColor;
        }

        public override void OnRender(RenderTarget rt)
        {
            FloatRect bounds = panelRect.GetGlobalBounds();

            cornerSprite.Texture = cornerDrawBuff.Texture;

            cornerSprite.Rotation = 0;
            cornerSprite.Position = new Vector2(bounds.Left, bounds.Top);
            cornerSprite.Origin = new Vector2(64, 64);
            rt.Draw(cornerSprite);

            cornerSprite.Rotation = 90;
            cornerSprite.Position = new Vector2(bounds.Left + bounds.Width, bounds.Top+1);
            cornerSprite.Origin = new Vector2(64, 64);
            rt.Draw(cornerSprite);

            cornerSprite.Rotation = 270;
            cornerSprite.Position = new Vector2(bounds.Left, bounds.Top + bounds.Height);
            cornerSprite.Origin = new Vector2(64, 64);
            rt.Draw(cornerSprite);

            cornerSprite.Rotation = 180;
            cornerSprite.Position = new Vector2(bounds.Left + bounds.Width, bounds.Top + bounds.Height);
            cornerSprite.Origin = new Vector2(64, 64);
            rt.Draw(cornerSprite);

            outlineRect.Size = (Vector2)panelRect.Size + new Vector2(0, OutlineThickness);
            outlineRect.Position = transform.WorldPosition;
            rt.Draw(outlineRect);

            outlineRect.Position = transform.WorldPosition - new Vector2(0, OutlineThickness);
            rt.Draw(outlineRect);

            outlineRect.Size = (Vector2)panelRect.Size + new Vector2(OutlineThickness, 0);
            outlineRect.Position = transform.WorldPosition;
            rt.Draw(outlineRect);

            outlineRect.Position = transform.WorldPosition - new Vector2(OutlineThickness, 0);
            rt.Draw(outlineRect);

            rt.Draw(panelRect);
        }


    }
}
