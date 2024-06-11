using SFML.Graphics;
using SFML.Window;

namespace SFML_Game_Engine.GUI
{
    public class ScrollerEntry
    {
        public int YOrder;
        public float YSize;

        public bool clickable = true;

        public string DisplayedText = string.Empty;

        public Color FontColor = Color.White;

        public int xOffset = 0;

        public object? val;
        public Type? valType;

        /// <summary>
        /// Position of the text, from 0.0-1.0
        /// </summary>
        public Vector2 textPosition = new Vector2(0.5f, 0.5f);
        /// <summary>
        /// Anchor of the text, from 0.0-1.0
        /// </summary>
        public Vector2 textAnchor = new Vector2(0.5f, 0.5f);

        public ScrollerEntry() {}

        public ScrollerEntry(float ySize, string displayedText, int xOffset = 0)
        {
            this.YSize = ySize;
            this.DisplayedText = displayedText;
            this.xOffset = xOffset;
        }

        public ScrollerEntry(int yOrder, float ySize, bool clickable, string displayedText)
        {
            YOrder = yOrder;
            YSize = ySize;
            this.clickable = clickable;
            DisplayedText = displayedText;
        }
    }

    public class GUIList : GUIPanel
    {
        public List<ScrollerEntry> content = null!;

        public float entrySpacing = 2;

        public float scrollPos = 0f;

        public float scrollSpeed = 5f;

        Sprite spr = new Sprite();

        RenderTexture scrollTexture = null!;

        Vector2 lastSize = new Vector2(0, 0);

        public bool Hovering { get; private set; } = false;

        public int SelectedEntry { get; private set; } = -1;

        RectangleShape contentRect = new RectangleShape();

        Text contentText = new Text();

        public override void Start()
        {
            base.Start();
            contentText.Font = Project.GetResource<FontResource>("Roboto-Light");
            lastSize = GetSize();
            scrollTexture = new RenderTexture((uint)lastSize.x, (uint)lastSize.y);

            // REMOVE WHEN DONE!!!
            if(content == null) 
            {
                content = new List<ScrollerEntry>();

                for (int i = 0; i < 5; i++)
                {
                    content.Add(new ScrollerEntry(0, RandomGen.Next(15f, 45f), true, "Test!! " + i));
                }
            }
        }

        bool pressed = false;

        public override void Update()
        {
            Vector2 mousePos = Scene.GetMouseScreenPosition();
            BoundBox bounds = GetBounds();

            bool isMousePressed = Mouse.IsButtonPressed(Mouse.Button.Left);


            bool inXBounds =
                mousePos.x >= bounds.TopLeft.x &&
                mousePos.x <= bounds.BottomRight.x;
            bool inYBounds =
                mousePos.y >= bounds.TopLeft.y &&
                mousePos.y <= bounds.BottomRight.y;

            if(inXBounds && inYBounds)
            {
                Hovering = true;
            } else { Hovering = false; }

            if (GetSize() != lastSize)
            {
                lastSize = GetSize();
                scrollTexture.Dispose();
                scrollTexture = new RenderTexture((uint)lastSize.x, (uint)lastSize.y);
            }

            scrollTexture.Clear(Color.Transparent);

            int hoveredEntry = -1;

            float curY = entrySpacing*2;
            float totalSize = 0;
            for(int i = 0; i < content.Count; i++)
            {
                Vector2 contSize = new Vector2(lastSize.x - entrySpacing, content[i].YSize - entrySpacing);

                ScrollerEntry entry = content[i];

                contentRect.Origin = new Vector2(contSize.x / 2f, 0);

                contentRect.Position = new Vector2((lastSize.x * 0.5f) + (content[i].xOffset > 0 ? content[i].xOffset : 0), curY + scrollPos);
                contentRect.Size = new Vector2(contSize.x - MathF.Abs(content[i].xOffset), contSize.y);

                contentText.CharacterSize = (uint)Math.Round(contentRect.Size.Y / 2f);

                contentText.DisplayedString = content[i].DisplayedText;

                Vector2 centeredReg = new Vector2(contentText.GetGlobalBounds().Width, contentText.GetGlobalBounds().Height) * entry.textAnchor;
                Vector2 centerOffset = new Vector2(contentText.GetLocalBounds().Left, contentText.GetLocalBounds().Top);

                contentText.Origin = Vector2.Round(centeredReg + centerOffset);
                contentText.Position =
                    ((new Vector2(contentRect.Position.X - lastSize.x / 2, contentRect.Position.Y) + new Vector2(3, 3)) +
                    ((Vector2)contentRect.Size - new Vector2(6,6)) * entry.textPosition).Round();

                contentRect.FillColor = GUIPanel.defaultBackground;
                contentRect.OutlineColor = GUIPanel.defaultSecondary;

                contentRect.OutlineThickness = -2;

                curY += (content[i].YSize + entrySpacing);
                totalSize = curY;


                if (Hovering)
                {
                    Vector2 offsetMousePos = mousePos - bounds.TopLeft;
                    BoundBox contentBounds = new BoundBox(contentRect.GetGlobalBounds());

                    bool inContentXBounds =
                         offsetMousePos.x >= contentBounds.TopLeft.x &&
                         offsetMousePos.x <= contentBounds.BottomRight.x;
                    bool inContentYBounds =
                        offsetMousePos.y >= contentBounds.TopLeft.y &&
                        offsetMousePos.y <= contentBounds.BottomRight.y;

                    if(inContentXBounds && inContentYBounds)
                    {
                        hoveredEntry = i;
                        contentRect.FillColor = GUIPanel.defaultPrimary;
                    }
                    if (hoveredEntry == i && (isMousePressed && !pressed) && content[i].clickable) { SelectedEntry = i; }
                }

                if(i == SelectedEntry) { contentRect.FillColor = GUIPanel.defaultPressed; }

                scrollTexture.Draw(contentRect);
                contentText.FillColor = content[i].FontColor;
                scrollTexture.Draw(contentText);
            }

            float maxScroll = (GetSize().y - totalSize);
            float minScroll = 0;

            if (!Hovering && isMousePressed && SelectedEntry != -1) { SelectedEntry = -1; }

            if (Hovering)
            {
                if(Project.ScrollDelta != 0)
                {
                    scrollPos += Project.ScrollDelta * scrollSpeed;
                }
            }

            if(scrollPos < maxScroll)
            {
                scrollPos = maxScroll;
            }

            if(scrollPos > 0)
            {
                scrollPos = minScroll;
            }

            if (isMousePressed) { pressed = true; } else { pressed = false; }
        }

        protected override void PostPass(RenderTarget rt)
        {
            if(scrollTexture == null) { return; }

            scrollTexture.Display();
            spr.Texture = scrollTexture.Texture;
            spr.TextureRect = new IntRect((SFML.System.Vector2i)new Vector2(0,0), (SFML.System.Vector2i)scrollTexture.Size);
            spr.Position = GetPosition().Round();

            rt.Draw(spr);
        }
    }
}
