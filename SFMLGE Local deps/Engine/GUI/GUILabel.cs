using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.GUI
{
    /// <summary>
    /// A Simple text label.
    /// </summary>
    public class GUILabel : GUIPanel 
    {
        public uint charSize = 18;
        public string displayedString;
        public Color fontColor = Color.White;
        public FontResource font = null!;

        /// <summary>
        /// Hides overflow when true. might be a tiny bit costly
        /// </summary>
        public bool hideOverflow = true;

        public Vector2 textAnchor = new Vector2(0.5f, 0.5f);

        public UDim2 textPosition = new UDim2(0.5f, 0.5f, 0, 0);

        Text text = new Text();

        RenderTexture internalText = null!;

        Vector2 lastSize = new Vector2(0, 0);

        public GUILabel() : base() { displayedString = string.Empty; }

        public GUILabel(string displayedString, uint characterSize = 18) : base()
        {
            this.displayedString = displayedString;
            charSize = characterSize;
        }

        public override void Start()
        {
            base.Start();

            // intelisense taught me this, i bow before its benevolence
            font ??= Project.GetResource<FontResource>(defaultFontName);

            lastSize = GetSize();
            internalText ??= new RenderTexture((uint)lastSize.x, (uint)lastSize.y);

            GUIPanel pan = new GUIPanel();
        }

        public BoundBox GetTextBounds()
        {
            FloatRect a = text.GetGlobalBounds();
            FloatRect b = text.GetLocalBounds();
            return new BoundBox(new FloatRect(a.Left + b.Left, a.Top + b.Top, a.Width + b.Width, a.Height + b.Height));
        }

        Sprite drawSpr = new Sprite();

        protected override void PostPass(RenderTarget rt)
        {
            if (hideOverflow)
            {
                if (lastSize != GetSize())
                {
                    internalText.Dispose();
                    lastSize = GetSize();
                    internalText = new RenderTexture((uint)lastSize.x, (uint)lastSize.y);
                }
                internalText.Clear(Color.Transparent);
            }

            text.Font = font;
            text.CharacterSize = charSize;
            text.DisplayedString = displayedString;

            if (hideOverflow)
            {
                text.Position = (GetSize() * textPosition.scale) + textPosition.offset;
            } 
            else
            {
                text.Position = (GetPosition() + GetSize() * textPosition.scale) + textPosition.offset;
            }

            Vector2 centeredReg = new Vector2(text.GetGlobalBounds().Width, text.GetGlobalBounds().Height) * textAnchor;
            Vector2 centerOffset = new Vector2(text.GetLocalBounds().Left, text.GetLocalBounds().Top);

            //https://learnsfml.com/basics/graphics/how-to-center-text/#example-code
            // HOLY SHIT why is it not mentioned ANYWHERE in the offical docs that GetLocalBounds() is shifted to help align the baseline on text,
            // I TRIED SO MANY TIMES MONTHS AGO TO THE POINT WHERE I WAS REMAKING THE TEXT CLASS THEN JUST LIVED WITH IT WHYYYYYYY

            // Laurent i am rapidy approaching your location, and you should be scared.

            text.Origin = Vector2.Round(centeredReg + centerOffset);
            text.Position = Vector2.Round(text.Position);

            text.FillColor = fontColor;

            if (hideOverflow)
            {
                drawSpr.Position = GetPosition();
                internalText.Draw(text);
                drawSpr.TextureRect = new IntRect(0, 0, (int)lastSize.x, (int)lastSize.y);
                internalText.Display();
                drawSpr.Texture = internalText.Texture;

                rt.Draw(drawSpr);
            } else
            {
                rt.Draw(text);
            }
        }

    }
}
