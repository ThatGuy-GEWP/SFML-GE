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

        /// <summary>
        /// A List of <see cref="IRenderable"/>'s to be rendered next frame, Cleared after every <see cref="Render(RenderWindow)"/>
        /// </summary>
        List<IRenderable> overlayQueue = new List<IRenderable>();

        public RenderManager() { }

        /// <summary>
        /// Adds an <see cref="IRenderable"/> to the renderQueue
        /// </summary>
        /// <param name="renderableComponent"></param>
        public void AddToQueue(IRenderable renderableComponent)
        {
            renderQueue.Add(renderableComponent);
        }

        /// <summary>
        /// Adds an <see cref="IRenderable"/> to the GUI renderQueue
        /// </summary>
        /// <param name="renderableComponent"></param>
        public void AddToGUIQueue(IRenderable renderableComponent)
        {
            overlayQueue.Add(renderableComponent);
        }

        /// <summary>
        /// Renders all <see cref="IRenderable"/>'s after sorting them by ZOrder,
        /// then clears the render queue.
        /// </summary>
        /// <param name="target"></param>
        internal void Render(RenderTarget target)
        {
            if (renderQueue.Count > 0)
            {
                renderQueue.Sort((x, y) => { return x.ZOrder - y.ZOrder; });

                for (int i = 0; i < renderQueue.Count; i++)
                {
                    if (!renderQueue[i].Visible) { continue; }
                    renderQueue[i].OnRender(target);
                }

                renderQueue.Clear();
            }
        }

        /// <summary>
        /// Renders all <see cref="IRenderable"/>'s after sorting them by ZOrder,
        /// then clears the GUIrender queue.
        /// items in this queue get rebased to the default view,
        /// meaning if you draw a square at (0,0), it will be in the top left of the screen regardless of camera position.
        /// </summary>
        /// <param name="target"></param>
        internal void RenderOverlay(RenderTarget target)
        {
            if (overlayQueue.Count > 0)
            {
                overlayQueue.Sort((x, y) => { return x.ZOrder - y.ZOrder; });

                for (int i = 0; i < overlayQueue.Count; i++)
                {
                    if (!overlayQueue[i].Visible) { continue; }
                    overlayQueue[i].OnRender(target);
                }

                overlayQueue.Clear();
            }
        }
    }
}
