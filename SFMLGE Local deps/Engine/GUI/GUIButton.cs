using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.GUI
{
    /// <summary>
    /// An interactable button, does not actually draw anything to the screen by itself, see <see cref="GUIButtonPanel"/> for that.
    /// </summary>
    public class GUIButton : GUIComponent
    {
        public event Action<GUIButton> OnClick = null!;
        public event Action<GUIButton> OnHold = null!;
        public event Action<GUIButton> OnRelease = null!;

        public event Action<GUIButton> OnHoveringStart = null!;
        public event Action<GUIButton> OnHoveringEnd = null!;

        public bool Hovering { get; private set; } = false;
        public bool HeldDown { get; private set; } = false;

        bool lastClickState = false;
        bool clickedThis = false;

        public GUIButton(GUIContext context) : base(context)
        {
            transform.size = new Vector2(150, 50);
            transform.origin = new Vector2(0.0f, 0.0f);
        }

        public GUIButton(GUIContext context, Vector2 size) : base(context)
        {
            transform.size = size;
            transform.origin = new Vector2(0.0f, 0.0f);
        }

        public override void Update()
        {
            Vector2 mousePos = context.scene.GetMouseScreenPosition();

            bool isMousePressed = Mouse.IsButtonPressed(Mouse.Button.Left);

            bool inXBounds =
                mousePos.x >= transform.WorldPosition.x &&
                mousePos.x <= transform.WorldPosition.x + transform.size.x;
            bool inYBounds =
                mousePos.y >= transform.WorldPosition.y &&
                mousePos.y <= transform.WorldPosition.y + transform.size.y;

            bool wasHovering = Hovering;

            if(inXBounds && inYBounds)
            {
                Hovering = true;
            } else { Hovering = false; }

            if(!wasHovering && Hovering)
            {
                OnHoveringStart?.Invoke(this);
            }
            if(wasHovering && !Hovering)
            {
                OnHoveringEnd?.Invoke(this);
            }

            if (Hovering)
            {
                if (lastClickState == false && isMousePressed)
                {
                    OnClick?.Invoke(this);
                    clickedThis = true;
                }
            }

            if(clickedThis && lastClickState == true && isMousePressed)
            {
                OnHold?.Invoke(this);
                HeldDown = true;
            } else { HeldDown = false; }

            if (lastClickState == true && !isMousePressed && clickedThis)
            {
                OnRelease?.Invoke(this);
                clickedThis = false;
            }

            lastClickState = isMousePressed;
        }

        RectangleShape debugRect = new RectangleShape(new Vector2(50, 50));

        public override void OnRender(RenderTarget rt)
        {
            debugRect.Position = transform.WorldPosition;
            debugRect.Size = transform.size;
            debugRect.OutlineThickness = 1;
            debugRect.FillColor = Color.Transparent;
            
            if(!Hovering )
            {
                debugRect.OutlineColor = Color.Green;
            } else
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
