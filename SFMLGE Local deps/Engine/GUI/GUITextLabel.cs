using SFML.Graphics;
using SFML_Game_Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.GUI
{
    /// <summary>
    /// A <see cref="GUIText"/> with an included <see cref="GUIPanel"/>
    /// </summary>
    public class GUITextLabel : GUIComponent
    {
        public GUIPanel panel;
        public GUIText text;

        public float outlineThickness = 1.0f;
        public float padding = 15;
        public uint charSize = 18;
        public string displayedString = string.Empty;

        public GUITextLabel(GUIContext context, string displayedString) : base(context)
        { 
            this.displayedString = displayedString;

            panel = new GUIPanel(context);
            text = new GUIText(context, defaultFontName);

            transform.size = panel.transform.size;
            panel.transform = transform;
            panel.transform.origin = new Vector2(0, 0);

            text.transform.parent = transform;
        }

        public override void Update()
        {
            if (!visible) { return; }

            text.CharSize = charSize;
            text.displayedString = displayedString;

            text.transform.origin = new Vector2(0.5f, 0.35f);
            text.transform.LocalPosition = panel.transform.size / 2f;

            text.Update();
        }


        public override void OnRender(RenderTarget rt)
        {
            panel.OnRender(rt);
            text.OnRender(rt);
        }

    }
}
