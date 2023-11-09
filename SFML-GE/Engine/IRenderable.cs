using SFML.Graphics;

namespace SFML_GE
{
    public interface IRenderable
    {
        public byte zOrder { get; set; }

        public bool Visible { get; set; }

        public void OnRender(RenderTarget rt);
    }
}
