


using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Diagnostics;

namespace SFML_Game_Engine
{
    public class Scene
    {
        public string Name { get; set; }

        public Project Project { get; private set; }

        GameObject root;

        public bool started = false;

        public bool isLoaded { get; private set; } = false;

        public AudioManager audioManager { get; private set; }

        public RenderManager renderManager { get; private set; } = new RenderManager();

        public event Action<Scene>? OnStart;

        public ShaderResource? overlayShader = null;

        public Camera camera { get; private set; }

        // This MIGHT be hacky AF but *might* be better then constantly re-sorting all game objects when a ZOrder is changed.
        public Dictionary<int, List<GameObject>> ZTree { get; private set; } = new Dictionary<int, List<GameObject>>();

        Stopwatch deltaWatch = new Stopwatch();

        List<GameObject> gameObjects = new List<GameObject>();

        public float deltaTime { get; private set; } = 0;

        public Scene(string name, Project project)
        {
            this.Name = name;
            Project = project;
            camera = new Camera(project.App);
            audioManager = new AudioManager(this);
            root = new GameObject(project, this);
            root.name = "ROOT";
        }

        public GameObject InstancePrefab(Prefab prefab)
        {
            GameObject? instance = prefab.CreatePrefab?.Invoke(Project, this);
            if (instance == null) { return null; }
            root.AddChild(instance);
            gameObjects.Add(instance);
            return instance;
        }

        public GameObject CreateGameObject(string name = "")
        {
            if(name == "" || name == null || name == string.Empty) { name = "Unnamed"; }

            GameObject go = new GameObject(Project, this, root);
            root.AddChild(go);
            go.name = name;
            gameObjects.Add(go);
            return go;
        }

        public GameObject CreateGameObject(string name, GameObject parent)
        {
            if(parent == null) { return null; }
            if(parent.Scene != this) { return null; }

            if (name == "" || name == null || name == string.Empty) { name = "Unnamed"; }

            GameObject go = new GameObject(Project, this, parent);
            go.name = name;
            gameObjects.Add(go);
            return go;
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

        public GameObject[] GetGameObjects(int depth = 0)
        {
            if(depth == 0)
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
                if(newZOrder == oldZOrder) { return; } else { ZTree[oldZOrder].Remove(go); }
            } 
            else
            {
                ZTree.Add(newZOrder, new List<GameObject>());
                ZTree[newZOrder].Add(go);
                if (newZOrder == oldZOrder) { return; } else { ZTree[oldZOrder].Remove(go); }
            }
        }

        void GetObjectsAt(GameObject from, List<GameObject> sofar, int depth)
        {
            sofar.AddRange(from.GetChildren());
            if(depth > 0)
            {
                foreach(GameObject child in from.Children)
                {
                    if (child.Children.Count > 0)
                    {
                        GetObjectsAt(child, sofar, depth - 1);
                    }
                }
            }
        }

        public void Start()
        {
            if (!deltaWatch.IsRunning)
            {
                deltaWatch.Start();
            }
            root.Start();
            started = true;

            OnStart?.Invoke(this);
        }

        public void UnloadScene()
        {
            deltaWatch.Stop();
            isLoaded = false;
            audioManager.OnUnload();

            root.OnUnload();
        }

        public void LoadScene()
        {
            isLoaded = true;
            root.OnLoad();
        }

        public void Update()
        {
            if (!isLoaded) { return; }
            if (!started) { Start(); return; }

            root.Update();

            for(int i = 0; i < gameObjects.Count; i++)
            {
                GameObject go = gameObjects[i];
                if(go.ShouldCleanup == true)
                {
                    gameObjects.RemoveAt(i);
                    if (ZTree.ContainsKey(go.ZOrder)) { ZTree[go.ZOrder].Remove(go); }
                    i--;
                }
            }

            deltaTime = deltaWatch.ElapsedMilliseconds * 0.001f;
            deltaWatch.Restart();
            audioManager.Update();
            camera.Update();
        }

        RenderTexture? screenText = null;
        Sprite drawSprite = new Sprite();

        public void Render(RenderTarget rt)
        {
            if (!isLoaded) { return; }

            if(screenText == null || screenText.Size != Project.App.Size)
            {
                screenText?.Dispose();
                screenText = new RenderTexture(Project.App.Size.X, Project.App.Size.Y);
                drawSprite.TextureRect = new IntRect(0, 0, (int)screenText.Size.X, (int)screenText.Size.Y);
            }

            screenText.Clear();
            screenText.SetView(camera.cameraView);

            foreach (GameObject gm in root.GetChildren())
            {
                gm.GetRenderables(renderManager);
            }

            
            renderManager.Render(screenText);

            View oldView = new View(screenText.GetView());
            screenText.SetView(screenText.DefaultView);

            renderManager.RenderOverlay(screenText);

            screenText.Display();
            drawSprite.Texture = screenText.Texture;
            drawSprite.Position = new Vector2(0, 0);

            oldView = rt.GetView();
            rt.SetView(rt.DefaultView);
            if (overlayShader != null)
            {
                rt.Draw(drawSprite, overlayShader);
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
