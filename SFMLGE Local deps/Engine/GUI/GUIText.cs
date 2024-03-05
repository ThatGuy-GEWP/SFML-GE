using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.GUI
{
    internal class GUIText : GUIComponent
    {
        public string displayedString = "empty";
        Text txt = null!;
        public uint CharSize = 18;

        public Vector2 textSize = Vector2.zero;

        public GUIText(GUIContext context) : base(context)
        {
            txt = new Text(displayedString, RenderText.defaultFont);
            txt.FillColor = GUIComponent.defaultForeground;
        }

        public GUIText(string displayedString, GUIContext context) : base(context)
        {
            this.displayedString = displayedString;
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
