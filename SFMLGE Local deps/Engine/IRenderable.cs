using SFML.Graphics;

namespace SFML_Game_Engine
{
    public interface IRenderable
    {
        public byte ZOrder { get; set; }

        public bool Visible { get; set; }

        public bool AutoQueue { get; set; }

        public void OnRender(RenderTarget rt);
    }
}
