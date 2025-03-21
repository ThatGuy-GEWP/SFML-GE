using SFML.Graphics;
using SFML.Window;
using SFML_GE.Resources;
using SFML_GE.System;

namespace SFML_GE.GUI
{
    /// <summary>
    /// An entry in a <see cref="GUIList"/>
    /// </summary>
    public class GUIListEntry
    {
        /// <summary>
        /// The order of this entry in the list.
        /// </summary>
        public int YOrder;

        /// <summary>
        /// The height of this entry in the list.
        /// </summary>
        public float YSize;

        /// <summary>
        /// if true, you can click on this entry.
        /// </summary>
        public bool clickable = true;

        /// <summary>
        /// the text displayed on this entry.
        /// </summary>
        public string DisplayedText = string.Empty;

        /// <summary>
        /// the color of the text for this entry.
        /// </summary>
        public Color FontColor = Color.White;

        /// <summary>
        /// an additional offset to the X position of the entry.
        /// </summary>
        public int xOffset = 0;

        /// <summary>
        /// the value this entry holds, if any.
        /// </summary>
        public object? val;

        /// <summary>
        /// the type of the value this entry holds, if any.
        /// </summary>
        public Type? valType;

        /// <summary>
        /// Position of the text, from 0.0-1.0
        /// </summary>
        public Vector2 textPosition = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// Anchor of the text, from 0.0-1.0
        /// </summary>
        public Vector2 textAnchor = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// Creates an empty entry.
        /// </summary>
        public GUIListEntry() { }

        /// <summary>
        /// Creates a new entry, does not add it the list.
        /// </summary>
        /// <param name="ySize">the height of this entry.</param>
        /// <param name="displayedText">the text displayed on this entry.</param>
        /// <param name="clickable">if true, the entry can be selected.</param>
        /// <param name="yOrder">the position in the list</param>
        /// <param name="xOffset">an offset to the x position of this entry.</param>
        public GUIListEntry(float ySize, string displayedText, bool clickable = true, int yOrder = 0, int xOffset = 0)
        {
            YSize = ySize;
            DisplayedText = displayedText;
            this.xOffset = xOffset;
        }
    }

    /// <summary>
    /// A GUIPanel that contains a list of <see cref="GUIListEntry"/>'s that can be selected and scrolled through.
    /// </summary>
    public class GUIList : GUIPanel
    {
        /// <summary>
        /// All <see cref="GUIListEntry"/>s in this <see cref="GUIList"/>
        /// </summary>
        public List<GUIListEntry> content = new List<GUIListEntry>();

        /// <summary>
        /// The pixel offset between each entry.
        /// </summary>
        public float entrySpacing = 2;

        /// <summary>
        /// the current scroll position of the list.
        /// </summary>
        public float scrollPos = 0f;

        /// <summary>
        /// how quickly you can scroll through the list.
        /// </summary>
        public float scrollSpeed = 5f;

        Sprite spr = new Sprite();

        RenderTexture scrollTexture = null!;

        Vector2 lastSize = new Vector2(0, 0);

        /// <summary>
        /// true if hovering over this lists <see cref="GUIPanel"/>
        /// </summary>
        public bool Hovering { get; private set; } = false;

        /// <summary>
        /// the <see cref="GUIListEntry"/> selected, or -1 if nothing is selected.
        /// </summary>
        public int SelectedEntry { get; private set; } = -1;

        RectangleShape contentRect = new RectangleShape();

        /// <summary>
        /// Called each time an entry is selected.
        /// </summary>
        public event Action<GUIListEntry> EntrySelected = null!;

        Text contentText = new Text();

        public override void Start()
        {
            base.Start();

            if(Project.GetResource<FontResource>(Project.GUIStyling.defaultFontName) != null)
            {
                contentText.Font = Project.GetResource<FontResource>(Project.GUIStyling.defaultFontName)!;
            }

            lastSize = GetSize();
            scrollTexture = new RenderTexture((uint)lastSize.x, (uint)lastSize.y);
        }

        /// <summary>
        /// Adds an entry to the <see cref="GUIList"/> then returns it.
        /// </summary>
        /// <param name="entry">The <see cref="GUIListEntry"/> to add.</param>
        public GUIListEntry AddEntry(GUIListEntry entry)
        {
            content.Add(entry);
            return entry;
        }

        /// <summary>
        /// Creates then adds a new <see cref="GUIListEntry"/> to the <see cref="GUIList"/>, then returns it.
        /// </summary>
        /// <param name="entryName">the name of the entry, will be displayed in the list.</param>
        public GUIListEntry AddEntry(string entryName)
        {
            GUIListEntry entr = new GUIListEntry(25, entryName, true);
            content.Add(entr);
            return entr;
        }

        bool pressed = false;

        public override void Update()
        {
            Vector2 mousePos = Scene.GetMouseScreenPosition();
            BoundBox bounds = GetBounds();

            bool isMousePressed = Mouse.IsButtonPressed(Mouse.Button.Left);

            if (bounds.WithinBounds(mousePos))
            {
                Hovering = true;
            }
            else { Hovering = false; }

            if (GetSize() != lastSize)
            {
                lastSize = GetSize();
                scrollTexture.Dispose();
                scrollTexture = new RenderTexture((uint)lastSize.x, (uint)lastSize.y);
            }

            scrollTexture.Clear(Color.Transparent);

            int hoveredEntry = -1;

            float curY = entrySpacing * 2;
            float totalSize = 0;
            for (int i = 0; i < content.Count; i++)
            {
                Vector2 contSize = new Vector2(lastSize.x - entrySpacing, content[i].YSize - entrySpacing);

                GUIListEntry entry = content[i];

                contentRect.Origin = new Vector2(contSize.x / 2f, 0);

                contentRect.Position = new Vector2(lastSize.x * 0.5f + (content[i].xOffset > 0 ? content[i].xOffset : 0), curY + scrollPos);
                contentRect.Size = new Vector2(contSize.x - MathF.Abs(content[i].xOffset), contSize.y);

                contentText.CharacterSize = (uint)Math.Round(contentRect.Size.Y / 2f);

                contentText.DisplayedString = content[i].DisplayedText;

                Vector2 centeredReg = new Vector2(contentText.GetGlobalBounds().Width, contentText.GetGlobalBounds().Height) * entry.textAnchor;
                Vector2 centerOffset = new Vector2(contentText.GetLocalBounds().Left, contentText.GetLocalBounds().Top);

                contentText.Origin = Vector2.Round(centeredReg + centerOffset);
                contentText.Position =
                    (new Vector2(contentRect.Position.X - lastSize.x / 2, contentRect.Position.Y) + new Vector2(3, 3) +
                    ((Vector2)contentRect.Size - new Vector2(6, 6)) * entry.textPosition).Round();

                contentRect.FillColor = defaultBackground;
                contentRect.OutlineColor = defaultSecondary;

                contentRect.OutlineThickness = -2;

                curY += content[i].YSize + entrySpacing;
                totalSize = curY;


                if (Hovering)
                {
                    Vector2 offsetMousePos = mousePos - bounds.TopLeft;
                    BoundBox contentBounds = new BoundBox(contentRect.GetGlobalBounds());

                    if (contentBounds.WithinBounds(offsetMousePos))
                    {
                        hoveredEntry = i;
                        contentRect.FillColor = defaultPrimary;
                    }
                    if (hoveredEntry == i && isMousePressed && !pressed && content[i].clickable) { SelectedEntry = i; EntrySelected?.Invoke(entry); }
                }

                if (i == SelectedEntry) { contentRect.FillColor = defaultPressed; }

                scrollTexture.Draw(contentRect);
                contentText.FillColor = content[i].FontColor;
                scrollTexture.Draw(contentText);
            }

            float maxScroll = GetSize().y - totalSize;
            float minScroll = 0;

            if (!Hovering && isMousePressed && SelectedEntry != -1) { SelectedEntry = -1; }

            if (Hovering)
            {
                if (Project.ScrollDelta != 0)
                {
                    scrollPos += Project.ScrollDelta * scrollSpeed;
                }
            }

            if (scrollPos < maxScroll)
            {
                scrollPos = maxScroll;
            }

            if (scrollPos > 0)
            {
                scrollPos = minScroll;
            }

            if (isMousePressed) { pressed = true; } else { pressed = false; }
        }

        protected override void PostPass(RenderTarget rt)
        {
            if (scrollTexture == null) { return; }

            scrollTexture.Display();
            spr.Texture = scrollTexture.Texture;
            spr.TextureRect = new IntRect((SFML.System.Vector2i)new Vector2(0, 0), (SFML.System.Vector2i)scrollTexture.Size);
            spr.Position = GetPosition().Round();

            rt.Draw(spr);
        }
    }
}
