using SFML.Graphics;

namespace SFML_Game_Engine
{
    /// <summary>
    /// Top level class that handles all rendering calls.
    /// allows for stuff like Z-Ordering and such.
    /// </summary>
    public class RenderManager
    {
        /// <summary>
        /// A List of <see cref="IRenderable"/>'s to be rendered next frame, Cleared after every <see cref="Render(RenderWindow)"/>
        /// </summary>
        List<IRenderable> renderQueue = new List<IRenderable>();

        public RenderManager() {}

        /// <summary>
        /// Adds an <see cref="IRenderable"/> to the renderQueue
        /// </summary>
        /// <param name="renderableComponent"></param>
        public void AddToRenderQueue(IRenderable renderableComponent)
        {
            renderQueue.Add(renderableComponent);
        }

        /// <summary>
        /// Renders all <see cref="IRenderable"/>'s after sorting them by zOrder,
        /// then clears the render queue.
        /// </summary>
        /// <param name="target"></param>
        public void Render(RenderTarget target)
        {
            if(renderQueue.Count > 0)
            {
                renderQueue.Sort((x, y) => { return x.ZOrder - y.ZOrder; });

                for (int i = 0; i < renderQueue.Count; i++)
                {
                    renderQueue[i].OnRender(target);
                }

                renderQueue.Clear();
            }
        }
    }
}
