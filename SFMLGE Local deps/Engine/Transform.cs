﻿using SFML_Game_Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
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
                if (owner.parent != null)
                {
                    return _position + owner.parent.transform.WorldPosition;
                }
                return _position;
            }
            set
            {
                if (owner.parent != null)
                {
                    Vector2 abs = owner.parent.transform.WorldPosition - value;

                    _position = value - abs;
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