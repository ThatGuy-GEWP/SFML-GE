namespace SFML_Game_Engine
{
    public class GameObject
    {
        public string name;

        public bool started = false;

        public bool enabled = true;

        bool DestroyQueued = false;

        /// <summary>The <see cref="Project"/> this GameObject belongs too.</summary>
        public Project Project { get; private set; }

        /// <summary>The <see cref="Scene"/> this GameObject belongs too.</summary>
        public Scene Scene { get; private set; }

        /// <summary>The parent of this GameObject, null if attached to scene root.</summary>
        public GameObject? parent { get; private set; }

        /// <summary>The children of this GameObject</summary>
        public List<GameObject> Children { get; private set; } = new List<GameObject>();

        public List<Component> Components { get; private set; } = new List<Component>();

        Vector2 _position = new Vector2(0, 0);

        /// <summary>Position relative to world</summary>
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                foreach (var child in Children)
                {
                    child.Position += value - _position;
                }
                _position = value;
            }
        }

        /// <summary>Position relative to parent</summary>
        public Vector2 LocalPosition
        {
            get
            {
                return parent == null ? _position : _position - parent.Position;
            }
            set
            {
                if (parent == null) { Position = value; return; }
                Position = parent.Position + value;
            }
        }

        float _rotation = 0;

        /// <summary>Rotation relative to parent</summary>
        public float Rotation
        {
            get
            {
                return parent == null ? _rotation : _rotation + parent.Rotation;
            }
            set
            {
                if (value != _rotation)
                {
                    RotationChanged?.Invoke(this);
                    foreach (GameObject g in Children)
                    {
                        g.RotationChanged?.Invoke(g);
                    }
                }
                _rotation = value;
            }
        }

        /// <summary> Fires when this gameObjects position is changed, or the parents <see cref="Position"/> is changed.</summary>
        public event Action<GameObject> PositionChanged;

        /// <summary>Fires when this gameobjects rotation is changed, or the <see cref="parent"/>'s rotation is changed.</summary>
        public event Action<GameObject> RotationChanged;

        public GameObject(Project project, Scene scene)
        {
            name = "GameObject";
            parent = null;
            Project = project;
            Scene = scene;
            Position = new Vector2(0, 0);
        }

        public GameObject(Project project, Scene scene, GameObject parent)
        {
            this.parent = parent;
            Project = project;
            Scene = scene;
            Position = new Vector2(0, 0);
        }

        public GameObject CreateChild(string name)
        {
            return AddChild(new GameObject(Project, Scene));
        }

        public GameObject? GetDescendant(string name)
        {
            foreach (GameObject gm in Children)
            {
                if (gm.name == name) return gm;
                if (gm.Children.Count > 0) return gm.GetDescendant(name);
            }
            return null;
        }

        public void GetRenderables(RenderManager renderManager)
        {
            if (DestroyQueued) return;

            foreach (var comp in Components)
            {
                if(comp is IRenderable && ((IRenderable)comp).AutoQueue)
                {
                    renderManager.AddToRenderQueue((IRenderable)comp);
                }
            }

            foreach (var child in Children)
            {
                child.GetRenderables(renderManager);
            }
        }

        void StartComponent(Component comp)
        {
            if (comp.Started) { return; }
            comp.scene = Scene;
            comp.project = Scene.Project;

            comp.Start(Scene.Project, Scene);
            comp.Started = true;
        }

        public void Start()
        {
            if (started) return;
            foreach (var Comp in Components)
            {
                StartComponent(Comp);
            }

            foreach (var child in Children)
            {
                child.Start();
            }
        }

        /// <summary>
        /// Queues the gameObject for destruction on next update
        /// </summary>
        public void Destroy()
        {
            DestroyQueued = true;
        }

        public void Update()
        {
            if (DestroyQueued)
            {
                foreach(var comp in Components)
                {
                    comp.OnDestroy(this);
                    comp.Enabled = false;
                }
                Scene.GameObjects.Remove(this);
                enabled = false;
                return;
            }

            if (!enabled) return;
            foreach (var Comp in Components)
            {
                if (!Comp.Enabled) continue;

                if (!Comp.Started) {
                    StartComponent(Comp);
                    continue;
                }

                Comp.Update();
            }

            foreach (var child in Children)
            {
                child.Update();
            }
        }

        /// <summary>
        /// Adds a child to this gameObject, returns the child.
        /// If called twice with the same child, it will still return the child
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GameObject AddChild(GameObject go)
        {
            if(go == parent)
            {
                throw new ArgumentException(
                    $"Cannot add parent to self as a child! |!| {name} tried to add {go.name} as a child"
                    );
            }

            if (go.parent == this) { return go; }
            go.parent = this;
            Children.Add(go);
            return go;
        }

        public T AddComponent<T>(T comp) where T : Component
        {
            comp.gameObject = this;
            Components.Add(comp);
            return comp;
        }

        public T? GetComponent<T>() where T : Component
        {
            foreach (Component component in Components)
            {
                if (component.GetType() == typeof(T))
                {
                    return (T)component;
                }
            }
            return null;
        }
    }
}
