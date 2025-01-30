using SFML.Audio;
using SFML.Graphics;
using SFML_GE.Resources;
using SFML_GE.System;

namespace SFML_GE.GUI
{
    /// <summary>
    /// A Panel, can be used to hold other GUI Components inside of it. also used as a base for every GUI Component
    /// </summary>
    public class GUIPanel : Component, IRenderable
    {
        // works for now, should be configurable later.
        internal static Color defaultForeground = new Color(0xE0EBF1FF);
        internal static Color defaultBackground = new Color(0x474859FF) - new Color(0, 0, 0, 125);

        internal static Color defaultSecondary = new Color(0x61637BFF);
        internal static Color defaultPrimary = new Color(0x767997FF);

        internal static Color defaultPressed = new Color(0x2C2D36FF);

        /// <summary>
        /// The background color of this GUI object
        /// </summary>
        public Color backgroundColor = defaultBackground;

        /// <summary>
        /// The outline color of this GUI object
        /// </summary>
        public Color outlineColor = defaultSecondary;

        /// <summary>Controls the thickness of the outline, values less then 0 will not render with rounded corners. 0 to disable the outline.</summary>
        public float outlineThickness = 2.0f;

        /// <summary>If false, outline corners will not be rounded.</summary>
        public bool roundedCorners = false;

        /// <summary> The texture displayed in the background of this <see cref="GUIPanel"/> </summary>
        public TextureResource panelContent = null!;

        /// <summary> The background panel rectangle from <see cref="GUIPanel"/>, can be
        /// altered right before its drawn in the <see cref="PrePass(RenderTarget, in Vector2, in Vector2)"/> </summary>
        protected RectangleShape backgroundPanelRect = new RectangleShape();

        RectangleShape outlineRect = new RectangleShape();

        /// <summary> controls whether or not the background from <see cref="GUIPanel"/> is drawn or not.</summary>
        protected bool renderBackgroundPanel = true;

        /// <summary> controls whether or not the outline from <see cref="GUIPanel"/> is drawn or not.</summary>
        protected bool renderOutline = true;

        // used to pre-render the corner texture, should only be generated once at the start of the engine if any GUIPanels are created.
        static Texture cornerText = null!;

        /// <summary>
        /// Controls where <see cref="Position"/> is. <para/> 0,0 would be the top left and 1,1 would be the bottom right
        /// </summary>
        public Vector2 Anchor = Vector2.zero;

        /// <summary>
        /// The position of this GUI object.
        /// </summary>
        public UDim2 Position = UDim2.zero;

        /// <summary>
        /// The size of this GUI object.
        /// </summary>
        public UDim2 Size = new UDim2(0.0f, 0.0f, 150f, 50f);

        GameObject? lastParent = null;
        GUIPanel? lastGUIPanel = null;

        public int ZOffset { get; set; } = 0;
        public bool Visible { get; set; } = true;
        public bool AutoQueue { get; set; } = true;
        public RenderQueueType QueueType { get; set; } = RenderQueueType.OverlayQueue;

        void generateCornerTexture()
        {
            RenderTexture rt = new RenderTexture(512, 512);
            CircleShape tempCircle = new CircleShape(512, 64);

            tempCircle.Position = new Vector2(512, 512);
            tempCircle.Origin = new Vector2(512, 512);

            rt.Draw(tempCircle);
            rt.Display();
            cornerText = new Texture(rt.Texture); // without this, rt.dispose would also take away the texture
            cornerText.Repeated = false;

            rt.Dispose();
            tempCircle.Dispose();
        }

        /// <summary>
        /// Internally updates the GUIPanel parent of this GUIPanel, if it has one.
        /// </summary>
        void ParentCheck() // prob increases performance by a bit!
        {
            if (lastParent != gameObject.Parent)
            {
                if (gameObject.Parent == null) { lastGUIPanel = lastGUIPanel == null ? lastGUIPanel : null; }
                lastGUIPanel = gameObject.Parent!.GetComponentOfSubclass<GUIPanel>();
                lastParent = gameObject.Parent;
            }
        }


        public GUIPanel()
        {
            if (cornerText == null)
            {
                generateCornerTexture();
            }
        }

        /// <summary>
        /// Creates a new <see cref="GUIPanel"/> thats transparent and has no border.
        /// Children will still be drawn correctly.
        /// </summary>
        /// <returns></returns>
        public static GUIPanel NewInvisiblePanel()
        {
            GUIPanel pan = new GUIPanel();
            pan.outlineThickness = 0;
            pan.backgroundColor = Color.Transparent;
            return pan;
        }

        public override void Start()
        {
            if (gameObject.ZOrder == 0)
            {
                if (gameObject.Parent == null) { return; }


                GUIPanel? panelParent = gameObject.Parent.GetComponentOfSubclass<GUIPanel>();
                if (panelParent != null)
                {
                    gameObject.ZOrder = (sbyte)(panelParent.gameObject.ZOrder + 1);
                }
            }
        }

        /// <summary>
        /// Gets the scalar used for <see cref="Size"/>'s <see cref="UDim2.GetVector(Vector2)"/>
        /// </summary>
        Vector2 ParentScale()
        {
            ParentCheck();

            if (lastGUIPanel != null)
            {
                return lastGUIPanel.GetSize();
            }

            return (Vector2)Project.RenderTargetSize;
        }

        /// <summary>
        /// Gets the bounds of the parent <see cref="GUIPanel"/>, or gets the screen size if there isnt a <see cref="GUIPanel"/> parent.
        /// </summary>
        /// <returns>A <see cref="BoundBox"/> sized to the parents bounds.</returns>
        protected BoundBox ParentBounds()
        {
            ParentCheck();

            if (lastGUIPanel != null)
            {
                return lastGUIPanel.GetBounds();
            }

            return new BoundBox(new FloatRect(0, 0, (int)Project.RenderTargetSize.x, (int)Project.RenderTargetSize.y));
        }

        /// <summary>
        /// Gets the current <see cref="GUIPanel"/> parent, if there is one.
        /// </summary>
        /// <returns>The <see cref="GUIPanel"/> parent if there is one, null otherwise</returns>
        protected GUIPanel? GUIParent()
        {
            ParentCheck();
            return lastGUIPanel;
        }

        /// <summary>
        /// Gets the base <see cref="GUIPanel"/> that holds this ones bounds. 
        /// </summary>
        protected BoundBox? ContainerBounds()
        {
            GUIPanel? foundPan = null;
            GUIPanel? lastPan = this;
            int searchIndx = 0;
            while (searchIndx < 100)
            {
                searchIndx++;
                GUIPanel? curPan = lastPan.GUIParent();
                if (curPan != null) // meaning we are at the top
                {
                    lastPan = curPan;
                }
                else { foundPan = lastPan; break; }
            }

            return foundPan?.GetBounds();
        }

        /// <summary>
        /// Gets the position of the Parent <see cref="GUIPanel"/>, returns <see cref="Vector2.zero"/> if there is no <see cref="GUIPanel"/> parent.
        /// </summary>
        Vector2 ParentPosition()
        {
            ParentCheck();

            if (lastGUIPanel != null)
            {
                return lastGUIPanel.GetPosition();
            }

            return Vector2.zero;
        }

        /// <summary>
        /// Gets the size of this <see cref="GUIPanel"/>
        /// </summary>
        public Vector2 GetSize()
        {
            return Size.GetVector(ParentScale());
        }

        /// <summary>
        /// Gets the position of this <see cref="GUIPanel"/> (relative to the <see cref="Anchor"/>)
        /// </summary>
        public Vector2 GetPosition()
        {
            Vector2 curSize = GetSize();
            Vector2 pos = ParentPosition() + Position.GetVector(ParentScale());

            Vector2 finalPos = pos - curSize * Anchor;

            return finalPos;
        }

        /// <summary>
        /// Gets the bounds of a <see cref="GUIPanel"/>.
        /// </summary>
        /// <returns></returns>
        public BoundBox GetBounds()
        {
            Vector2 curSize = GetSize();
            Vector2 pos = Position.GetVector(ParentScale()) + ParentPosition();

            return new BoundBox(new FloatRect(pos - curSize * Anchor, curSize));
        }

        RectangleShape recShape = new RectangleShape();

        // with this version of the corner and connectors code, if it aint broke dont fix it!
        /// <summary> Draws the corners of the outline </summary>
        void DrawCorners(Vector2 pos, Vector2 size, RenderTarget rt, float outlineThickness)
        {
            recShape.FillColor = outlineColor;
            recShape.Texture = cornerText;
            recShape.Size = new Vector2(outlineThickness, outlineThickness);

            recShape.Position = new Vector2(pos.x, pos.y);
            recShape.Origin = new Vector2(outlineThickness, outlineThickness);
            recShape.Rotation = 0f;
            rt.Draw(recShape);

            recShape.Position = new Vector2(pos.x + size.x, pos.y);
            recShape.Origin = new Vector2(outlineThickness, outlineThickness);
            recShape.Rotation = 90f;
            rt.Draw(recShape);

            recShape.Position = new Vector2(pos.x, pos.y + size.y);
            recShape.Origin = new Vector2(outlineThickness, outlineThickness);
            recShape.Rotation = -90f;
            rt.Draw(recShape);

            recShape.Position = new Vector2(pos.x + size.x, pos.y + size.y);
            recShape.Origin = new Vector2(outlineThickness, outlineThickness);
            recShape.Rotation = 180f;
            rt.Draw(recShape);
        }

        /// <summary> Draws the lines actually connecting the rounded corners </summary>
        void DrawCornerConnectors(Vector2 pos, Vector2 size, RenderTarget rt, float outlineThickness)
        {
            outlineRect.FillColor = outlineColor;

            outlineRect.Size = new Vector2(backgroundPanelRect.Size.X, outlineThickness);
            outlineRect.Position = pos - new Vector2(0, outlineThickness);
            rt.Draw(outlineRect);

            outlineRect.Size = new Vector2(backgroundPanelRect.Size.X, outlineThickness);
            outlineRect.Position = pos + new Vector2(0, size.y);
            rt.Draw(outlineRect);

            outlineRect.Size = new Vector2(outlineThickness, backgroundPanelRect.Size.Y);
            outlineRect.Position = pos - new Vector2(outlineThickness, 0);
            rt.Draw(outlineRect);

            outlineRect.Size = new Vector2(outlineThickness, backgroundPanelRect.Size.Y);
            outlineRect.Position = pos + new Vector2(size.x - 0, 0);
            rt.Draw(outlineRect);
        }

        public void OnRender(RenderTarget rt)
        {
            Vector2 pos = GetPosition();
            Vector2 size = GetSize();

            backgroundPanelRect.FillColor = backgroundColor;
            backgroundPanelRect.Size = size;
            backgroundPanelRect.Position = pos;

            if (panelContent != null)
            {
                backgroundPanelRect.Texture = panelContent.Resource;
            }

            PrePass(rt, ref pos, ref size);

            if (MathF.Abs(outlineThickness) != 0 && outlineColor.A > 0)
            {
                if (outlineThickness < 0 || !roundedCorners)
                {
                    backgroundPanelRect.OutlineColor = outlineColor;
                    backgroundPanelRect.OutlineThickness = outlineThickness;
                }
                else
                {
                    backgroundPanelRect.OutlineThickness = 0;

                    DrawCorners(pos, size, rt, outlineThickness);
                    DrawCornerConnectors(pos, size, rt, outlineThickness);
                }
            }

            rt.Draw(backgroundPanelRect);

            PostPass(rt);
        }

        /// <summary>
        /// Gets called before anything is drawn to the screen, allows you to change the position and size variables before being drawn by
        /// passing them as a refrence.
        /// </summary>
        /// <param name="rt">the render target this panel is being drawn too</param>
        /// <param name="pos">a refrence to the current position thats going to be used to draw.</param>
        /// <param name="size">a refrence to the current size thats going to be used to draw.</param>
        protected virtual void PrePass(RenderTarget rt, ref Vector2 pos, ref Vector2 size) { return; }

        /// <summary>
        /// Gets called after the base panel is drawn to the screen.
        /// </summary>
        protected virtual void PostPass(RenderTarget rt) { return; }
    }
}
