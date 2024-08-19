using SFML.Graphics;
using SFML_Game_Engine.Engine.System;
using SFML_Game_Engine.Resources;
using SFML_Game_Engine.System;

namespace SFML_Game_Engine.GUI
{
    /// <summary>
    /// A Simple text label.
    /// </summary>
    public class GUILabel : GUIPanel 
    {
        bool destroyed = false;

        /// <summary>The character size, not in pixels</summary>
        public uint charSize = 16;
        /// <summary>The string that will be displayed</summary>
        public string displayedString;

        /// <summary>The color of the text.</summary>
        public Color textFillColor = Color.White;

        /// <summary>When true all text will be bold, false can still contain bold text from <see cref="richEnabled"/>.</summary>
        public bool isBold = false;

        public FontResource font = null!;

        /// <summary>Hides overflow when true by rendering to an internal texture.</summary>
        public bool hideOverflow = true;

        public TextAlignment textAlignment = TextAlignment.Center;

        /// <summary>
        /// Controls where the center of the text is.
        /// </summary>
        public Vector2 textAnchor = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// Controls where in the label the text is.
        /// </summary>
        public UDim2 textPosition = new UDim2(0.5f, 0.5f, 0, 0);

        /// <summary>
        /// When false, rich text formatting is disabled
        /// </summary>
        public bool richEnabled = true;

        RenderTexture internalRenderTexture = null!; // used for hideOverflow

        Vector2 lastSize = new Vector2(0, 0); // used to update internalRenderTexture's size

        protected RichText text = null!;

        public GUILabel(string displayedString = "") : base() 
        { 
            this.displayedString = displayedString;  
        }

        public GUILabel(FontResource font, string displayedString, uint characterSize = 16) : base()
        {
            this.font = font;
            this.displayedString = displayedString;
            charSize = characterSize;

            text = new RichText(font.resource, displayedString, characterSize);
        }

        public override void Start()
        {
            base.Start();

            // intelisense taught me this, i bow before its benevolence
            font ??= Project.GetResource<FontResource>(defaultFontName);

            lastSize = GetSize();
            internalRenderTexture ??= new RenderTexture((uint)lastSize.x, (uint)lastSize.y);

            GUIPanel pan = new GUIPanel();
        }

        /// <summary>
        /// Gets the local bounds of the text.
        /// </summary>
        public BoundBox GetTextLocalBounds()
        {
            return text.GetLocalBounds();
        }

        /// <summary>
        /// Gets the global bounds of the text.
        /// </summary>
        public BoundBox GetTextGlobalBounds()
        {
            return text.GetGlobalBounds();
        }

        Sprite drawSpr = new Sprite();

        protected override void PostPass(RenderTarget rt)
        {
            if(destroyed) return;
            if(font == null) { return; }
            if(font != null && text == null) { text = new RichText(font.resource, displayedString, charSize); }

            if (hideOverflow)
            {
                if (lastSize != GetSize())
                {
                    internalRenderTexture.Dispose();
                    lastSize = GetSize();
                    internalRenderTexture = new RenderTexture((uint)lastSize.x, (uint)lastSize.y);
                }
                internalRenderTexture.Clear(Color.Transparent);
            }

            text.RichEnabled = richEnabled;
            text.Font = font!;
            text.CharacterSize = charSize;
            text.DisplayedString = displayedString;
            text.FillColor = textFillColor;
            text.IsBold = isBold;

            if (hideOverflow)
            {
                text.position = textPosition.GetVector(GetSize());
            }
            else
            {
                text.position = GetPosition() + textPosition.GetVector(GetSize());
            }

            text.position -= text.GetLocalBounds().Size * textAnchor;

            text.position = Vector2.Round(text.position); // keep that text *crispy*

            if (hideOverflow)
            {
                drawSpr.Position = GetPosition().Round();
                internalRenderTexture.Draw(text);
                drawSpr.TextureRect = new IntRect(0, 0, (int)lastSize.x, (int)lastSize.y);
                internalRenderTexture.Display();
                drawSpr.Texture = internalRenderTexture.Texture;

                rt.Draw(drawSpr);
            } else
            {
                rt.Draw(text);
            }
        }

        public override void OnDestroy(GameObject gameObject)
        {
            destroyed = true;
            internalRenderTexture.Dispose();
            // dispose text? idk prob later
        }
    }
}
