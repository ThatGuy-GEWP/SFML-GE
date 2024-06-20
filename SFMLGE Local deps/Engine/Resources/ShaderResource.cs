using SFML.Graphics;
using SFML_Game_Engine.System;

namespace SFML_Game_Engine.Resources
{
    /// <summary>
    /// A simple wrapper for SFML.Net Shaders, to set uniforms within shaders,
    /// use ShaderResource.Resource.SetUniform(uniformName, uniformValue)
    /// </summary>
    public class ShaderResource : Resource
    {
        /// <summary>
        /// The <see cref="Shader"/> this Resource contains
        /// </summary>
        public Shader Resource
        {
            get;
            set;
        }

        /// <summary> True if the <see cref="Shader"/> contains a Vertex Shader program in it. </summary>
        public bool containsVertex;

        /// <summary> True if the <see cref="Shader"/> contains a Geometry Shader program in it. </summary>
        public bool containsGeometry;

        /// <summary> True if the <see cref="Shader"/> contains a Fragment Shader program in it. </summary>
        public bool containsFragment;

        /// <summary>
        /// Creates a shader resource from file paths, pass null to any string to opt out of using it
        /// (i.e null for everything but fragment if your only using a fragment shader)
        /// </summary>
        public ShaderResource(string name, string? VertexShaderPath, string? GeometryShaderPath, string? FragmentShaderPath)
        {
            if (VertexShaderPath == null) { containsVertex = false; } else { containsVertex = true; }
            if (GeometryShaderPath == null) { containsGeometry = false; } else { containsGeometry = true; }
            if (FragmentShaderPath == null) { containsFragment = false; } else { containsFragment = true; }

            if (!containsVertex && !containsGeometry && !containsFragment)
            {
                throw new ArgumentException("A shader was created with no shader paths!");
            }

            Name = name;

            Resource = new Shader(VertexShaderPath, GeometryShaderPath, FragmentShaderPath);
        }

        // because its alot nicer to just pass a ShaderResource into App.draw(thing, renderStates)
        /// <summary>
        /// Creates a new <see cref="RenderStates"/> used to apply this <see cref="Shader"/>
        /// </summary>
        public static explicit operator RenderStates(ShaderResource shad) => new RenderStates(shad.Resource);

        /// <summary>
        /// Disposes of the <see cref="Shader"/> within this resource
        /// </summary>
        public override void Dispose()
        {
            Resource.Dispose();
        }
    }
}
