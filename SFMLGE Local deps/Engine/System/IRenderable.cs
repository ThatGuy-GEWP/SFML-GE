using SFML.Graphics;
using SFML_Game_Engine.Editor;
using SFML_Game_Engine.Engine.System;

namespace SFMLGE_Local_deps.Engine.System
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
        /// <summary>
        /// An additional offset to apply to the gameObject this component is attached too
        /// </summary>
        public int ZOffset { get; set; }

        /// <summary>
        /// If false, this renderable will not be drawn even if added to a queue.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// If true, this renderable will automatically be added to the current scenes <see cref="RenderManager.renderQueue"/>
        /// </summary>
        public bool AutoQueue { get; set; }

        /// <summary>
        /// Dictates which renderQueue this IRenderable will be added two when using <see cref="AutoQueue"/>
        /// </summary>
        public RenderQueueType QueueType { get; set; }

        public void OnRender(RenderTarget rt);
    }
}
