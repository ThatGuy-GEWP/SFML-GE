using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.GUI
{
    /// <summary>
    /// A Transform special made for GUIComponents.
    /// </summary>
    public class GUITransform
    {
        public GUITransform? parent;

        public Vector2 size = new Vector2(1, 1);

        public Vector2 origin = Vector2.zero;

        public sbyte zOrder = 0;

        Vector2 _position = Vector2.zero;

        /// <summary>The position of this transform relative to all parent's transforms</summary>
        public Vector2 WorldPosition
        {
            get
            {
                if (parent != null)
                {
                    return (_position + parent.WorldPosition) - (size * origin);
                }
                return _position - (size * origin);
            }
            set
            {
                if(parent != null)
                {
                    Vector2 c = value - parent.WorldPosition;
                    _position = c;
                }
                else
                {
                    _position = value;
                }
            }
        }

        /// <summary> The position of this transform relative to itself. </summary>
        public Vector2 LocalPosition
        {
            get { return _position; }
            set { _position = value; }
        }


        public GUITransform() {}

        public GUITransform(GUITransform parent)
        {
            this.parent = parent;
        }
    }
}
