using SFML.Graphics;
using SFML_Game_Engine;

namespace SFMLGE_Local_deps.Engine
{
    /// <summary>
    /// A simple wrapper for SFML.Net Shaders, to set uniforms within shaders,
    /// use ShaderResource.Resource.SetUniform(uniformName, uniformValue)
    /// </summary>
    public class ShaderResource : Resource
    {
        public Shader Resource {
            get;
            set;
        }

        public bool containsVertex;
        public bool containsGeometry;
        public bool containsFragment;


        /// <summary>
        /// Creates a shader resource from file paths, pass null to any string to opt out of using it
        /// (i.e null for everything but fragment if your only using a fragment shader)
        /// </summary>
        /// <param name="VertexShaderPath"></param>
        /// <param name="GeometryShaderPath"></param>
        /// <param name="FragmentShaderPath"></param>
        public ShaderResource(string name, string VertexShaderPath, string GeometryShaderPath, string FragmentShaderPath)
        {
            if(VertexShaderPath == null) { containsVertex = false; } else { containsVertex = true; }
            if (GeometryShaderPath == null) { containsGeometry = false; } else { containsGeometry = true; }
            if (FragmentShaderPath == null) { containsFragment = false; } else { containsFragment = true; }
            
            this.name = name;

            Resource = new Shader(VertexShaderPath, GeometryShaderPath, FragmentShaderPath);
        }

        public static implicit operator RenderStates(ShaderResource shad) => new RenderStates(shad.Resource);

        public override void Dispose()
        {
            Resource.Dispose();
        }
    }
}
