using SFML.Graphics;


namespace SFML_Game_Engine
{
    /// <summary>
    /// A class for drawing rectangles.<para></para>
    /// </summary>
    internal class Sprite2D : Component, IRenderable
    {
        public Vector2 size = new Vector2(50,50);

        public Vector2 origin = new Vector2(0.5f,0.5f);

        public Vector2 offset = new Vector2(0.0f, 0.0f);

        /// <summary>
        /// The fill color of this rectangle.
        /// </summary>
        public Color fillColor = Color.White;

        /// <summary>
        /// The color of the outline for this rectangle.
        /// </summary>
        public Color outlineColor = Color.White;

        /// <summary>
        /// The outline thickness of this rectangle.
        /// </summary>
        public float outlineThickness = 0;

        /// <summary>
        /// The texture of this rectangle, when set the texture will be stretched to this rects <see cref="size"/>, defaults to a white texture.
        /// </summary>
        public TextureResource? texture = null;

        RectangleShape shape = new RectangleShape();

        public sbyte zOrder { get; set; } = 5;
        public bool Visible { get; set; } = true;
        byte IRenderable.ZOrder { get; set; }
        public bool AutoQueue { get; set; } = true;

        public Sprite2D() { }

        public Sprite2D(Vector2 Size)
        {
            size = Size * 2;
        }

        public Sprite2D(Vector2 Size, TextureResource Texture)
        {
            size = Size * 2;
            texture = Texture;
        }

        public Sprite2D(Vector2 Size, Vector2 Origin)
        {
            size = Size * 2;
            this.origin = Origin;
        }

        public Sprite2D(Vector2 Size, Vector2 Origin, TextureResource Texture)
        {
            size = Size * 2;
            this.origin = Origin;
            texture = Texture;
        }

        public override void Start(Project project, Scene scene)
        {
            shape.Position = gameObject.Position;
            shape.Size = size;
            shape.Origin = size * origin;
        }

        public void OnRender(RenderTarget rt)
        {
            if (texture is null)
            {
                texture = project.Resources.GetResourceByName<TextureResource>("DefaultSprite");
            }

            shape.Texture = texture.resource;
            shape.Position = gameObject.Position;
            shape.Size = size;
            shape.Origin = size * origin;
            shape.FillColor = fillColor;
            shape.OutlineColor = outlineColor;
            shape.OutlineThickness = outlineThickness;
            shape.Rotation = gameObject.Rotation;
            rt.Draw(shape);
        }
    }
}
