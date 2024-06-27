
using SFML.Graphics;
using SFML.Window;
using SFML_Game_Engine.System;

namespace SFML_Game_Engine.GUI
{
    internal class GUIButtonLabel : GUILabel
    {
        public event Action<GUIButtonLabel> OnClick = null!;

        /// <summary> Called while the button is held </summary>
        public event Action<GUIButtonLabel> OnHold = null!;

        /// <summary> Called when the button is released </summary>
        public event Action<GUIButtonLabel> OnRelease = null!;

        /// <summary> Called when the button is being hovered over </summary>
        public event Action<GUIButtonLabel> OnHoveringStart = null!;

        /// <summary> Called when the button is no longer being hovered over </summary>
        public event Action<GUIButtonLabel> OnHoveringEnd = null!;

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

        public GUIButtonLabel() { }

        public GUIButtonLabel(string displayedString)
        {
            this.displayedString = displayedString;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            base.Update();
            if (!Project.App.HasFocus()) { return; }

            Vector2 mousePos = Scene.GetMouseScreenPosition();

            BoundBox bounds = GetBounds();
            bool isMousePressed = Project.IsMouseButtonHeld(Mouse.Button.Left);

            bool wasHovering = Hovering;

            if(bounds.WithinBounds(mousePos))
            {
                Hovering = true;
                if (useHoverEffects) { currentColor = hoverColor; }
            }
            else { Hovering = false; if (useHoverEffects) { currentColor = backgroundColor; } }

            bool lastInerac = interactable;
            if (clipInteraction)
            {
                BoundBox? ownerBounds = ContainerBounds();
                if (ownerBounds != null)
                {
                    if (!((BoundBox)ownerBounds).WithinBounds(mousePos))
                    {
                        if (wasHovering && !Hovering && interactable)
                        {
                            Project.CursorState = Cursor.CursorType.Arrow;
                            OnHoveringEnd?.Invoke(this);
                        }
                        interactable = false;
                    }
                }
            }

            if (!wasHovering && Hovering && interactable)
            {
                if (useHoverEffects) { Project.CursorState = Cursor.CursorType.Hand; }
                OnHoveringStart?.Invoke(this);
            }
            if (wasHovering && !Hovering && interactable)
            {
                if (useHoverEffects) { Project.CursorState = Cursor.CursorType.Arrow; }
                OnHoveringEnd?.Invoke(this);
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

        public override void OnDestroy(GameObject gameObject)
        {
            base.OnDestroy(gameObject);
            if (Hovering && interactable)
            {
                Project.CursorState = Cursor.CursorType.Arrow;
            }
        }

        protected override void PrePass(RenderTarget rt, in Vector2 pos, in Vector2 size)
        {
            base.PrePass(rt, pos, size);
            backgroundPanelRect.FillColor = useHoverEffects ? currentColor : backgroundColor;
        }
    }
}
