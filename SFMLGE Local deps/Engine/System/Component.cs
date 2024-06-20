namespace SFML_Game_Engine.Engine.System
{
    /// <summary>
    /// A Base class all components derive from
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// The <see cref="GameObject"/> this component is attached too. null if this component is not added to anything yet
        /// </summary>
        public GameObject gameObject = null!;

        public float DeltaTime
        {
            get { return Scene.DeltaTime; }
        }

        public bool Started
        {
            get { return _started; }
            set { _started = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { if (!_enabled && value) { Awake(); } _enabled = value; }
        }

        bool _started = false;
        bool _enabled = true;

        Project _project = null!;
        Scene _scene = null!;

        /// <summary>
        /// The <see cref="System.Project"/> this component is within.
        /// Null on component creation.
        /// </summary>
        public Project Project
        {
            get
            {
                if (_project == null)
                {
                    throw new NullReferenceException("\n" +
                        "->  A component is trying to access Component.Project before its initialized!" + "\n" +
                        " -> Component.project should only be accessed during and after Component.Start()"
                        );
                }
                return _project;
            }
            set { _project = value; }
        }

        /// <summary>
        /// The <see cref="System.Scene"/> this component is within.
        /// Null on component creation.
        /// </summary>
        public Scene Scene
        {
            get
            {
                if (_scene == null)
                {
                    throw new NullReferenceException("\n" +
                        "->  A component is trying to access Component.Scene before its initialized!" + "\n" +
                        " -> Component.Scene should only be accessed during and after Component.Start()"
                        );
                }
                return _scene;
            }
            set { _scene = value; }
        }

        /// <summary>Called every <see cref="Project.Update"/></summary>
        public virtual void Update() { return; }

        /// <summary>Called once the <see cref="System.Scene"/> this component is in has loaded.</summary>
        public virtual void Start() { return; }

        /// <summary> Called when a component is enabled after being disabled.</summary>
        public virtual void Awake() { return; }

        /// <summary> Called when a component is just added to a <see cref="GameObject"/>.</summary>
        public virtual void OnAdded(GameObject gameObject) { return; }

        /// <summary> Called when this Component or <see cref="gameObject"/> is destroyed.</summary>
        public virtual void OnDestroy(GameObject gameObject) { return; }

        /// <summary> Called when this <see cref="gameObject"/>'s scene is unloaded</summary>
        public virtual void OnUnload() { return; }

        /// <summary> Called when this <see cref="gameObject"/>'s scene is loaded</summary>
        public virtual void OnLoad() { return; }
    }
}
