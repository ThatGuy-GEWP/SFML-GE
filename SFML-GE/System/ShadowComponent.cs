using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_GE.System
{
    /// <summary>
    /// A Component thats not attached to a real GameObject, 
    /// used internally for <see cref="RenderManager"/> delegate calls that arent binded to a component.
    /// </summary>
    internal class ShadowComponent : Component, IRenderable, IDisposable
    {
        public int ZOffset { get; set; } = 0;
        public bool Visible { get; set; } = true;
        public bool AutoQueue { get; set; } = false;
        public RenderQueueType QueueType { get; set; } = RenderQueueType.DefaultQueue;

        public void Dispose()
        {
            return;
        }

        public Action<RenderTarget> OnRenderAction;

        public ShadowComponent(Action<RenderTarget> onRenderAction, int ZOrder)
        {
            OnRenderAction = onRenderAction;
            ZOffset = ZOrder;
        }

        public void OnRender(RenderTarget rt)
        {
            throw new NotImplementedException();
        }
    }
}
