using SFML.Graphics;

namespace SFML_Game_Engine
{
    public class FontResource : Resource
    {
        public Font resource;

        public FontResource(string filePath, string name)
        {
            this.resource = new Font(filePath);
            this.name = name;
        }

        public override void Dispose()
        {
            return;
        }

        public static implicit operator Font(FontResource resource) { return resource.resource; }
    }
}
