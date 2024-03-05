using SFML_Game_Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.GUI
{
    internal class GUIButtonPannel : GUIComponent
    {
        public GUIButton button;
        public GUIPanel panel;

        public GUIButtonPannel(GUIContext context) : base(context)
        {
            panel = new GUIPanel(context);
            button = new GUIButton(context);

            panel.transform.parent = this.transform;
            button.transform.parent = panel.transform;

            transform.size = new Vector2(150, 50);


            // shared transforms work here, maybe dont use them anywhere else though
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
    }
}
