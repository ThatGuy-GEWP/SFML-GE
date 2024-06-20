using SFML.Graphics;
using SFML_Game_Engine.Engine.System;

namespace SFML_Game_Engine
{
    /// <summary>
    /// A Resource containing a <see cref="Font"/>
    /// </summary>
    public class FontResource : Resource
    {
        /// <summary>
        /// The <see cref="Font"/> this resource contains.
        /// </summary>
        public Font resource;

        /// <summary>
        /// Creates a new <see cref="FontResource"/> from a given <paramref name="filePath"/> and <paramref name="name"/>
        /// </summary>
        /// <param name="filePath">the path to the <see cref="Font"/></param>
        /// <param name="name">the name of this resource</param>
        public FontResource(string filePath, string name)
        {
            this.resource = new Font(filePath);
            this.Name = name;
        }

        public override void Dispose()
        {
            return;
        }

        /// <summary>
        /// Converts a <see cref="FontResource"/> into a <see cref="Font"/> implicitly
        /// </summary>
        public static implicit operator Font(FontResource resource) { return resource.resource; }
    }
}
