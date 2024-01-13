

namespace SFML_Game_Engine
{
    /// <summary>
    /// Base class all components derive from, anything that requires access to the <see cref="Project"/> or the <see cref="Scene"/>,
    /// should be done in <see cref="Start(Project, Scene)"/>
    /// if you need to access these variables in update, use <see cref="project"/>, and <see cref="project.Scene"/>
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// The gameObject this component is attached too. null if not yet added.
        /// </summary>
        public GameObject gameObject { 
            get { return _gameObject; }
            set { _gameObject = value; OnAdded(value); }
        }

        public bool Started
        {
            get { return _started; }
            set { _started = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; if (value) { Awake(); } }
        }

        bool _started = false;
        bool _enabled = true;

        GameObject? _gameObject;

        /// <summary>
        /// The <see cref="project"/> this component is within.
        /// Null on component creation.
        /// </summary>
        public Project project;

        /// <summary>
        /// The <see cref="scene"/> this component is within.
        /// Null on component creation.
        /// </summary>
        public Scene scene;

        public virtual void Update() { return; }

        public virtual void Start(Project project, Scene scene)
        {
            return;
        }
        /// <summary> Run when component is enabled after being disabled.</summary>
        public virtual void Awake() { return; }

        public virtual void OnAdded(GameObject gameObject) { return; }

        public virtual void OnDestroy(GameObject gameObject) { return; }

        public virtual void OnUnload(GameObject gameObject) { return; }

        public virtual void OnLoad(GameObject gameObject) { return; }
    }
}
