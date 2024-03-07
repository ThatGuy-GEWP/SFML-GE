using SFML.Graphics;

namespace SFML_Game_Engine.GUI
{
    /// <summary>
    /// The base class for all GUIComponents.
    /// </summary>
    public abstract class GUIComponent
    {
        public static readonly Color defaultForeground = new Color(0xE0EBF1FF);
        public static readonly Color defaultBackground = new Color(0x474859FF);

        public static readonly Color defaultSecondary = new Color(0x61637BFF);
        public static readonly Color defaultPrimary = new Color(0x767997FF);

        public static readonly Color defaultPressed = new Color(0x2C2D36FF);

        public static readonly string defaultFontName = "Roboto-Regular";
        

        public GUIContext context;
        public GUITransform transform = new GUITransform();

        public bool started = false;
        public bool visible = true;
        public bool autoQueue = true;

        public GUIComponent(GUIContext context) 
        { 
            this.context = context;
            context.AddComponent(this); 
        }

        static protected void CopyTransformBasic(GUITransform from, GUITransform to)
        {
            to.LocalPosition = from.LocalPosition;
            to.origin = from.origin;
            to.size = from.size;
        }

        public virtual void Start()
        {
            return;
        }

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

        public virtual void OnRemove()
        {
            return;
        }
    }
}
