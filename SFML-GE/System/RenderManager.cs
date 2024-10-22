using SFML.Graphics;

namespace SFML_GE.System
{
    // Does not yet take advantage of the new ZTree, should fix later!

    /// <summary>
    /// Top level class that handles all rendering calls.
    /// Expects all <see cref="Component"/>'s given to implment <see cref="IRenderable"/>
    /// </summary>
    public class RenderManager
    {
        /// <summary>
        /// A List of <see cref="IRenderable"/>'s to be rendered next frame, Cleared after every <see cref="Render(RenderTarget)"/>
        /// </summary>
        List<Component> renderQueue = new List<Component>();

        /// <summary>
        /// A List of <see cref="IRenderable"/>'s to be rendered next frame, Cleared after every <see cref="Render(RenderTarget)"/>
        /// </summary>
        List<Component> overlayQueue = new List<Component>();

        internal RenderManager() { }

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

        /// <summary>
        /// Adds a <see cref="Action"/> that renders something to the <see cref="renderQueue"/>.
        /// This method is experimental and can be subject to change or removal in the future.
        /// </summary>
        /// <param name="renderAction">The action that will be called</param>
        /// <param name="offset">The ZOrder of the render call.</param>
        public void AddToQueue(Action<RenderTarget> renderAction, int offset)
        {
            ShadowComponent shadow = new ShadowComponent(renderAction, offset);
            renderQueue.Add(shadow);
        }

        /// <summary>
        /// Adds a <see cref="Action"/> that renders something to the <see cref="overlayQueue"/>.
        /// This method is experimental and can be subject to change or removal in the future.
        /// </summary>
        /// <param name="renderAction">The action that will be called</param>
        /// <param name="offset">The ZOrder of the render call.</param>
        public void AddToOverlayQueue(Action<RenderTarget> renderAction, int offset)
        {
            ShadowComponent shadow = new ShadowComponent(renderAction, offset);
            overlayQueue.Add(shadow);
        }

        static int ZSort(Component x, Component y)
        {
            int realXZ = 0;
            int realYZ = 0;

            if(x is ShadowComponent)
            {
                realXZ = (x as IRenderable)!.ZOffset;
            }
            else
            {
                realXZ = x.gameObject.ZOrder + (x as IRenderable)!.ZOffset;
            }
            if(y is ShadowComponent)
            {
                realYZ = (y as IRenderable)!.ZOffset;
            }
            else
            {
                realYZ = y.gameObject.ZOrder + (y as IRenderable)!.ZOffset;
            }

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
                    if (renderQueue[i] is ShadowComponent) { ((ShadowComponent)renderQueue[i]).OnRenderAction(target); continue; }
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
                    if (renderQueue[i] is ShadowComponent) { ((ShadowComponent)renderQueue[i]).OnRenderAction(target); continue; }
                    if (!((IRenderable)overlayQueue[i]).Visible) { continue; }
                    ((IRenderable)overlayQueue[i]).OnRender(target);
                }

                overlayQueue.Clear();
            }
        }
    }
}
