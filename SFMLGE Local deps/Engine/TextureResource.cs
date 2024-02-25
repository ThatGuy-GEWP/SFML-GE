using SFML.Graphics;

namespace SFML_Game_Engine
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
            base.name = name;
            Resource = text;
        }

        public TextureResource(string path, string name)
        {
            base.name = name;
            Resource = new Texture(path);
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
