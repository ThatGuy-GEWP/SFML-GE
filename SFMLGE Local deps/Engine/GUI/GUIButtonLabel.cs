using SFML_Game_Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.GUI
{
    /// <summary>
    /// A <see cref="GUIButton"/> with an included <see cref="GUIPanel"/> and <see cref="GUIText"/>, uses a shared transform.
    /// </summary>
    public class GUIButtonLabel : GUIComponent
    {
        public GUIButton button;
        public GUIPanel panel;
        public GUIText text;

        public GUIButtonLabel(GUIContext context, string displayedString) : base(context)
        {
            panel = new GUIPanel(context);
            button = new GUIButton(context);
            text = new GUIText(context, defaultFontName);

            text.displayedString = displayedString;
            panel.transform.parent = this.transform;
            button.transform.parent = panel.transform;

            transform.size = new Vector2(150, 50);


            // shared transforms work here, maybe dont use them anywhere else though
            text.transform.parent = transform;
            panel.transform = transform;
            button.transform = transform;

            button.OnHoveringStart += (button) =>
            {
                panel.backgroundColor = defaultPressed;
            };

            button.OnHoveringEnd += (button) =>
            {
                panel.backgroundColor = defaultBackground;
            };
        }

        public override void Update()
        {
            text.transform.origin = new Vector2(0.5f, 0.5f);
            text.transform.LocalPosition = transform.size / 2f + new Vector2(0,-5);
        }
    }
}
