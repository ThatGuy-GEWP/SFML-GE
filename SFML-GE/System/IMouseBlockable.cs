namespace SFML_GE.System
{
    /// <summary>
    /// This interface is applied to objects to subscribe them to various
    /// depth tests, i.e when a user is clicking/hovering over something
    /// and we want to know if a component is behind another one
    /// <para></para>
    /// for example, <see cref="Scene.HoveredClickable"/> if not null will
    /// be the component (as an IMouseBlockable) thats being hovered.
    /// You could then check in your own code if that component is yours,
    /// if not then something is blocking the mouse!
    /// </summary>
    public interface IMouseBlockable
    {
        /// <summary>
        /// The boundBox that makes up this mouse blocker
        /// </summary>
        public BoundBox MouseBounds { get; }

        /// <summary>
        /// If false, the blocker will be ignored.
        /// </summary>
        public bool BlockingMouse { get; set; }
    }
}
