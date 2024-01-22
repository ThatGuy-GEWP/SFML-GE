using SFML.Graphics;

namespace SFML_Game_Engine
{
    /// <summary>
    /// Tells the renderer how to render IRenderables, see <see cref="IRenderable.QueueType"/>
    /// </summary>
    public enum RenderQueueType
    {
        /// <summary>
        /// Adds to the <see cref="RenderManager.renderQueue"/> queue, drawing in worldspace
        /// </summary>
        DefaultQueue,
        /// <summary>
        /// Adds to the <see cref="RenderManager.overlayQueue"/>, drawing in screenspace
        /// </summary>
        OverlayQueue
    }

    public interface IRenderable
    {
        public sbyte ZOrder { get; set; }

        public bool Visible { get; set; }

        public bool AutoQueue { get; set; }

        public RenderQueueType QueueType { get; set; }

        public void OnRender(RenderTarget rt);
    }
}
