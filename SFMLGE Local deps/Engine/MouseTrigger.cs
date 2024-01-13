using SFML.Graphics;
using SFML.Window;

namespace SFML_Game_Engine
{
    // too lazy to rewrite this, i probably should though!
    /// <summary>
    /// A trigger that can sense when the mouse interacts with it, great for UI or ingame buttons
    /// </summary>
    internal class MouseTrigger : Component
    {
        /// <summary>An Offset relative to the <see cref="GameObject.Position"/></summary>
        public Vector2 Offset;

        /// <summary>Not scale!, absolute size of this mouse trigger</summary>
        public Vector2 Size;

        /// <summary>Origin of the mouse trigger, (0.5, 0.5) would be centered, (0.0, 0.0) would be the top left</summary>
        public Vector2 Origin;

        /// <summary>Target button of this trigger.</summary>
        public Mouse.Button Button = Mouse.Button.Left;

        /// <summary>True if mouse is held and hovering over this trigger.</summary>
        public bool mouseHeld = false;

        /// <summary>True for a single frame when this trigger is clicked with the selected <see cref="Button"/></summary>
        public bool mousePressed = false;

        /// <summary>True if mouse is hovering over this trigger.</summary>
        public bool mouseHovering = false;

        /// <summary>Called once every time the trigger is clicked. Passes the current <see cref="MouseTrigger"/></summary>
        public event Action<MouseTrigger> OnClick = null!;

        /// <summary>Called once the trigger is released, or the cursor left the trigger while held. passes the current <see cref="MouseTrigger"/></summary>
        public event Action<MouseTrigger> OnRelease = null!;

        /// <summary>Called while the trigger is held, passes the current <see cref="MouseTrigger"/></summary>
        public event Action<MouseTrigger> OnHeld = null!;

        bool mousePressedReset = false; // used internaly, do not remove

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
            Vector2 newPosition = gameObject.Position + Offset;

            Vector2 upperBound = newPosition;
            Vector2 lowerBound = newPosition + Size;

            upperBound -= Size * Origin;
            lowerBound -= Size * Origin;

            Vector2 mousePos = gameObject.Scene.GetMousePosition();

            bool was = mouseHeld;

            if (mousePressed) { mousePressed = false; }
            mouseHeld = false;

            if (Mouse.IsButtonPressed(Button) && mouseHovering == false)
            {
                return;
            }

            if (mousePos.x > upperBound.x && mousePos.y > upperBound.y && mousePos.x < lowerBound.x && mousePos.y < lowerBound.y)
            {
                mouseHovering = true;
                mouseHeld = Mouse.IsButtonPressed(Button);
                if(was == true && mouseHeld == false)
                {
                    OnRelease?.Invoke(this);
                }
            }
            else
            {
                if (was == true)
                {
                    OnRelease?.Invoke(this);
                }
                mouseHovering = false;
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

            if (mouseHeld)
            {
                OnHeld?.Invoke(this);
            }
        }
    }
}
