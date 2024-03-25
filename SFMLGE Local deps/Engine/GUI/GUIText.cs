using SFML.Audio;
using SFML.Graphics;

namespace SFML_Game_Engine.GUI
{
    /// <summary>
    /// Just plain text without any other stuff included.
    /// </summary>
    public class GUIText : GUIComponent
    {
        public string displayedString = "";

        public uint CharSize = 18;

        public FontResource font;

        public Text text;

        string targFont = "";

        public GUIText(GUIContext context, FontResource font, string displayedString = "") : base(context)
        {
            text = new Text();
            this.displayedString = displayedString;
            transform.zOrder = 5;
            this.font = font;
        }

        public GUIText(GUIContext context, string fontName, string displayedString = "") : base(context)
        {
            text = new Text();
            this.displayedString = displayedString;
            transform.zOrder = 5;
            targFont = fontName;
            font = null!;
        }

        public override void Start()
        {
            if(font == null && targFont != string.Empty)
            {
                font = context.Project.GetResource<FontResource>(targFont);
            }
        }

        public override void Update()
        {
            if (!started) { return; }
            if (font == null) { throw new NullReferenceException("Font is null!"); }
            text.Font = font.resource;
            text.DisplayedString = displayedString;
            text.CharacterSize = CharSize;

            FloatRect bounds = text.GetLocalBounds();
            transform.size = new Vector2(bounds.Width, bounds.Height);
            text.Origin = new Vector2(bounds.Left, bounds.Top);
            text.Position = transform.WorldPosition;
        }


        public override void OnRender(RenderTarget rt)
        {
            if (!started) { return; }
            text.Position = Vector2.Floor(text.Position);
            rt.Draw(text);
        }

    }
}
