﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_GE.System
{

    /// <summary>
    /// A class that defines how to position and rotate an object
    /// </summary>
    public class Transform
    {
        /// <summary>The owner of this transform</summary>
        public GameObject owner;

        /// <summary>The rotation of this transform</summary>
        public float rotation;

        Vector2 _position;

        /// <summary>The position of this transform relative to all parent's transforms</summary>
        public Vector2 GlobalPosition
        {
            get
            {
                if (owner.Parent != null)
                {
                    return _position + owner.Parent.transform.GlobalPosition;
                }
                return _position;
            }
            set
            {
                if (owner.Parent != null)
                {
                    Vector2 c = value - owner.Parent.transform.GlobalPosition;
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
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }


        /// <summary>
        /// Creates a new Transform and sets its owner to <paramref name="owner"/>
        /// </summary>
        /// <param name="owner">The owner of this Transform</param>
        public Transform(GameObject owner)
        {
            this.owner = owner;
        }

    }
}
