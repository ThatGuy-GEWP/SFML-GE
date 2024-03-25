

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
        public GameObject gameObject
        {
            get { return _gameObject; }
            set { _gameObject = value; OnAdded(value); }
        }

        public float deltaTime
        {
            get { return Scene.deltaTime; }
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

        Project _project = null!;
        Scene _scene = null!;

        /// <summary>
        /// The <see cref="SFML_Game_Engine.Project"/> this component is within.
        /// Null on component creation.
        /// </summary>

        public Project Project {
            get { 
                if (_project == null) 
                {
                    throw new NullReferenceException("\n"+
                        "->  A component is trying to access Component.project before its initialized!"+"\n"+
                        " -> Component.project should only be accessed during and after Component.Start()"
                        );
                } 
                return _project; 
            } 
            set { _project = value; }
        }

        /// <summary>
        /// The <see cref="SFML_Game_Engine.Scene"/> this component is within.
        /// Null on component creation.
        /// </summary>
        public Scene Scene {
            get { return _scene; }
            set { _scene = value; }
        }

        public virtual void Update() { return; }

        public virtual void Start()
        {
            return;
        }
        /// <summary> Run when component is enabled after being disabled.</summary>
        public virtual void Awake() { return; }

        public virtual void OnAdded(GameObject gameObject) { return; }

        public virtual void OnDestroy(GameObject gameObject) { return; }

        public virtual void OnUnload() { return; }

        public virtual void OnLoad() { return; }
    }
}
