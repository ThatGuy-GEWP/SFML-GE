using SFML.Graphics;
using SFML_Game_Engine.System;

namespace SFML_Game_Engine.Resources
{
    public class TextureResource : Resource
    {
        public Texture Resource
        {
            get;
            set;
        }

        public TextureResource(Texture text, string name)
        {
            Name = name;
            Resource = text;
            Description = "path to: " + "Generated at Runtime.\n" + getTextureInfo();
        }

        public TextureResource(string path, string name)
        {
            Name = name;
            Resource = new Texture(path);
            Description = "path to: " + path + "\n" + getTextureInfo();
        }

        string getTextureInfo()
        {
            return
                "Texture Size:" + Resource.Size.X + "x" + Resource.Size.Y +
                "\nRepeated? " + Resource.Repeated +
                "\nSmooth? " + Resource.Smooth +
                "\nSRGB Enabled? " + Resource.Srgb;
        }

        public override string ToString()
        {
            return Name;
        }

        public override void Dispose()
        {
            Resource.Dispose();
        }

        public static implicit operator Texture(TextureResource resource)
        {
            return resource.Resource;
        }
    }
}
