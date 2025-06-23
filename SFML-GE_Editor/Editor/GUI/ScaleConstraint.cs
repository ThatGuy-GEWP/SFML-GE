using SFML_GE.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_GE_Editor.Editor.GUI
{
    /// <summary>
    /// Sets a gameObjects position relative to the size of the rendertarget, using scale and offset.
    /// </summary>
    public class ScaleConstraint : Component
    {
        /// <summary>
        /// the anchor of this constraint
        /// </summary>
        public Vector2 anchor = Vector2.zero;

        /// <summary>
        /// the scale of this constranint
        /// </summary>
        public Vector2 position_scale = new Vector2(0.5f);

        /// <summary>
        /// the size of the gameobject this constraint is moving
        /// </summary>
        public Vector2 objectSize = new Vector2(100);

        public override void Update()
        {
            Vector2 screenSize = Project.RenderTargetSize;

            Console.WriteLine(Project.RenderTargetSize);

            Vector2 position = screenSize * position_scale;

            Vector2 final_position = position + (objectSize * anchor);

            gameObject.transform.Position = final_position;
        }
    }
}
