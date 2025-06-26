using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML_GE.Debugging;
using SFML_GE.Resources;
using System.Diagnostics;

namespace SFML_GE.System
{
    /// <summary>
    /// A Scene that is part of a <see cref="Project"/>.
    /// <para>
    /// Scenes hold <see cref="GameObject"/>'s inside of them and those GameObjects in turn have their own <see cref="Component"/>'s.
    /// Scenes are used to seperate diffrent stages or areas of a game, I.E you would have a Main Menu Scene and a Game Scene, and switch
    /// Between them when needed.
    /// </para>
    /// </summary>
    public class Scene
    {
        /// <summary>
        /// The name of this scene.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The <see cref="System.Project"/> this scene is within
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// The root GameObject that all other gameObjects are attached too.
        /// </summary>
        readonly GameObject root;

        /// <summary>
        /// If true, the scene has been started.
        /// </summary>
        public bool Started { get; private set; } = false;

        /// <summary>
        /// If true, the scene is currently loaded and running
        /// </summary>
        public bool IsLoaded { get; private set; } = false;

        /// <summary>
        /// The color used to clear the screen, if screen clearing is enabled.
        /// </summary>
        public Color ClearColor { get; set; } = Color.Black;

        /// <summary>
        /// If true, the screen will be cleared before rendering the frame.
        /// </summary>
        public bool ClearScreen { get; set; } = true;

        /// <summary>
        /// The <see cref="System.AudioManager"/> this scene controls
        /// </summary>
        public AudioManager AudioManager { get; private set; }

        /// <summary>
        /// The <see cref="System.RenderManager"/> this scene controls
        /// </summary>
        public RenderManager RenderManager { get; private set; } = new RenderManager();

        /// <summary>
        /// Called once the scene starts
        /// </summary>
        public event Action<Scene>? OnStart;

        /// <summary>
        /// A Shader thats applied to the scene when its rendered
        /// </summary>
        public ShaderResource? overlayShader = null;

        /// <summary>
        /// The <see cref="Camera"/> this scene is viewed from
        /// </summary>
        public Camera Camera { get; private set; }

        // This MIGHT be hacky AF but *might* be better then constantly re-sorting all game objects when a ZOrder is changed.
        /// <summary>
        /// A Dictionary containing all GameObjects at given ZOrder's.
        /// As an example, if you wanted to get all GameObjects at ZOrder 0, you would do:
        /// <code>
        /// // Write the names of all GameObjects at ZOrder 0
        /// if(Scene.ZTree.ContainsKey(0))
        /// {
        ///     foreach(GameObject gameObject in ZTree[0])
        ///     {
        ///         Console.WriteLine(gameObject.Name);
        ///     }
        /// }
        /// </code>
        /// This can be used to find objects above/below a given ZOrder
        /// </summary>
        public Dictionary<int, List<GameObject>> ZTree { get; private set; } = new Dictionary<int, List<GameObject>>();

        /// <summary>
        /// The time in seconds since the last frame
        /// </summary>
        public float DeltaTime { get; private set; } = 0;

        /// <summary>
        /// The component thats the mouse is currently over.
        /// Only works for components inheriting <see cref="IMouseBlockable"/>
        /// </summary>
        public Component? HoveredClickable { get { return mouseBlockManager.HoveredComponent; } }

        /// <summary>
        /// A HashSet containing all used names by gameObjects within this scene
        /// </summary>
        readonly HashSet<string> usedNames = new HashSet<string>();

        public float timeScale = 1f;

        readonly Stopwatch deltaWatch = new Stopwatch();

        readonly List<GameObject> gameObjects = new List<GameObject>();

        internal MouseBlockManager mouseBlockManager;

        /// <summary>
        /// Creates a new <see cref="Scene"/> and adds it to a given <paramref name="project"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="project"></param>
        public Scene(string name, Project project)
        {
            Name = name;
            Project = project;
            Camera = new Camera(project.App);
            AudioManager = new AudioManager(this);
            root = new GameObject(project, this);
            root.name = "ROOT";

            mouseBlockManager = new MouseBlockManager(this);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        ~Scene()
        {
            screenText?.Dispose();
            drawSprite.Dispose();
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Instances a given <paramref name="prefab"/>
        /// </summary>
        public GameObject? InstancePrefab(Prefab prefab)
        {
            GameObject? instance = prefab.CreatePrefab?.Invoke(Project, this);
            if (instance == null) { return null; }
            root.AddChild(instance);
            gameObjects.Add(instance);
            return instance;
        }

        /// <summary>
        /// Creates a new <see cref="GameObject"/> instance with a given <paramref name="name"/>
        /// </summary>
        public GameObject CreateGameObject(string name = "", GameObject? parent = null)
        {
            if (name == "" || name == null || name == string.Empty) { name = "Unnamed"; }
            if (parent == null) { parent = root; }

            GameObject go = new GameObject(Project, this, parent);
            RegisterGameObjectToScene(go, name);
            return go;
        }

        /// <summary>
        /// Creates a new <see cref="GameObject"/> and attaches a component to it.<para></para>
        /// Shorthand for <c>Scene.CreateGameObject().AddComponent</c>
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="comp">The component instance to add</param>
        /// <param name="name">The new GameObject's name</param>
        /// <param name="parent">The new Parent of the GameObject</param>
        /// <returns>A Component instance attached to a newly created GameObject</returns>
        public T CreateGameObjectWithComp<T>(T comp, string name = "", GameObject? parent = null) where T : Component
        {
            return CreateGameObject(name, parent).AddComponent(comp);
        }

        string GetValidName(string fromName)
        {
            if (usedNames.Contains(fromName))
            {
                for (int i = 1; i < int.MaxValue; i++)
                {
                    if (!usedNames.Contains(fromName + " " + i))
                    {
                        usedNames.Add(fromName + " " + i);
                        return fromName + " " + i;
                    }
                }
            }
            usedNames.Add(fromName);
            return fromName;
        }

        void RegisterGameObjectToScene(GameObject go, string name)
        {
            string finalName = GetValidName(name);
            go.name = finalName;
            gameObjects.Add(go);
        }

        /// <summary>
        /// Searches entire gameObject tree for child with the matching name,
        /// returns null if not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject? GetGameObject(string name)
        {
            foreach (GameObject gm in root.GetChildren())
            {
                if (gm.name == name)
                {
                    return gm;
                }

                GameObject? search = gm.GetDescendant(name);
                if (search != null)
                {
                    return search;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets an array of <see cref="GameObject"/>'s, and stopping after a given <paramref name="depth"/>
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public GameObject[] GetGameObjects(int depth = 0)
        {
            if (depth == 0)
            {
                return root.GetChildren();
            }

            List<GameObject> children = new List<GameObject>();

            GetObjectsAt(root, children, depth);

            return children.ToArray();
        }

        /// <summary>
        /// Updates a <see cref="GameObject"/>'s ZOrder in the <see cref="ZTree"/>
        /// </summary>
        internal void ZOrderGameObjectUpdate(GameObject go, int newZOrder, int oldZOrder)
        {
            if (ZTree.ContainsKey(newZOrder))
            {
                ZTree[newZOrder].Add(go);
                if (newZOrder == oldZOrder) { return; } else { ZTree[oldZOrder].Remove(go); }
            }
            else
            {
                ZTree.Add(newZOrder, new List<GameObject>());
                ZTree[newZOrder].Add(go);
                if (newZOrder == oldZOrder) { return; } else { ZTree[oldZOrder].Remove(go); }
            }
        }

        /// <summary>
        /// Gets all GameObjects from <paramref name="from"/>, and adds then to <paramref name="sofar"/>, stopping at <paramref name="depth"/>
        /// </summary>
        void GetObjectsAt(GameObject from, List<GameObject> sofar, int depth)
        {
            sofar.AddRange(from.GetChildren());
            if (depth > 0)
            {
                foreach (GameObject child in from.Children)
                {
                    if (child.Children.Count > 0)
                    {
                        GetObjectsAt(child, sofar, depth - 1);
                    }
                }
            }
        }

        public bool IsPaused { get; private set; } = false;

        /// <summary>
        /// Pauses this scene's deltaTime timer, and stops all update calls.
        /// </summary>
        public void Pause()
        {
            if(IsPaused) return;
            IsPaused = true;
            deltaWatch.Stop();
        }

        /// <summary>
        /// Resumes this scenes' deltaTime timer, and resumes all update calls.
        /// </summary>
        public void Resume()
        {
            if (!IsPaused) return;
            IsPaused = false;
            deltaWatch.Start();
        }

        /// <summary>
        /// Starts the scene and its gameObjects
        /// </summary>
        internal void Start()
        {
            if (!deltaWatch.IsRunning)
            {
                deltaWatch.Start();
            }
            root.Start();
            Started = true;

            OnStart?.Invoke(this);
        }

        /// <summary>
        /// Unloads the scene
        /// </summary>
        internal void UnloadScene()
        {
            deltaWatch.Stop();
            IsLoaded = false;
            AudioManager.OnUnload();

            root.OnUnload();
        }

        /// <summary>
        /// Loads the scene.
        /// </summary>
        internal void LoadScene()
        {
            IsLoaded = true;
            root.OnLoad();
        }

        /// <summary>
        /// Should NOT be called manually, use <see cref="Project.Update"/>
        /// </summary>
        internal void Update()
        {
            if (!IsLoaded) { return; }
            if (IsPaused) return;
            if (!Started) { Start(); return; }

            root.Update();

            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject go = gameObjects[i];
                if (go.ShouldCleanup == true)
                {
                    gameObjects.RemoveAt(i);
                    root.Children.Remove(go);
                    if (ZTree.ContainsKey(go.ZOrder)) { ZTree[go.ZOrder].Remove(go); }
                    usedNames.Remove(go.name);
                    i--;
                }
            }

            mouseBlockManager.Update();
            DeltaTime = (float)deltaWatch.Elapsed.TotalSeconds * timeScale;
            deltaWatch.Restart();
            AudioManager.Update();
            Camera.Update();
        }

        RenderTexture? screenText = null;
        Sprite drawSprite = new Sprite();
        internal void Render(RenderTarget rt)
        {
            if (!IsLoaded) { return; }

            if (screenText == null || (Vector2)screenText.Size != Project.RenderTargetSize)
            {
                screenText?.Dispose();
                screenText = new RenderTexture((uint)Project.RenderTargetSize.x, (uint)Project.RenderTargetSize.y);
                drawSprite.TextureRect = new IntRect(0, 0, (int)screenText.Size.X, (int)screenText.Size.Y);
            }

            if (ClearScreen) { screenText.Clear(ClearColor); }
            screenText.SetView(Camera.cameraView);

            GameObject[] curChildren = root.GetChildren();
            foreach (GameObject gm in curChildren)
            {
                gm.GetRenderables(RenderManager);
                gm.GetClickables(mouseBlockManager);
            }

            RenderManager.Render(screenText);

#if DEBUG
            Gizmo.RenderInternalCalls(screenText);
#endif

            View oldView = new View(screenText.GetView());
            screenText.SetView(screenText.DefaultView);

            RenderManager.RenderOverlay(screenText);


            screenText.Display();
            drawSprite.Texture = screenText.Texture;
            drawSprite.Position = new Vector2(0, 0);

            oldView = rt.GetView();
            rt.SetView(rt.DefaultView);
            if (overlayShader != null)
            {
                rt.Draw(drawSprite, (RenderStates)overlayShader);
            }
            else
            {
                rt.Draw(drawSprite);
            }
            rt.SetView(oldView);
            screenText.SetView(oldView);
        }

        /// <summary> Gets the mouse position in Screen space </summary>
        public Vector2 GetMouseScreenPosition()
        {
            return (Vector2)Mouse.GetPosition((RenderWindow)Project.App);
        }

        /// <summary> Gets the mouse position in World space </summary>
        public Vector2 GetMouseWorldPosition()
        {
            Vector2 pos = GetMouseScreenPosition();
            return (Vector2)Project.App.MapPixelToCoords((Vector2i)pos.Clamp(0, int.MaxValue), Camera.cameraView);
        }
    }

}
