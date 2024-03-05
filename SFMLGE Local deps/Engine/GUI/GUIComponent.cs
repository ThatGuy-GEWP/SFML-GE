using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.GUI
{
    public abstract class GUIComponent
    {
        public static readonly Color defaultForeground = new Color(0xDCE0FAFF);
        public static readonly Color defaultBackground = new Color(0x2C2D36FF);
        public static readonly Color defaultSecondary = new Color(0x212436FF);


        public GUIContext context;
        public GUITransform transform = new GUITransform();
        public bool visible = true;

        public virtual void Update()
        {
            return;
        }

        public virtual void OnAdd()
        {
            return;
        }

        public virtual void OnRender(RenderTarget rt)
        {
            return;
        }
    }
}
