using SFML.Graphics;

namespace SFML_Game_Engine.GUI
{
    // Summary is a bit weird but should work for now
    /// <summary>
    /// Allows you to setup GUI content that can scroll.
    /// Any <see cref="GUIPanel"/>'s inside of this GUIScroller will be positioned relative to it,
    /// and can be scrolled through.
    /// </summary>
    public class GUIScroller : GUIPanel
    {
        public RenderTexture renderTarget = null!;

        readonly List<GUIPanel> embeddedChildren = new List<GUIPanel>();
        readonly List<UDim2> originalPositions = new List<UDim2>();

        public float scrollMultiplier = 5f;

        public float ScrollPosition { get; private set; } = 0;

        Vector2 lastSize = Vector2.zero;

        public GUIScroller() { }

        public override void Start()
        {
            base.Start();
        }

        static void SetupDescendants(GameObject of)
        {
            GameObject[] children = of.GetDescendants().ToArray();

            foreach (GameObject child in children)
            {
                GUIPanel? panel = child.GetComponentOfSubclass<GUIPanel>();
                if(panel != null)
                {
                    panel.Enabled = false;
                    panel.AutoQueue = false;
                }
            }
        }

        static void UpdateDescendants(GameObject of)
        {
            GameObject[] children = of.GetDescendants().ToArray();

            foreach (GameObject child in children)
            {
                GUIPanel? panel = child.GetComponentOfSubclass<GUIPanel>();
                if (panel != null)
                {
                    if(panel.Started == false) { continue; }
                    panel.Update();
                }
            }
        }

        static void DrawDecendants(GameObject of, RenderTarget rt)
        {
            GameObject[] children = of.GetDescendants().ToArray();

            List<GUIPanel> renderables = new List<GUIPanel>();

            foreach (GameObject child in children)
            {
                GUIPanel? panel = child.GetComponentOfSubclass<GUIPanel>();
                if (panel != null)
                {
                    renderables.Add(panel);
                }
            }

            renderables.Sort((x, y) => { return x.gameObject.ZOrder - y.gameObject.ZOrder; });

            foreach (IRenderable renderable in renderables)
            {
                renderable.OnRender(rt);
            }
        }

        public override void Update()
        {
            base.Update();

            if(GetSize() != lastSize)
            {
                lastSize = GetSize();
                renderTarget = new RenderTexture((uint)lastSize.x, (uint)lastSize.y);
            }

            for (int i = 0; i < embeddedChildren.Count; i++)
            {
                embeddedChildren[i].Position = originalPositions[i];
            }

            embeddedChildren.Clear();
            originalPositions.Clear();
            GameObject[] children = gameObject.GetChildren();
            for(int i = 0; i < children.Length; i++)
            {
                GUIPanel? panel = children[i].GetComponentOfSubclass<GUIPanel>();

                if (panel != null)
                {
                    embeddedChildren.Add(panel);
                    originalPositions.Add(panel.Position);
                    panel.Enabled = false;
                    ((IRenderable)panel).AutoQueue = false;
                    SetupDescendants(panel.gameObject);
                }
            }

            float contentSizeY = 0;

            for(int i = 0; i < embeddedChildren.Count; i++)
            {
                GUIPanel panel = embeddedChildren[i];

                if(panel.GetBounds().BottomLeft.y - GetBounds().Rect.Top > contentSizeY) { contentSizeY = panel.GetBounds().BottomLeft.y - GetBounds().Rect.Top; }
            }

            if (GetBounds().WithinBounds(Scene.GetMouseScreenPosition()))
            {
                ScrollPosition -= Project.ScrollDelta * scrollMultiplier;
            }

            float distToBottom = contentSizeY - ScrollPosition;

            if(ScrollPosition < 0)
            {
                ScrollPosition = 0;
            }

            if(distToBottom <= GetSize().y && contentSizeY > GetSize().y)
            {
                ScrollPosition = contentSizeY - GetSize().y;
            } else if(contentSizeY < GetSize().y) { ScrollPosition = 0; }

            //Console.WriteLine(scrollPosition);
            /*if (distToBottom <= GetSize().y)
            {
                
                Console.WriteLine(
                    "Reached! scrlPos:" + (scrollPosition) + 
                    " totalYSize:" + contentSizeY + 
                    " insideSize:" + GetSize().y + 
                    " dist to bot:" + distToBottom
                    );
            }*/

            for (int i = 0; i < embeddedChildren.Count; i++)
            {
                GUIPanel panel = embeddedChildren[i];
                if(panel.Started != true) { panel.gameObject.Update(); }
                panel.Position = new UDim2(panel.Position.scale, (originalPositions[i].offset + new Vector2(0, -ScrollPosition)));
                panel.Update();
                UpdateDescendants(panel.gameObject);
            }
        }

        Sprite drawSprite = new Sprite();
        protected override void PostPass(RenderTarget rt)
        {
            renderTarget.Clear(Color.Transparent);

            embeddedChildren.Sort((x, y) => { return x.gameObject.ZOrder - y.gameObject.ZOrder; });

            for (int i = 0; i < embeddedChildren.Count; i++)
            {
                GUIPanel panel = embeddedChildren[i];

                panel.Position = new UDim2(panel.Position.scale, (originalPositions[i].offset + new Vector2(0, -ScrollPosition)) - GetPosition());
                panel.OnRender(renderTarget);
                DrawDecendants(panel.gameObject, renderTarget);
            }

            renderTarget.Display();

            drawSprite.Texture = renderTarget.Texture;
            drawSprite.TextureRect = new IntRect(0, 0, (int)renderTarget.Size.X, (int)renderTarget.Size.Y);
            drawSprite.Position = GetPosition();

            rt.Draw(drawSprite);
        }
    }
}
