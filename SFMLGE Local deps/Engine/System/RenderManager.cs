using SFML.Graphics;
using SFMLGE_Local_deps.Engine.System;

namespace SFML_Game_Engine.Engine.System
{
    // Does not yet take advantage of the new ZTree, should fix later!

    /// <summary>
    /// Top level class that handles all rendering calls.
    /// Expects all <see cref="Component"/>'s given to implment <see cref="IRenderable"/>
    /// </summary>
    public class RenderManager
    {
        /// <summary>
        /// A List of <see cref="IRenderable"/>'s to be rendered next frame, Cleared after every <see cref="Render(RenderWindow)"/>
        /// </summary>
        List<Component> renderQueue = new List<Component>();

        /// <summary>
        /// A List of <see cref="IRenderable"/>'s to be rendered next frame, Cleared after every <see cref="Render(RenderWindow)"/>
        /// </summary>
        List<Component> overlayQueue = new List<Component>();

        public RenderManager() { }

        /// <summary>
        /// Adds a <see cref="Component"/> to <see cref="renderQueue"/>
        /// </summary>
        /// <param name="renderableComponent"></param>
        public void AddToQueue(Component renderableComponent)
        {
            if (!typeof(IRenderable).IsAssignableFrom(renderableComponent.GetType()))
            {
                throw new ArgumentException(renderableComponent.GetType().FullName + " does not implment the IRenderable interface.");
            }
            renderQueue.Add(renderableComponent);
        }

        /// <summary>
        /// Adds a <see cref="Component"/> to <see cref="overlayQueue"/>
        /// </summary>
        /// <param name="renderableComponent"></param>
        public void AddToOverlayQueue(Component renderableComponent)
        {
            if (!typeof(IRenderable).IsAssignableFrom(renderableComponent.GetType()))
            {
                throw new ArgumentException(renderableComponent.GetType().FullName + " does not implment the IRenderable interface.");
            }
            overlayQueue.Add(renderableComponent);
        }

        static int ZSort(Component x, Component y)
        {
            int realXZ = x.gameObject.ZOrder + (x as IRenderable)!.ZOffset;
            int realYZ = y.gameObject.ZOrder + (y as IRenderable)!.ZOffset;
            if (realXZ == realYZ) { return 0; }
            if (realXZ < realYZ)
            {
                return -1;
            }
            else
            {
                return 1;
            }
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
                renderQueue.Sort(ZSort);

                for (int i = 0; i < renderQueue.Count; i++)
                {
                    if (!((IRenderable)renderQueue[i]).Visible) { continue; }
                    ((IRenderable)renderQueue[i]).OnRender(target);
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
                overlayQueue.Sort(ZSort);

                for (int i = 0; i < overlayQueue.Count; i++)
                {
                    if (!((IRenderable)overlayQueue[i]).Visible) { continue; }
                    ((IRenderable)overlayQueue[i]).OnRender(target);
                }

                overlayQueue.Clear();
            }
        }
    }
}
