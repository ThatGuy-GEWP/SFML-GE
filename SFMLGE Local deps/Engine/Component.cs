

namespace SFML_Game_Engine
{
    /// <summary>
    /// A Base class all components derive from
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// The <see cref="GameObject"/> this component is attached too. null if this component is not added to anything yet
        /// </summary>
        public GameObject gameObject { 
            get { return _gameObject; }
            set { _gameObject = value; OnAdded(value); }
        }

        public float deltaTime
        {
            get { return scene.deltaTime; }
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
        /// The <see cref="Project"/> this component is within.
        /// Null on component creation.
        /// </summary>
        public Project project = null!; // null! to suppress warnings

        /// <summary>
        /// The <see cref="Scene"/> this component is within.
        /// Null on component creation.
        /// </summary>
        public Scene scene = null!; // null! to suppress warnings

        public virtual void Update() { return; }

        public virtual void Start()
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
