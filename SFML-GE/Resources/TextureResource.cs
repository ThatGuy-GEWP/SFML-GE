using SFML.Graphics;
using SFML_GE.System;

namespace SFML_GE.Resources
{
    /// <summary>
    /// A Resource representing a loaded <see cref="SFML.Graphics.Texture"/>
    /// </summary>
    public class TextureResource : Resource
    {
        /// <summary>
        /// Gets the underlying <see cref="SFML.Graphics.Texture"/>
        /// </summary>
        public Texture Resource
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new <see cref="TextureResource"/> from a given <see cref="SFML.Graphics.Texture"/> and a name
        /// </summary>
        /// <param name="text">The <see cref="SFML.Graphics.Texture"/> the resource will represent.</param>
        /// <param name="name">The (SHOULD BE UNIQUE) name of this resource</param>
        public TextureResource(Texture text, string name)
        {
            Name = name;
            Resource = text;
            Description = "path to: " + "Generated at Runtime.\n" + GetTextureInfo();
        }

        /// <summary>
        /// Loads a new <see cref="SFML.Graphics.Texture"/> from a file, then returns a new <see cref="TextureResource"/> from it.
        /// </summary>
        /// <param name="path">the relative or absolute path to the file to load.</param>
        /// <param name="name">The (SHOULD BE UNIQUE) name of this resource</param>
        public TextureResource(string path, string name)
        {
            Name = name;
            Resource = new Texture(path);
            Description = "path to: " + path + "\n" + GetTextureInfo();
        }

        /// <summary>
        /// Gets some info on the texture, used for debugging with the WIP editor.
        /// </summary>
        /// <returns></returns>
        string GetTextureInfo()
        {
            return
                "Texture Size:" + Resource.Size.X + "x" + Resource.Size.Y +
                "\nRepeated? " + Resource.Repeated +
                "\nSmooth? " + Resource.Smooth +
                "\nSRGB Enabled? " + Resource.Srgb;
        }

        /// <summary>
        /// Returns the name of this <see cref="Resource"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Disposes of the <see cref="SFML.Graphics.Texture"/> this resource represents.
        /// this WILL mess with anything still using the texture or expecting it.
        /// </summary>
        public override void Dispose()
        {
            Resource.Dispose();
        }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static implicit operator Texture(TextureResource resource)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return resource.Resource;
        }
    }
}
