using SFML.Graphics;
using SFML.Window;
using SFML_GE.Debugging;
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
        public bool clipInteraction = false; // just spent 4 minutes debugging since this was on by default and i forgot about it, lmao

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
            currentColor = backgroundColor;
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

            // for click blocking stuff
            bool localInteractable = interactable && Scene.HoveredClickable == this;

            // for click blocking stuff
            bool localUseHoverEffects = useHoverEffects;

            if (bounds.WithinBounds(mousePos) && localInteractable)
            {
                Hovering = true;
                if (localUseHoverEffects) { currentColor = hoverColor; }
            }
            else { Hovering = false; if (localUseHoverEffects) { currentColor = backgroundColor; } }

            bool hoveringEnded = lastHovering == true && Hovering == false;
            bool hoveringStarted = lastHovering == false && Hovering == true;

            if (hoveringStarted)
            {
                if (localUseHoverEffects) { Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.Hand)); }
                if (localInteractable) { OnHoveringStart?.Invoke(this); }
            }
            if (hoveringEnded)
            {
                if (localUseHoverEffects) { Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.Arrow)); }
                if (localInteractable) { OnHoveringEnd?.Invoke(this); }
            }

            if (Hovering)
            {
                if (lastClickState == false && isMousePressed)
                {
                    if (localInteractable) { OnClick?.Invoke(this); }
                    clickedThis = true;
                }
            }

            if (Hovering && isMousePressed)
            {
                if (localInteractable) { OnHold?.Invoke(this); }
                HeldDown = true;
                if (useHoverEffects && localInteractable) { currentColor = heldColor; };
            }
            else { HeldDown = false; }

            if (lastClickState == true && !isMousePressed && clickedThis)
            {
                if (localInteractable) { OnRelease?.Invoke(this); }
                clickedThis = false;
            }

            lastClickState = isMousePressed;
            interactable = lastInerac;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        protected override void PostPass(RenderTarget rt)
        {
            base.PostPass(rt);

            //BoundBox b = GetBounds();

            //rt.Draw(new Vertex[] { b.TopLeft, b.TopRight }, PrimitiveType.Lines);
            //rt.Draw(new Vertex[] { b.TopRight, b.BottomRight }, PrimitiveType.Lines);
            //rt.Draw(new Vertex[] { b.BottomRight, b.BottomLeft }, PrimitiveType.Lines);
            //rt.Draw(new Vertex[] { b.BottomLeft, b.TopLeft }, PrimitiveType.Lines);
        }
    }
}
