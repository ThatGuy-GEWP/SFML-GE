using SFML.Graphics;
using SFML_GE.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SFML_GE.Debugging
{
    /// <summary>
    /// A Class containing a bunch of functions for drawing debugging info
    /// to the screen (lines, axis helpers, basic shapes, ect..)<para></para>
    /// Gizmo functions have to called constantly to be drawn to the screen,
    /// In either an update call or a render call.
    /// Gizmos will *not* be drawn before a project is made, and can only bind to one project at a time.
    /// Method calls will return if in release.<para></para>
    /// Gizmos will be drawn on top of the <see cref="RenderQueueType.DefaultQueue"/>, and under the
    /// <see cref="RenderQueueType.OverlayQueue"/>
    /// </summary>
    /// <remarks>Not thread safe, and might not be very fast if using lots of gizmos</remarks>
    public static class Gizmo
    {
        private static Project? LinkedProject;
        private static List<Action<RenderTarget>> GizmoCalls = new List<Action<RenderTarget>>();


        internal static void LinkToProject(Project proj)
        {
            LinkedProject = proj;
            
        }

        /// <summary>
        /// To be called from the LinkedProject's <see cref="RenderManager"/>
        /// </summary>
        /// <param name="to">Target to render to.</param>
        internal static void RenderInternalCalls(RenderTarget to)
        {
            if(LinkedProject == null) { GizmoCalls.Clear(); return; }
            for(int i = 0; i < GizmoCalls.Count; i++)
            {
                GizmoCalls[i].Invoke(to);
            }
            GizmoCalls.Clear();
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="startPoint">The starting point</param>
        /// <param name="endPoint">The ending point</param>
        public static void DrawLine(Vector2 startPoint, Vector2 endPoint)
        {
#if DEBUG
            if (LinkedProject == null) { return; }

            GizmoCalls.Add((rt) =>
            {
                rt.Draw(new Vertex[] { (Vertex)startPoint, (Vertex)endPoint }, PrimitiveType.Lines);
            });
#else
            return;
#endif
        }


        /// <summary>
        /// Draws two lines representing the X axis and Y axis
        /// </summary>
        /// <param name="atPoint">The point to draw at</param>
        /// <param name="rotation">The rotation of the axis</param>
        /// <param name="scale">The length of the axis lines</param>
        public static void DrawAxis(Vector2 atPoint, float rotation = 0f, float scale = 2.5f)
        {
#if DEBUG
            Vector2 up = Vector2.Rotate(new Vector2(0, -scale), rotation);
            up = atPoint + up;
            Vector2 right = Vector2.Rotate(new Vector2(scale, 0), rotation);
            right = atPoint + right;

            GizmoCalls.Add((rt) =>
            {
                rt.Draw(new Vertex[] { 
                    new Vertex(atPoint, Color.Red),
                    new Vertex(right, Color.Red)
                }, PrimitiveType.Lines);

                rt.Draw(new Vertex[] {
                    new Vertex(atPoint, Color.Green),
                    new Vertex(up, Color.Green)
                }, PrimitiveType.Lines);
            });
#else
            return
#endif
        }



    }
}
