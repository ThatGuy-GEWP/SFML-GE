﻿using SFML.Graphics;
using SFML.Window;

namespace SFML_Game_Engine.GUI
{
    struct ScrollerContent
    {
        public float height;
        public string content;

        public ScrollerContent(string content, float height)
        {
            this.content = content;
            this.height = height;
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


        float minScrollPos = -100f;
        float maxScrollPos = 5f;
        float scrollPos = 5f;

        float contentSpacing = 5f;

        float scrollSpeed = 8f;

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

            labelText = new Text("", RenderText.defaultFont);
        }

        float scrollDelta = 0f;


        bool setupevent = false;

        public override void Update()
        {
            if (!setupevent)
            {
                context.project.App.MouseWheelScrolled += (obj, args) =>
                {
                    scrollDelta = args.Delta;
                };
                setupevent = true;
            }


            Vector2 mousePos = context.scene.GetMouseScreenPosition();

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
        }

        public void AddContent(string str, float boxHeight)
        {
            content.Add(new ScrollerContent(str, boxHeight));

            minScrollPos = 0;

            float heightSum = 0;
            for (int i = 0; i < content.Count; i++)
            {
                heightSum += content[i].height + (contentSpacing * i);
            }

            heightSum += contentSpacing;
            heightSum -= transform.size.y;

            minScrollPos = heightSum * -1f;
            minScrollPos = MathGE.Clamp(minScrollPos, -99999999999f, 0);

            Console.WriteLine(minScrollPos);
        }

        RectangleShape rs = new RectangleShape(new Vector2(50, 50));
        public override void OnRender(RenderTarget rt)
        {
            panel.OnRender(rt);

            renderText.Clear(Color.Transparent);

            float yPos = 0;

            for (int i = 0; i < content.Count; i++)
            {
                rs.FillColor = defaultSecondary;
                rs.OutlineColor = defaultPrimary;
                rs.OutlineThickness = 1f;
                rs.Size = new Vector2(transform.size.x - 5f, content[i].height);
                rs.Position = new SFML.System.Vector2f(2.5f, yPos + (scrollPos + contentSpacing * i));

                labelText.Position = (Vector2)rs.Position + new Vector2(15f, 0);

                labelText.CharacterSize = 16;

                labelText.DisplayedString = content[i].content;

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
