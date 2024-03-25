using SFML.Graphics;
using SFML.Window;

namespace SFML_Game_Engine.GUI
{
    public struct ScrollerContent
    {
        public float height;
        public string content;
        public object? obj;

        public ScrollerContent(string content, float height, object? obj)
        {
            this.content = content;
            this.height = height;
            this.obj = obj;
        }
    }


    /// <summary>
    /// Displays a <see cref="List{T}"/> of strings, allowing you to scroll through them.
    /// </summary>
    public class GUIScroller : GUIComponent
    {
        public bool Hovering = false;
        public GUIPanel panel;
        RenderTexture renderText;
        Sprite spr = new Sprite();


        public Color ContentBackgroundColor = defaultSecondary;
        public Color ContentOutlineColor = defaultPrimary;


        float minScrollPos = -100f;
        float maxScrollPos = 5f;
        float scrollPos = 5f;

        float contentSpacing = 5f;

        public float scrollSpeed = 8f;

        public int hoveredItem = -1;
        public int selectedItem = -1;

        public bool interactable = true;

        public event Action<GUIScroller, ScrollerContent> OnContentSelected = null!;

        public uint charSize = 21;

        public Font font { get; private set; }

        List<ScrollerContent> content = new List<ScrollerContent>();

        Text labelText;

        public GUIScroller(GUIContext context, Vector2 size) : base(context)
        {
            panel = new GUIPanel(context);
            panel.autoQueue = false;

            autoQueue = true;

            panel.transform = transform;

            transform.WorldPosition = new Vector2(600, 20);
            transform.size = size;

            renderText = new RenderTexture((uint)transform.size.x, (uint)transform.size.y);

            labelText = new Text();
        }

        float scrollDelta = 0f;


        bool setupevent = false;

        public override void Start()
        {
            if (labelText.Font == null)
            {
                labelText.Font = context.Project.GetResource<FontResource>(defaultFontName);
            }
            font = labelText.Font;
        }

        public override void Update()
        {
            if (!started) { return; }

            if (!setupevent)
            {
                context.Project.App.MouseWheelScrolled += (obj, args) =>
                {
                    scrollDelta = args.Delta;
                };
                setupevent = true;
            }

            if (!context.Project.App.HasFocus()) { return; }

            Vector2 mousePos = context.Scene.GetMouseScreenPosition();

            bool inXBounds =
                mousePos.x >= transform.WorldPosition.x &&
                mousePos.x <= transform.WorldPosition.x + transform.size.x;
            bool inYBounds =
                mousePos.y >= transform.WorldPosition.y &&
                mousePos.y <= transform.WorldPosition.y + transform.size.y;

            if (inXBounds && inYBounds)
            {
                Hovering = true;
            }
            else { Hovering = false; }

            if (Hovering && scrollDelta != 0)
            {
                scrollPos += scrollDelta * scrollSpeed;
                scrollPos = scrollPos > maxScrollPos ? maxScrollPos : scrollPos < minScrollPos ? minScrollPos : scrollPos;
            }

            scrollDelta = 0f;

            if (!interactable) { return; }

            float yPos = 0;

            if (!Hovering) { hoveredItem = -1; }

            bool mouseDown = Mouse.IsButtonPressed(Mouse.Button.Left);

            for (int i = 0; i < content.Count; i++)
            {
                Vector2 position = transform.WorldPosition + new Vector2(2.5f, yPos + (scrollPos + contentSpacing * i));

                bool inContentBoundsX =
                    mousePos.x >= position.x &&
                    mousePos.x <= position.x + transform.size.x;
                bool inContentBoundsY =
                    mousePos.y >= position.y &&
                    mousePos.y <= position.y + content[i].height;

                if (inContentBoundsX && inContentBoundsY && Hovering)
                {
                    hoveredItem = i;

                    if (hoveredItem != selectedItem && mouseDown)
                    {
                        selectedItem = hoveredItem;
                        OnContentSelected?.Invoke(this, content[selectedItem]);
                    }
                } else
                {
                    if(hoveredItem == i)
                    {
                        hoveredItem = -1;
                    }
                }

                yPos += content[i].height;
            }
        }

        public void AddContent(string str, float boxHeight, object? obj = null)
        {
            content.Add(new ScrollerContent(str, boxHeight, obj));

            minScrollPos = 0;

            float heightSum = -transform.size.y;
            for (int i = 0; i < content.Count; i++)
            {
                heightSum += content[i].height + contentSpacing;
            }

            heightSum += contentSpacing;

            minScrollPos = heightSum * -1f;
            minScrollPos = MathGE.Clamp(minScrollPos, -99999999999f, 0);
        }

        public void ResetScrollPosition()
        {
            scrollPos = maxScrollPos;
        }

        public void ClearContent(bool resetSelection = false)
        {
            if (resetSelection)
            {
                selectedItem = -1;
                hoveredItem = -1;
            }
            content.Clear();
        }

        RectangleShape rs = new RectangleShape(new Vector2(50, 50));
        public override void OnRender(RenderTarget rt)
        {
            panel.OnRender(rt);

            renderText.Clear(Color.Transparent);
            

            float yPos = 0;


            for (int i = 0; i < content.Count; i++)
            {
                rs.FillColor = ContentBackgroundColor;

                if(i == hoveredItem)
                {
                    rs.FillColor = defaultPrimary;
                }

                if (i == selectedItem)
                {
                    rs.FillColor = defaultPressed;
                }

                rs.OutlineColor = ContentOutlineColor;
                rs.OutlineThickness = 1f;
                rs.Size = new Vector2(transform.size.x - 5f, content[i].height);
                rs.Position = new SFML.System.Vector2f(2.5f, yPos + (scrollPos + contentSpacing * i));

                labelText.Position = (Vector2)rs.Position + new Vector2(5f, 0);

                labelText.CharacterSize = charSize;

                labelText.DisplayedString = content[i].content;

                labelText.OutlineThickness = 1f;
                labelText.OutlineColor = Color.Black;

                renderText.Draw(rs);
                renderText.Draw(labelText);
                yPos += content[i].height;
            }

            renderText.Display();
            spr.Texture = renderText.Texture;
            spr.Position = transform.WorldPosition;

            rt.Draw(spr);
        }

        public override void OnRemove()
        {
            renderText.Dispose();
        }
    }
}
