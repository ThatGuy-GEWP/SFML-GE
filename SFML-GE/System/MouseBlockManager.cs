using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_GE.System
{
    internal class MouseBlockManager
    {
        List<Component> componentsToCheck;

        internal Component? HoveredComponent = null;

        Scene scene;

        internal MouseBlockManager(Scene scn)
        {
            this.scene = scn;
            componentsToCheck = new List<Component>();
        }


        int ZSort(Component x, Component y)
        {
            int aZ = x.gameObject.ZOrder;
            if(x is IRenderable renderableA)
            {
                aZ += renderableA.ZOffset;
            }
            

            int bZ = y.gameObject.ZOrder;
            if (y is IRenderable renderableB)
            {
                bZ += renderableB.ZOffset;
            }

            if(aZ == bZ) { return 0; }
            if(aZ < bZ) { return 1; }
            return -1;
        }

        internal void QueueComp(Component comp)
        {
            componentsToCheck.Add(comp);
        }

        internal void Update()
        {
            componentsToCheck.Sort(ZSort);

            Vector2 WorldMousePos = scene.GetMouseWorldPosition();
            Vector2 ScreenMousePos = scene.GetMouseScreenPosition();

            HoveredComponent = null;

            for(int i = 0; i < componentsToCheck.Count; i++)
            {
                IMouseBlockable blockr = ((componentsToCheck[i]) as IMouseBlockable)!;

                int trueZ = componentsToCheck[i].gameObject.ZOrder + ((componentsToCheck[i] is IRenderable rendz) ? rendz.ZOffset : 0);

                if ((componentsToCheck[i] is IRenderable rend) && rend.QueueType == RenderQueueType.OverlayQueue)
                {
                    if (blockr.MouseBounds.WithinBounds(ScreenMousePos))
                    {
                        HoveredComponent = componentsToCheck[i];
                        break;
                    }
                }
                else
                {
                    if (blockr.MouseBounds.WithinBounds(WorldMousePos))
                    {
                        HoveredComponent = componentsToCheck[i];
                        break;
                    }
                }
            }

            componentsToCheck.Clear();
        }

    }
}
