using SFML.Graphics;
using SFML.Window;

namespace SFML_Game_Engine
{
    /// <summary>
    /// A trigger that can sense when the mouse interacts with it, great for UI or ingame buttons
    /// </summary>
    public class MouseTrigger : Component, IRenderable
    {
        public Vector2 Offset; // relative to gameObject of course.
        public Vector2 Size; // not scale!, absolute size of this mouse trigger
        public Vector2 Origin; // origin is same as everying else, (0.0,0.0) would be the top left of this box, (1.0,1.0) would be bottom right

        /// <summary>
        /// Draws the mouseTrigger with a green outline when it can be pressed,
        /// </summary>
        public bool debugDraw = false;

        /// <summary>
        /// Target button of this trigger.
        /// </summary>
        public Mouse.Button Button = Mouse.Button.Left;

        /// <summary>
        /// True if mouse is held and hovering over this trigger.
        /// </summary>
        public bool mouseHeld = false;
        /// <summary>
        /// True for a single frame when this trigger is clicked with the selected <see cref="Button"/>
        /// </summary>
        public bool mousePressed = false;
        /// <summary>
        /// True if mouse is hovering over this trigger.
        /// </summary>
        public bool mouseHovering = false;

        /// <summary>
        /// If true, the trigger will check within the gameObject.Position + screen space or smtn idfk
        /// </summary>
        public bool relativeToScreen = true;

        /// <summary>
        /// If true, the trigger will only work if the program is in focus.
        /// </summary>
        public bool requireFocus = true;

        bool mousePressedReset = false; // used internaly, do not remove

        public sbyte ZOrder { get; set; } = 127;
        public bool Visible { get; set; } = true;
        public bool AutoQueue { get; set; } = false;

        public RenderQueueType QueueType { get; set; } = RenderQueueType.DefaultQueue;

        /// <summary>
        /// Called once every time the trigger is clicked. Passes the current <see cref="MouseTrigger"/>
        /// </summary>
        public event Action<MouseTrigger> OnClick;

        /// <summary>
        /// Called once the mouse enters the trigger.
        /// </summary>
        public event Action<MouseTrigger> OnMouseEnter;

        /// <summary>
        /// Called once the mouse leaves the trigger.
        /// </summary>
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

        public override void Update()
        {
            if (requireFocus && !project.App.HasFocus()) { return; }

            Vector2 realPosition = gameObject.WorldPosition + Offset;

            Vector2 upperBound = realPosition;
            Vector2 lowerBound = realPosition + Size;

            upperBound -= Size * Origin;
            lowerBound -= Size * Origin;

            Vector2 mousePos = relativeToScreen ? scene.GetMouseScreenPosition() : scene.GetMouseWorldPosition();

            if (mousePressed) { mousePressed = false; }
            mouseHeld = false;

            if (Mouse.IsButtonPressed(Button) && mouseHovering == false)
            {
                return;
            }

            bool lastHover = mouseHovering;

            if (mousePos.x > upperBound.x && mousePos.y > upperBound.y && mousePos.x < lowerBound.x && mousePos.y < lowerBound.y)
            {
                mouseHovering = true;
                mouseHeld = Mouse.IsButtonPressed(Button);
            }
            else
            {
                mouseHovering = false;
            }

            if (mouseHovering != lastHover)
            {
                if (mouseHovering == false && lastHover == true)
                {
                    OnMouseExit?.Invoke(this);
                }
                if (mouseHovering == true && lastHover == false)
                {
                    OnMouseEnter?.Invoke(this);
                }
            }

            if (Mouse.IsButtonPressed(Button) && !mousePressedReset)
            {
                mousePressed = true;
                mousePressedReset = true;
                OnClick?.Invoke(this);
            }

            if (!Mouse.IsButtonPressed(Button))
            {
                mousePressedReset = false;
            }

            if (debugDraw)
            {
                if (relativeToScreen)
                {
                    scene.renderManager.AddToGUIQueue(this);
                    return;
                }
                scene.renderManager.AddToQueue(this);
            }
        }

        public void OnRender(RenderTarget rt)
        {
            Vector2 worldPosition = gameObject.WorldPosition + Offset;
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
