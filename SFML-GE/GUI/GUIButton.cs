using SFML.Graphics;
using SFML.Window;
using SFML_GE.System;

namespace SFML_GE.GUI
{
    /// <summary>
    /// An Interactable GUI button.
    /// </summary>
    public class GUIButton : GUIPanel
    {
        /// <summary> Called when the button is clicked </summary>
        public event Action<GUIButton> OnClick = null!;

        /// <summary> Called while the button is held </summary>
        public event Action<GUIButton> OnHold = null!;

        /// <summary> Called when the button is released </summary>
        public event Action<GUIButton> OnRelease = null!;

        /// <summary> Called when the button is being hovered over </summary>
        public event Action<GUIButton> OnHoveringStart = null!;

        /// <summary> Called when the button is no longer being hovered over </summary>
        public event Action<GUIButton> OnHoveringEnd = null!;

        /// <summary>
        /// If <c>false</c>, the button will not respond to clicks.
        /// </summary>
        public bool interactable = true;

        /// <summary>
        /// If <c>true</c>, the button will not respond to clicks outside the base parent container.
        /// </summary>
        public bool clipInteraction = true;

        /// <summary>
        /// If <c>false</c>, the button will not have hovering effects.
        /// </summary>
        public bool useHoverEffects = true;

        /// <summary>
        /// If <c>true</c>, the cursor will switch to a pointer when hovering
        /// </summary>
        public bool changeCuror = true;

        /// <summary>
        /// The color to switch to when hovering, requires <see cref="useHoverEffects"/> to be true
        /// </summary>
        public Color hoverColor = defaultSecondary;

        /// <summary>
        /// The color to switch to when pressed down, requires <see cref="useHoverEffects"/> to be true
        /// </summary>
        public Color heldColor = defaultPressed;

        /// <summary>
        /// <c>true</c> while the mouse is hovering over this button.
        /// </summary>
        public bool Hovering { get; private set; } = false;

        /// <summary>
        /// <c>true</c> while the mouse is holding down this button.
        /// </summary>
        public bool HeldDown { get; private set; } = false;

        Color currentColor;
        bool lastClickState = false;
        bool clickedThis = false;

        /// <inheritdoc/>
        public override void Start()
        {
            base.Start();
        }

        /// <inheritdoc/>
        public override void Update()
        {
            if (!Project.App.HasFocus()) { return; }

            Vector2 mousePos = Scene.GetMouseScreenPosition();

            BoundBox bounds = GetBounds();
            bool isMousePressed = Project.IsMouseButtonHeld(Mouse.Button.Left);

            bool lastInerac = interactable;
            if (clipInteraction)
            {
                BoundBox? ownerBounds = ContainerBounds();
                if (ownerBounds != null)
                {
                    if (!((BoundBox)ownerBounds).WithinBounds(mousePos))
                    {
                        interactable = false;
                    }
                }
            }


            bool lastHovering = Hovering;

            if (bounds.WithinBounds(mousePos) && interactable)
            {
                Hovering = true;
                if (useHoverEffects) { currentColor = hoverColor; }
            }
            else { Hovering = false; if (useHoverEffects) { currentColor = backgroundColor; } }

            bool hoveringEnded = lastHovering == true && Hovering == false;
            bool hoveringStarted = lastHovering == false && Hovering == true;

            if (hoveringStarted)
            {
                if (useHoverEffects) { Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.Hand)); }
                if (interactable) { OnHoveringStart?.Invoke(this); }
            }
            if (hoveringEnded)
            {
                if (useHoverEffects) { Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.Arrow)); }
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
                if (useHoverEffects && interactable) { currentColor = heldColor; };
            }
            else { HeldDown = false; }

            if (lastClickState == true && !isMousePressed && clickedThis)
            {
                if (interactable && interactable) { OnRelease?.Invoke(this); }
                clickedThis = false;
            }

            lastClickState = isMousePressed;
            interactable = lastInerac;
        }

        public override void OnUnload()
        {
            if (Hovering)
            {
                Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.Arrow));
            }
        }

        /// <inheritdoc/>
        protected override void PrePass(RenderTarget rt, ref Vector2 pos, ref Vector2 size)
        {
            backgroundPanelRect.FillColor = useHoverEffects ? currentColor : backgroundColor;
        }

        /*RectangleShape debugRect = new RectangleShape(new Vector2(50, 50));
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

            rt.Draw(debugRect);
        }*/
    }
}
