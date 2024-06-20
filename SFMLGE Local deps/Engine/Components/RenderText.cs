using SFML.Graphics;
using SFML_Game_Engine.System;
using SFML_Game_Engine.System;
using SFMLGE_Local_deps.Engine.System;

namespace SFML_Game_Engine.Components
{
    /// <summary>
    /// A simple class for rendering text
    /// </summary>
    public class RenderText : Component, IRenderable
    {
        public static readonly Font defaultFont = new Font("Engine\\Font\\Roboto-Regular.ttf");
        public Font font = defaultFont;

        public Color color = Color.White;

        public Color outlineColor = Color.White;
        public float outlineThickness = 0.0f;

        Text rtext;

        string _text = string.Empty;

        public string Text
        {
            get { return _text; }
            set
            {
                if (value == _text) { return; }
                rtext.DisplayedString = value;
                _text = value;
            }
        }
        public uint size = 40;
        public Vector2 offset = new Vector2(0, 0);

        public RenderText(string Text)
        {
            rtext = new Text(this.Text, font);
            this.Text = Text;
        }

        public RenderText(string Text, Font font)
        {
            rtext = new Text(this.Text, font);
            this.Text = Text;
            this.font = font;
        }

        public int ZOffset { get; set; } = 0;
        public bool Visible { get; set; } = true;
        public bool AutoQueue { get; set; } = true;
        public RenderQueueType QueueType { get; set; } = RenderQueueType.DefaultQueue;

        public void OnRender(RenderTarget rt)
        {
            rtext.CharacterSize = size;
            rtext.FillColor = color;
            rtext.Rotation = gameObject.transform.rotation;
            rtext.Position = gameObject.transform.WorldPosition + offset;
            rtext.OutlineColor = outlineColor;
            rtext.OutlineThickness = outlineThickness;
            rt.Draw(rtext);
        }
    }
}
