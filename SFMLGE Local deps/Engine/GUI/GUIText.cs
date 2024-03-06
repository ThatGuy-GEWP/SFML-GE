using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.GUI
{
    /// <summary>
    /// Just plain text without any other stuff included.
    /// </summary>
    public class GUIText : GUIComponent
    {
        public string displayedString = "empty";
        Text txt = null!;
        public uint CharSize = 18;

        public Vector2 textSize = Vector2.zero;

        public GUIText(GUIContext context) : base(context)
        {
            transform.zOrder = 5;
            txt = new Text(displayedString, RenderText.defaultFont);
            txt.FillColor = GUIComponent.defaultForeground;
        }

        public GUIText(GUIContext context, string displayedString) : base(context)
        {
            transform.zOrder = 5;
            this.displayedString = displayedString;
            txt = new Text(displayedString, RenderText.defaultFont);
            txt.DisplayedString = displayedString;
        }

        public override void Update()
        {

            txt.DisplayedString = displayedString;
            txt.CharacterSize = CharSize;

            transform.size = new Vector2(txt.GetLocalBounds().Width, txt.GetLocalBounds().Height);

            txt.Position = transform.WorldPosition;
        }

        public override void OnRender(RenderTarget rt)
        {
            rt.Draw(txt);
        }

    }
}
