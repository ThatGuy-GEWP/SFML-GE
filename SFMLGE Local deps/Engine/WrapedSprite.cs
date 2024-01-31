using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;

namespace SFML_Game_Engine
{
    /// <summary>
    /// Just a more useful version of <see cref="Sprite">
    /// </summary>
    public class WrapedSprite
    {
        public Vector2 position = new Vector2(0, 0);
        public Vector2 scale = new Vector2(1, 1);
        public Vector2 origin = new Vector2(0, 0);
        public Color color = Color.White;

        Sprite sprite = new Sprite();
        public Texture texture;

        public Sprite Sprite
        {
            get 
            {
                sprite.Position = position;
                sprite.Scale = scale;
                sprite.Color = color;

                if (texture != null) { sprite.Texture = texture; }

                sprite.Origin = new Vector2(sprite.GetLocalBounds().Width, sprite.GetLocalBounds().Height) * origin;

                return sprite; 
            }
        }

        public WrapedSprite()
        {
            this.texture = null!; // sure why not
        }
        public WrapedSprite(TextureResource texture)
        {
            this.texture = texture;
        }

        public static implicit operator Sprite(WrapedSprite sprite)
        {
            sprite.sprite.Position = sprite.position;
            sprite.sprite.Scale = sprite.scale;
            sprite.sprite.Color = sprite.color;

            if(sprite.texture != null) { sprite.sprite.Texture = sprite.texture; }

            sprite.sprite.Origin = (new Vector2(sprite.sprite.GetLocalBounds().Width, sprite.sprite.GetLocalBounds().Height)) * sprite.origin;

            return sprite.sprite;
        }
    }
}
