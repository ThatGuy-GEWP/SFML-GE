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
    /// A <see cref="GUIButton"/> with an included <see cref="GUIPanel"/>, uses a shared transform amongst the panel, button and itself
    /// </summary>
    public class GUIButtonPanel : GUIComponent
    {
        public GUIButton button;
        public GUIPanel panel;

        public GUIButtonPanel(GUIContext context) : base(context)
        {
            panel = new GUIPanel(context);
            button = new GUIButton(context);

            transform.size = new Vector2(150, 50);


            // shared transforms work here, maybe dont use them anywhere else though
            panel.transform = transform;
            button.transform = transform;

            panel.autoQueue = false;
            EnableHoverEffects();
        }

        public GUIButtonPanel(GUIContext context, bool useHoverEffects) : base(context)
        {
            panel = new GUIPanel(context);
            button = new GUIButton(context);

            panel.transform.parent = this.transform;
            button.transform.parent = panel.transform;

            transform.size = new Vector2(150, 50);


            // shared transforms work here, maybe dont use them anywhere else though
            panel.transform = transform;
            button.transform = transform;

            panel.autoQueue = false;

            if (useHoverEffects) { EnableHoverEffects(); }
        }

        void EnableHoverEffects()
        {
            button.OnHoveringStart += (button) =>
            {
                panel.backgroundColor = defaultPressed;
            };

            button.OnHoveringEnd += (button) =>
            {
                panel.backgroundColor = defaultBackground;
            };
        }

        public override void OnRender(RenderTarget rt)
        {
            panel.OnRender(rt);
        }
    }
}
