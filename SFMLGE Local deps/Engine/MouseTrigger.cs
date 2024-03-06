using SFML.Graphics;
using SFML.Window;

namespace SFML_Game_Engine
{
    /// <summary>
    /// A trigger that can sense when the mouse interacts with it, great for UI or ingame buttons
    /// </summary>
    public class MouseTrigger : Component, IRenderable
    {
        public sbyte ZOrder { get; set; } = 127;
        public bool Visible { get; set; } = true;
        public bool AutoQueue { get; set; } = false;


        public Vector2 Offset; // relative to gameObject of course.
        public Vector2 Size; // not scale!, absolute size of this mouse trigger
        public Vector2 Origin; // origin is same as everying else, (0.0,0.0) would be the top left of this box, (1.0,1.0) would be bottom right

        /// <summary> Draws the mouseTrigger with a green outline when it can be pressed </summary>
        public bool debugDraw = true;

        /// <summary> Target button of this trigger. </summary>
        public Mouse.Button Button = Mouse.Button.Left;

        /// <summary> True if mouse is held and hovering over this trigger. </summary>
        public bool IsMouseHeld {get; private set;}= false;

        /// <summary>True for a single frame when this trigger is clicked </summary>
        public bool IsMousePressed { get; private set; } = false;

        /// <summary>True if mouse is hovering over this trigger. </summary>
        public bool IsMouseHovering {get; private set;} = false;

        /// <summary>If true, the trigger will check within the gameObject.Position + screen space or smtn idfk</summary>
        public bool relativeToScreen = true;

        /// <summary>If true, the trigger will only work if the program is in focus.</summary>
        public bool requireFocus = true;

        public RenderQueueType QueueType { get; set; } = RenderQueueType.DefaultQueue;

        /// <summary> Called once every time the trigger is clicked. Passes the current <see cref="MouseTrigger"/> </summary>
        public event Action<MouseTrigger> OnClick;

        /// <summary> Called every frame the trigger is held down. Passes the current <see cref="MouseTrigger"/> </summary>
        public event Action<MouseTrigger> OnHeld;

        /// <summary> Called once every time the trigger is unclicked. Passes the current <see cref="MouseTrigger"/> </summary>
        public event Action<MouseTrigger> OnRelease;

        /// <summary> Called once the mouse enters the trigger. Passes the current <see cref="MouseTrigger"/> </summary>
        public event Action<MouseTrigger> OnMouseEnter;

        /// <summary> Called once the mouse leaves the trigger. Passes the current <see cref="MouseTrigger"/> </summary>
        public event Action<MouseTrigger> OnMouseExit;

        public MouseTrigger(Vector2 size, Vector2 origin, Vector2 offset, Mouse.Button button)
        {
            Offset = offset;
            Size = size;
            Origin = origin;
            Button = button;
        }

        public MouseTrigger(Vector2 size, Vector2 origin, Vector2 offset)
        {
            Offset = offset;
            Size = size;
            Origin = origin;
        }

        public MouseTrigger(Vector2 size, Vector2 origin)
        {
            Offset = new Vector2(0, 0);
            Size = size;
            Origin = origin;
        }

        public MouseTrigger(Vector2 size)
        {
            Offset = new Vector2(0, 0);
            Size = size;
            Origin = new Vector2(0, 0);
        }

        bool mouseDown = false;

        public override void Update()
        {
            if (requireFocus && !project.App.HasFocus()) { return; }

            Vector2 realPosition = gameObject.transform.WorldPosition + Offset;

            Vector2 lowerBound = realPosition;
            Vector2 upperBound = realPosition + Size;

            upperBound -= Size * Origin;
            lowerBound -= Size * Origin;

            Vector2 mousePos = relativeToScreen ? scene.GetMouseScreenPosition() : scene.GetMouseWorldPosition();

            bool withinXBounds = mousePos.x <= upperBound.x && mousePos.x >= lowerBound.x;
            bool withinYBounds = mousePos.y <= upperBound.y && mousePos.y >= lowerBound.y;

            bool wasHovering = IsMouseHovering;
            bool wasPressed = mouseDown;

            if (withinXBounds && withinYBounds)
            {
                IsMouseHovering = true;
            } else { IsMouseHovering = false; }

            if (!wasHovering && IsMouseHovering) { OnMouseEnter?.Invoke(this); }
            if (wasHovering && !IsMouseHovering) { OnMouseExit?.Invoke(this); }

            if(IsMousePressed == true) { IsMousePressed = false; }

            mouseDown = Mouse.IsButtonPressed(Button);

            if (IsMouseHovering && mouseDown && !wasPressed) { OnClick?.Invoke(this); IsMousePressed = true; }
            if (IsMouseHovering && !mouseDown && wasPressed) { OnRelease?.Invoke(this); }

            if(IsMouseHovering && mouseDown && wasPressed)
            {
                OnHeld?.Invoke(this);
                IsMouseHeld = true;
            } else { IsMouseHeld = false; }

            if (debugDraw)
            {
                QueueType = relativeToScreen ? RenderQueueType.OverlayQueue : RenderQueueType.DefaultQueue;
                AutoQueue = true;
            } else { AutoQueue = false; }
        }

        public void OnRender(RenderTarget rt)
        {
            Vector2 worldPosition = gameObject.transform.WorldPosition + Offset;
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
