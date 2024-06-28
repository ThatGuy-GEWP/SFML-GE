using SFML_Game_Engine.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.System
{

    /// <summary>
    /// A class that defines how to position an object
    /// </summary>
    public class Transform
    {
        /// <summary>The owner of this transform</summary>
        public GameObject owner;

        /// <summary>The rotation of this transform</summary>
        public float rotation;

        Vector2 _position;

        /// <summary>The position of this transform relative to all parent's transforms</summary>
        public Vector2 WorldPosition
        {
            get
            {
                if (owner.Parent != null)
                {
                    return _position + owner.Parent.transform.WorldPosition;
                }
                return _position;
            }
            set
            {
                if (owner.Parent != null)
                {
                    Vector2 c = value - owner.Parent.transform.WorldPosition;
                    _position = c;
                }
                else
                {
                    _position = value;
                }
            }
        }

        /// <summary>
        /// The position of this transform relative to itself.
        /// </summary>
        public Vector2 LocalPosition
        {
            get { return _position; }
            set { _position = value; }
        }


        public Transform(GameObject owner)
        {
            this.owner = owner;
        }

    }
}
