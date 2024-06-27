


using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML_Game_Engine.Resources;
using SFML_Game_Engine.System;
using System.Diagnostics;
using System.Xml.Linq;

namespace SFML_Game_Engine.System
{
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
        public Dictionary<int, List<GameObject>> ZTree { get; private set; } = new Dictionary<int, List<GameObject>>();

        readonly Stopwatch deltaWatch = new Stopwatch();

        readonly List<GameObject> gameObjects = new List<GameObject>();

        /// <summary>
        /// A HashSet containing all used names by gameObjects within this scene
        /// </summary>
        readonly HashSet<string> usedNames = new HashSet<string>();

        /// <summary>
        /// The time in seconds since the last frame
        /// </summary>
        public float DeltaTime { get; private set; } = 0;

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
        }

        /// <summary>
        /// Instances a given <paramref name="prefab"/>
        /// </summary>
        public GameObject InstancePrefab(Prefab prefab)
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

        string GetValidName(string fromName)
        {
            if(usedNames.Contains(fromName))
            {
                for(int i = 1; i < int.MaxValue; i++)
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
        public void ZOrderGameObjectUpdate(GameObject go, int newZOrder, int oldZOrder)
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

        /// <summary>
        /// Starts the scene and its gameObjects
        /// </summary>
        public void Start()
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
        public void UnloadScene()
        {
            deltaWatch.Stop();
            IsLoaded = false;
            AudioManager.OnUnload();

            root.OnUnload();
        }

        public void LoadScene()
        {
            IsLoaded = true;
            root.OnLoad();
        }

        public void Update()
        {
            if (!IsLoaded) { return; }
            if (!Started) { Start(); return; }

            root.Update();

            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObject go = gameObjects[i];
                if (go.ShouldCleanup == true)
                {
                    gameObjects.RemoveAt(i);
                    if (ZTree.ContainsKey(go.ZOrder)) { ZTree[go.ZOrder].Remove(go); }
                    usedNames.Remove(go.name);
                    i--;
                }
            }

            DeltaTime = deltaWatch.ElapsedMilliseconds * 0.001f;
            deltaWatch.Restart();
            AudioManager.Update();
            Camera.Update();
        }

        RenderTexture? screenText = null;
        Sprite drawSprite = new Sprite();

        public void Render(RenderTarget rt)
        {
            if (!IsLoaded) { return; }

            if (screenText == null || screenText.Size != Project.App.Size)
            {
                screenText?.Dispose();
                screenText = new RenderTexture(Project.App.Size.X, Project.App.Size.Y);
                drawSprite.TextureRect = new IntRect(0, 0, (int)screenText.Size.X, (int)screenText.Size.Y);
            }

            screenText.Clear();
            screenText.SetView(Camera.cameraView);

            GameObject[] curChildren = root.GetChildren();
            foreach (GameObject gm in curChildren)
            {
                gm.GetRenderables(RenderManager);
            }


            RenderManager.Render(screenText);

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
            return (Vector2)Mouse.GetPosition(Project.App);
        }

        /// <summary> Gets the mouse position in World space </summary>
        public Vector2 GetMouseWorldPosition()
        {
            Vector2 pos = GetMouseScreenPosition();
            return Project.App.MapPixelToCoords((Vector2i)pos);
        }

    }

}
