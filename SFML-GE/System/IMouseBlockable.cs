using System;

namespace SFML_GE.System
{
    /// <summary>
    /// This interface should be applied to <see cref="Component"/>'s to subscribe them to various
    /// depth tests, i.e when a user is clicking/hovering over something
    /// and we want to know if a component is behind another one.
    /// <para/>uses <see cref="GameObject.ZOrder"/> (plus any offset from <see cref="IRenderable.ZOffset"/>) to know
    /// a <see cref="Component"/>'s depth.
    /// <para/>
    /// For example, <see cref="Scene.HoveredClickable"/> will
    /// be the <see cref="Component"/> (as an IMouseBlockable) thats being hovered (or null otherwise).
    /// You could then check if that component is yours,
    /// if not then something is blocking the mouse!
    /// </summary>
    public interface IMouseBlockable
    {
        /// <summary>
        /// The BoundBox that makes up this mouse blocker
        /// </summary>
        public BoundBox MouseBounds { get; }

        /// <summary>
        /// If false, the blocker will be ignored.
        /// </summary>
        public bool BlocksMouse { get; set; }
    }
}
