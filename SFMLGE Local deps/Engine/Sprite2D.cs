using SFML.Graphics;


namespace SFML_Game_Engine
{
    /// <summary>
    /// A class for drawing rectangles.<para></para>
    /// </summary>
    internal class Sprite2D : Component, IRenderable
    {
        public Vector2 size = new Vector2(50, 50);

        public Vector2 origin = new Vector2(0.5f, 0.5f);

        public Vector2 offset = new Vector2(0.0f, 0.0f);

        /// <summary>
        /// If true, the <see cref="size"/> will match the <see cref="Texture"/>'s size
        /// </summary>
        public bool fitTexture = true;

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
        public TextureResource? Texture {
            get;
            set;
        }

        RectangleShape shape = new RectangleShape();

        public sbyte ZOrder { get; set; } = 5;
        public bool Visible { get; set; } = true;
        public bool AutoQueue { get; set; } = true;
        public RenderQueueType QueueType { get; set; } = RenderQueueType.DefaultQueue;

        public Sprite2D() { }

        public Sprite2D(float sizeX, float sizeY)
        {
            size = new Vector2(sizeX, sizeY);
        }

        public Sprite2D(Vector2 Size)
        {
            size = Size;
        }

        public Sprite2D(TextureResource Texture)
        {
            this.Texture = Texture;
            size = (Vector2)Texture.Resource.Size;
        }

        public Sprite2D(Vector2 Size, TextureResource Texture)
        {
            size = Size;
            this.Texture = Texture;
        }

        public Sprite2D(Vector2 Size, Vector2 Origin)
        {
            size = Size;
            this.origin = Origin;
        }

        public Sprite2D(Vector2 Size, Vector2 Origin, TextureResource Texture)
        {
            size = Size;
            this.origin = Origin;
            this.Texture = Texture;
        }

        public override void Start()
        {
            shape.Position = gameObject.WorldPosition;
            shape.Size = size;
            shape.Origin = size * origin;
        }

        public void OnRender(RenderTarget rt)
        {
            if (Texture is null)
            {
                Texture = project.GetResource<TextureResource>("DefaultSprite");
            }
            if (fitTexture) { size = new Vector2(Texture.Resource.Size.X, Texture.Resource.Size.Y); }
            shape.Texture = Texture.Resource;
            shape.TextureRect = new IntRect(0, 0, (int)Texture.Resource.Size.X, (int)Texture.Resource.Size.Y);
            shape.Position = gameObject.WorldPosition + offset;
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
