namespace SFML_Game_Engine
{
    public class GameObject
    {
        public string name;

        public bool started = false;

        public bool enabled = true;

        /// <summary>Indicates to the current <see cref="Scene"/> that this gameobject is fully destroyed.</summary>
        public bool ShouldCleanup { get; private set; }

        bool DestroyQueued = false;

        /// <summary>The <see cref="SFML_Game_Engine.Project"/> this GameObject belongs too.</summary>
        public Project Project { get; private set; }

        /// <summary>The <see cref="SFML_Game_Engine.Scene"/> this GameObject belongs too.</summary>
        public Scene Scene { get; private set; }

        /// <summary>The parent of this GameObject, null if attached to scene root.</summary>
        public GameObject? Parent { get; private set; }

        /// <summary>The children of this GameObject</summary>
        public List<GameObject> Children { get; private set; } = new List<GameObject>();

        public List<Component> Components { get; private set; } = new List<Component>();

        int _zOrder = 0;

        /// <summary>Controls how GameObjects will be drawn on top of each other, and which ones are closest to the screen </summary>
        public int ZOrder { get { return _zOrder; } set { if (value != _zOrder) { ZOrderChanged(value); } } }

        public Transform transform;

        /// <summary>
        /// if true, when <see cref="Position"/> is changes, children will be moved.
        /// </summary>
        public bool moveChildren = true;

        /// <summary>
        /// <see cref="Scene.CreateGameObject(string)"/> should be used instead.
        /// </summary>
        public GameObject(Project project, Scene scene, GameObject? parent = null)
        {
            name = "GameObject";
            if (parent != null) { parent.AddChild(this); } else { parent = null; }
            Project = project;
            Scene = scene;
            _zOrder = 0;
            ZOrderChanged(0);
            transform = new Transform(this);
        }

        void ZOrderChanged(int to)
        {
            Scene.ZOrderGameObjectUpdate(this, to, _zOrder);
            _zOrder = to;
        }


        public GameObject CreateChild(string name)
        {
            return AddChild(new GameObject(Project, Scene));
        }

        /// <summary>
        /// Gets all children and Descendents of children from a gameobject
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetDescendants()
        {
            List<GameObject> foundChildren = new List<GameObject>();
            foundChildren.AddRange(Children);

            foreach (GameObject gm in Children)
            {
                foundChildren.AddRange(gm.GetDescendants());
            }

            return foundChildren;
        }

        /// <summary>
        /// Gets a Descendent that matches the <paramref name="name"/> given.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject? GetDescendant(string name)
        {
            foreach (GameObject gm in Children)
            {
                if (gm.name == name) return gm;
                if (gm.Children.Count > 0) return gm.GetDescendant(name);
            }
            return null;
        }

        /// <summary>
        /// Collects all <see cref="IRenderable"/>s from a gameobject, and puts them in the correct queue 
        /// (only if <see cref="IRenderable.AutoQueue"/> and <see cref="IRenderable.Visible"/> is true)
        /// </summary>
        /// <param name="renderManager"></param>
        public void GetRenderables(RenderManager renderManager)
        {
            if (DestroyQueued) return;

            for (int i = 0; i < Components.Count; i++)
            {
                Component comp = Components[i];
                if (!comp.Started) { continue; }

                if (comp as IRenderable != null && typeof(IRenderable).IsAssignableFrom(comp.GetType()))
                {
                    IRenderable renderComp = (IRenderable)comp;
                    if (renderComp.AutoQueue == true)
                    {
                        switch (renderComp.QueueType) // switch just felt right here idk why
                        {
                            case RenderQueueType.OverlayQueue:
                                renderManager.AddToOverlayQueue(comp);
                                continue;
                            case RenderQueueType.DefaultQueue:
                                renderManager.AddToQueue(comp);
                                continue;
                        }
                    }
                }
            }

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].GetRenderables(renderManager);
            }
        }

        void StartComponent(Component comp)
        {
            comp.Scene = Scene;
            comp.Project = Scene.Project;
            if (comp.Started) { return; }

            comp.Start();
            comp.Started = true;
        }

        public void Start()
        {
            if (started) return;
            for (int i = 0; i < Components.Count; i++)
            {
                StartComponent(Components[i]);
            }

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Start();
            }
        }

        /// <summary>
        /// Queues the gameObject for destruction on next update
        /// </summary>
        public void Destroy()
        {
            DestroyQueued = true;
        }

        /// <summary> Forces a GameObject to finalize itself for cleanup. </summary>
        protected void DestroyNow()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].OnDestroy(this);
                Components[i].Enabled = false;
            }
            Components.Clear();

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].DestroyNow();
            }
            Children.Clear();
            ShouldCleanup = true;
            enabled = false;
        }

        public void Update()
        {
            if (!enabled) return;

            if (DestroyQueued) // queued so stuff does not get removed mid update.
            {
                DestroyNow(); 
                return;
            }

            for (int i = 0; i < Components.Count; i++)
            {
                if (!Components[i].Started)
                {
                    StartComponent(Components[i]);
                    continue;
                }

                if (!Components[i].Enabled) continue;

                Components[i].Update();
            }

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Update();

                if (Children[i].DestroyQueued)
                {
                    Children.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Called everytime the scene this gameObject is in is unloaded.
        /// </summary>
        public void OnUnload()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].OnUnload();
            }

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].OnUnload();
            }
        }

        /// <summary>
        /// Called everytime the scene this gameObject is in is loaded.
        /// </summary>
        public void OnLoad()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].OnLoad();
            }

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].OnLoad();
            }
        }

        /// <summary>
        /// Adds a child to this gameObject, returns the child.
        /// If called twice with the same child, it will still return the child
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public GameObject AddChild(GameObject child)
        {
            if (child == Parent)
            {
                throw new ArgumentException(
                    $"Cannot add parent to self as a child! |!| \"{name}\" tried to add \"{child.name}\" as a child"
                    );
            }

            if (child.Parent == this) { return child; }
            
            if(child.Parent != null)
            {
                if (child.Parent.HasChild(child))
                {
                    if(child.Parent.RemoveChild(child) == null)
                    {
                        throw new Exception($"Could not remove child \"{child.name}\" from old parent \"{child.Parent.name}\"!");
                    }
                }
            }

            child.Parent = this;
            Children.Add(child);
            return child;
        }

        public GameObject[] GetChildren()
        {
            return Children.ToArray();
        }

        public GameObject? GetChild(string name)
        {
            foreach (GameObject child in Children)
            {
                if(child.name == name) { return child; }
            }
            return null;
        }
        
        public bool HasChild(string name)
        {
            foreach (GameObject child in Children)
            {
                if (child.name == name) { return true; }
            }
            return false;
        }

        public bool HasChild(GameObject child)
        {
            foreach (GameObject schild in Children)
            {
                if(schild == child) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Removes then returns removed child, null if child was not removed
        /// </summary>
        /// <param name="name">name of the child</param>
        public GameObject? RemoveChild(string name)
        {
            GameObject? child = GetChild(name);
            if(child != null)
            {
                if (Children.Remove(child))
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// Removes then returns removed child, null if child was not removed
        /// </summary>
        /// <param name="name">name of the child</param>
        public GameObject? RemoveChild(GameObject child)
        {
            if (Children.Remove(child))
            {
                return child;
            }
            return null;
        }

        public T AddComponent<T>(T comp) where T : Component
        {
            comp.gameObject = this;
            Components.Add(comp);
            return comp;
        }

        /// <summary>
        /// Returns the first component that is a <typeparamref name="T"/> type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the first component that is a subclass of the type <typeparamref name="T"/> or is a <typeparamref name="T"/> itself.
        /// <para>See <see cref="GetComponentOfSubclassOnly{T}"/> to get a class that inherits <typeparamref name="T"/> but isnt an instance of <typeparamref name="T"/> itself.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetComponentOfSubclass<T>() where T : Component
        {
            foreach (Component component in Components)
            {
                if (component.GetType().IsSubclassOf(typeof(T)) || component.GetType() == typeof(T))
                {
                    return (T)component;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the first component that is a subclass of <typeparamref name="T"/> ONLY. <para></para>
        /// will not return an instance of <typeparamref name="T"/> itself, only classes that inherit from it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetComponentOfSubclassOnly<T>() where T : Component
        {
            foreach (Component component in Components)
            {
                if (component.GetType().IsSubclassOf(typeof(T)))
                {
                    return (T)component;
                }
            }
            return null;
        }
    }
}
