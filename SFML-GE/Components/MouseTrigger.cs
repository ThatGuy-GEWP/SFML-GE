using SFML.Graphics;
using SFML.Window;
using SFML_GE.System;

namespace SFML_GE.Components
{
    /// <summary>
    /// A trigger that can sense when the mouse interacts with it, great for UI or ingame buttons
    /// </summary>
    public class MouseTrigger : Component, IRenderable
    {
        /// <inheritdoc/>
        public int ZOffset { get; set; } = 0;
        /// <inheritdoc/>
        public bool Visible { get; set; } = true;
        /// <inheritdoc/>
        public bool AutoQueue { get; set; } = false;

        /// <summary>
        /// An Additional offset to apply to the trigger, relative to the parent.
        /// </summary>
        public Vector2 Offset; // relative to gameObject of course.
        /// <summary>
        /// The size of this <see cref="MouseTrigger"/>, in pixels.
        /// </summary>
        public Vector2 Size; // not scale!, absolute size of this mouse trigger
        /// <summary>
        /// The origin of this <see cref="MouseTrigger"/>, where (0, 0) is the top left, and (1, 1) is the bottom right.
        /// </summary>
        public Vector2 Origin; // origin is same as everying else, (0.0,0.0) would be the top left of this box, (1.0,1.0) would be bottom right

        /// <summary> Draws the mouseTrigger with a green outline when it can be pressed </summary>
        public bool debugDraw = false;

        /// <summary> Target button of this trigger. </summary>
        public Mouse.Button Button = Mouse.Button.Left;

        /// <summary> True if mouse is held and hovering over this trigger. </summary>
        public bool IsMouseHeld { get; private set; } = false;

        /// <summary>True for a single frame when this trigger is clicked </summary>
        public bool IsMousePressed { get; private set; } = false;

        /// <summary>True if mouse is hovering over this trigger. </summary>
        public bool IsMouseHovering { get; private set; } = false;

        /// <summary>If true, the trigger will check within the gameObject.Position + screen space or smtn idfk</summary>
        public bool relativeToScreen = true;

        /// <summary>If true, the trigger will only work if the program is in focus.</summary>
        public bool requireFocus = true;

        /// <inheritdoc/>
        public RenderQueueType QueueType { get; set; } = RenderQueueType.DefaultQueue;

        /// <summary> Called once every time the trigger is clicked. Passes the current <see cref="MouseTrigger"/> </summary>
        public event Action<MouseTrigger> OnClick = null!;

        /// <summary> Called every frame the trigger is held down. Passes the current <see cref="MouseTrigger"/> </summary>
        public event Action<MouseTrigger> OnHeld = null!;

        /// <summary> Called once every time the trigger is unclicked. Passes the current <see cref="MouseTrigger"/> </summary>
        public event Action<MouseTrigger> OnRelease = null!;

        /// <summary> Called once the mouse enters the trigger. Passes the current <see cref="MouseTrigger"/> </summary>
        public event Action<MouseTrigger> OnMouseEnter = null!;

        /// <summary> Called once the mouse leaves the trigger. Passes the current <see cref="MouseTrigger"/> </summary>
        public event Action<MouseTrigger> OnMouseExit = null!;

        /// <summary>
        /// Creates a new <see cref="MouseTrigger"/>
        /// </summary>
        /// <param name="size">the size of this trigger in pixels</param>
        /// <param name="origin">the origin of this trigger, where (0,0) is the top left, and (1,1) is the bottom right</param>
        /// <param name="offset">the offset to apply to this trigger, relative to its parent position</param>
        /// <param name="button">the mouse button that activates this trigger</param>
        public MouseTrigger(Vector2 size, Vector2 origin, Vector2 offset, Mouse.Button button)
        {
            Offset = offset;
            Size = size;
            Origin = origin;
            Button = button;
        }

        /// <summary>
        /// Creates a new <see cref="MouseTrigger"/>
        /// </summary>
        /// <param name="size">the size of this trigger in pixels</param>
        /// <param name="origin">the origin of this trigger, where (0,0) is the top left, and (1,1) is the bottom right</param>
        /// <param name="offset">the offset to apply to this trigger, relative to its parent position</param>
        public MouseTrigger(Vector2 size, Vector2 origin, Vector2 offset)
        {
            Offset = offset;
            Size = size;
            Origin = origin;
        }

        /// <summary>
        /// Creates a new <see cref="MouseTrigger"/>
        /// </summary>
        /// <param name="size">the size of this trigger in pixels</param>
        /// <param name="origin">the origin of this trigger, where (0,0) is the top left, and (1,1) is the bottom right</param>
        public MouseTrigger(Vector2 size, Vector2 origin)
        {
            Offset = new Vector2(0, 0);
            Size = size;
            Origin = origin;
        }

        /// <summary>
        /// Creates a new <see cref="MouseTrigger"/>
        /// </summary>
        /// <param name="size">the size of this trigger in pixels</param>
        public MouseTrigger(Vector2 size)
        {
            Offset = new Vector2(0, 0);
            Size = size;
            Origin = new Vector2(0, 0);
        }

        bool mouseDown = false;

        /// <inheritdoc/>
        public override void Update()
        {
            if (requireFocus && !Project.App.HasFocus()) { return; }

            Vector2 realPosition = gameObject.transform.GlobalPosition + Offset;

            Vector2 lowerBound = realPosition;
            Vector2 upperBound = realPosition + Size;

            upperBound -= Size * Origin;
            lowerBound -= Size * Origin;

            Vector2 mousePos = relativeToScreen ? Scene.GetMouseScreenPosition() : Scene.GetMouseWorldPosition();

            bool withinXBounds = mousePos.x <= upperBound.x && mousePos.x >= lowerBound.x;
            bool withinYBounds = mousePos.y <= upperBound.y && mousePos.y >= lowerBound.y;

            bool wasHovering = IsMouseHovering;
            bool wasPressed = mouseDown;

            if (withinXBounds && withinYBounds)
            {
                IsMouseHovering = true;
            }
            else { IsMouseHovering = false; }

            if (!wasHovering && IsMouseHovering) { OnMouseEnter?.Invoke(this); }
            if (wasHovering && !IsMouseHovering) { OnMouseExit?.Invoke(this); }

            if (IsMousePressed == true) { IsMousePressed = false; }

            mouseDown = Mouse.IsButtonPressed(Button);

            if (IsMouseHovering && mouseDown && !wasPressed) { OnClick?.Invoke(this); IsMousePressed = true; }
            if (IsMouseHovering && !mouseDown && wasPressed) { OnRelease?.Invoke(this); }

            if (IsMouseHovering && mouseDown && wasPressed)
            {
                OnHeld?.Invoke(this);
                IsMouseHeld = true;
            }
            else { IsMouseHeld = false; }

            if (debugDraw && !AutoQueue)
            {
                QueueType = relativeToScreen ? RenderQueueType.OverlayQueue : RenderQueueType.DefaultQueue;
                AutoQueue = true;
            }
            else { AutoQueue = false; }
        }

        /// <inheritdoc/>
        public void OnRender(RenderTarget rt)
        {
            Vector2 worldPosition = gameObject.transform.GlobalPosition + Offset;
            RectangleShape shape = new RectangleShape(Size);
            shape.FillColor = new Color(255, 255, 255, 0);
            shape.OutlineColor = new Color(0, 255, 0, 255);
            shape.OutlineThickness = -1f;
            shape.Position = worldPosition;
            shape.Origin = Size * Origin;
            rt.Draw(shape);
        }
    }
}
