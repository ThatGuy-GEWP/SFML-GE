using SFML.Graphics;
using SFML.Window;

namespace SFML_Game_Engine.GUI
{
    /// <summary>
    /// An interactable button, does not actually draw anything to the screen by itself, see <see cref="GUIButtonPanel"/> for that.
    /// </summary>
    public class GUIButton : GUIPanel
    {
        public event Action<GUIButton> OnClick = null!;
        public event Action<GUIButton> OnHold = null!;
        public event Action<GUIButton> OnRelease = null!;

        public event Action<GUIButton> OnHoveringStart = null!;
        public event Action<GUIButton> OnHoveringEnd = null!;

        /// <summary>
        /// If false, the button will not respond to clicks.
        /// </summary>
        public bool interactable = true;

        /// <summary>
        /// If false, the button will not have hovering effects.
        /// </summary>
        public bool useHoverEffects = true;

        /// <summary>
        /// If true, the cursor will switch to a pointing image when hovering
        /// </summary>
        public bool changeCuror = true;

        public Color hoverColor = defaultSecondary;
        public Color heldColor = defaultPressed;

        Color currentColor;

        public bool Hovering { get; private set; } = false;
        public bool HeldDown { get; private set; } = false;

        bool lastClickState = false;
        bool clickedThis = false;

        public override void Update()
        {
            if (!Project.App.HasFocus()) { return; }

            Vector2 mousePos = Scene.GetMouseScreenPosition();

            BoundBox bounds = GetBounds();
            bool isMousePressed = Project.IsMouseButtonHeld(Mouse.Button.Left);

            bool wasHovering = Hovering;

            if (bounds.WithinBounds(mousePos))
            {
                Hovering = true;
                if (useHoverEffects) { currentColor = hoverColor; }
            }
            else { Hovering = false; if (useHoverEffects) { currentColor = backgroundColor; } }

            if (!wasHovering && Hovering)
            {
                Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.Hand));
                if (interactable) { OnHoveringStart?.Invoke(this); }
            }
            if (wasHovering && !Hovering)
            {
                Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.Arrow));
                if (interactable) { OnHoveringEnd?.Invoke(this); }
            }

            if (Hovering)
            {
                if (lastClickState == false && isMousePressed)
                {
                    if (interactable) { OnClick?.Invoke(this); }
                    clickedThis = true;
                }
            }

            if (Hovering && isMousePressed)
            {
                if (interactable) { OnHold?.Invoke(this); }
                HeldDown = true;
                currentColor = heldColor;
            }
            else { HeldDown = false; }

            if (lastClickState == true && !isMousePressed && clickedThis)
            {
                if (interactable) { OnRelease?.Invoke(this); }
                clickedThis = false;
            }

            lastClickState = isMousePressed;
        }

        RectangleShape debugRect = new RectangleShape(new Vector2(50, 50));


        protected override void PrePass(RenderTarget rt)
        {
            backgroundPanelRect.FillColor = currentColor;
        }


        protected override void PostPass(RenderTarget rt)
        {
            debugRect.Position = GetPosition();
            debugRect.Size = GetSize();
            debugRect.OutlineThickness = 1;
            debugRect.FillColor = Color.Transparent;

            if (!Hovering)
            {
                debugRect.OutlineColor = Color.Green;
            }
            else
            {
                debugRect.OutlineColor = Color.Yellow;
            }

            if (HeldDown)
            {
                debugRect.OutlineColor = Color.Cyan;
            }

            //rt.Draw(debugRect);
        }
    }
}
