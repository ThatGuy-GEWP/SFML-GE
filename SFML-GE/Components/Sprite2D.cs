using SFML.Graphics;
using SFML_GE.Resources;
using SFML_GE.System;

namespace SFML_GE.Components
{
    /// <summary>
    /// A class for drawing rectangles.<para></para>
    /// </summary>
    public class Sprite2D : Component, IRenderable
    {
        /// <summary>
        /// The size of this Sprite2D in pixels.
        /// </summary>
        public Vector2 size = new Vector2(50, 50);

        /// <summary>
        /// The drawing origin of this Sprite2D, (0, 0) would draw
        /// the sprite from the left corner at <see cref="System.Transform.GlobalPosition"/>,
        /// (1, 1) would draw from the bottom right corner, and (0.5, 0.5) would draw the sprite centered.
        /// </summary>
        public Vector2 origin = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// A pixel offset to the sprite, added after <see cref="origin"/> is applied.
        /// </summary>
        public Vector2 offset = new Vector2(0.0f, 0.0f);

        /// <summary>
        /// A sub-section of the texture to render, in pixels.
        /// </summary>
        public IntRect TextureRect = new IntRect(0,0, 100, 100);

        /// <summary>
        /// If true, the <see cref="size"/> will match the <see cref="Texture"/>'s size, and so will its <see cref="TextureRect"/>
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
        public TextureResource? Texture
        {
            get;
            set;
        }

        RectangleShape shape = new RectangleShape();

        public int ZOffset { get; set; } = 0;
        public bool Visible { get; set; } = true;
        public bool AutoQueue { get; set; } = true;
        public RenderQueueType QueueType { get; set; } = RenderQueueType.DefaultQueue;

        /// <summary>
        /// Creates a new Sprite2D of size <paramref name="sizeX"/> and <paramref name="sizeY"/>
        /// </summary>
        /// <param name="sizeX">the size in the x direction</param>
        /// <param name="sizeY">the size in the y direction</param>
        public Sprite2D(float sizeX, float sizeY)
        {
            size = new Vector2(sizeX, sizeY);
            fitTexture = false;
            TextureRect = new IntRect(0, 0, (int)size.x, (int)size.y);
        }

        /// <summary>
        /// Creates a new Sprite2D of size <paramref name="Size"/>
        /// </summary>
        /// <param name="Size">The size of the Sprite2D</param>
        public Sprite2D(Vector2 Size)
        {
            size = Size;
            fitTexture = false;
            TextureRect = new IntRect(0, 0, (int)size.x, (int)size.y);
        }

        /// <summary>
        /// Creates a new Sprite2D using a given <see cref="TextureResource"/> <paramref name="Texture"/>.
        /// sets <see cref="fitTexture"/> to <c>true</c> which will auto fit the texture.
        /// </summary>
        /// <param name="Texture">The texture of this sprite</param>
        public Sprite2D(TextureResource Texture)
        {
            this.Texture = Texture;
            TextureRect = new IntRect(0, 0, (int)Texture.Resource.Size.X, (int)Texture.Resource.Size.Y);
            fitTexture = true;
            size = (Vector2)Texture.Resource.Size;
        }

        /// <summary>
        /// Creates a new Sprite2D using a given <see cref="TextureResource"/> <paramref name="Texture"/>,
        /// thats sized with <paramref name="Size"/>.
        /// </summary>
        /// <param name="Size">The size of this sprite</param>
        /// <param name="Texture">The texture of this sprite</param>
        public Sprite2D(Vector2 Size, TextureResource Texture)
        {
            size = Size;
            fitTexture = false;
            this.Texture = Texture;
            TextureRect = new IntRect(0, 0, (int)size.x, (int)size.y);
        }

        /// <summary>
        /// Creates a new Sprite2D of size <paramref name="Size"/> and with an origin of <paramref name="Origin"/>
        /// </summary>
        /// <param name="Size">The size of this Sprite2D</param>
        /// <param name="Origin">The Origin of this Sprite2D, where (0,0) is the top left, and (1, 1) is the bottom right</param>
        public Sprite2D(Vector2 Size, Vector2 Origin)
        {
            size = Size;
            fitTexture = false;
            origin = Origin;
            TextureRect = new IntRect(0, 0, (int)size.x, (int)size.y);
        }

        /// <summary>
        /// Creates a new Sprite2D of size <paramref name="Size"/> and with an origin of <paramref name="Origin"/>,
        /// and a texture set to <paramref name="Texture"/>
        /// </summary>
        /// <param name="Size">The size of this Sprite2D</param>
        /// <param name="Origin">The Origin of this Sprite2D, where (0,0) is the top left, and (1, 1) is the bottom right</param>
        /// <param name="Texture">The Texture of this Sprite2D</param>
        public Sprite2D(Vector2 Size, Vector2 Origin, TextureResource Texture)
        {
            size = Size;
            fitTexture = false;
            origin = Origin;
            this.Texture = Texture;
        }

        public override void Start()
        {
            shape.Position = gameObject.transform.GlobalPosition;
            shape.Size = size;
            shape.Origin = size * origin;
        }

        public void OnRender(RenderTarget rt)
        {
            if (Texture is null)
            {
                Texture = Project.GetResource<TextureResource>("DefaultSprite")!;
            }
            shape.Texture = Texture.Resource;
            shape.TextureRect = TextureRect;

            if (fitTexture)
            {
                shape.TextureRect = new IntRect(0, 0, (int)Texture.Resource.Size.X, (int)Texture.Resource.Size.Y);
                size = new Vector2(Texture.Resource.Size.X, Texture.Resource.Size.Y);
            }

            shape.Position = gameObject.transform.GlobalPosition + offset;
            shape.Size = size;
            shape.Origin = size * origin;

            shape.FillColor = fillColor;
            shape.OutlineColor = outlineColor;

            shape.OutlineThickness = outlineThickness;

            shape.Rotation = gameObject.transform.GlobalRotation;
            rt.Draw(shape);
        }
    }
}
