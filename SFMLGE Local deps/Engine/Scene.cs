


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

        public Camera camera { get; private set; }

        Stopwatch deltaWatch = new Stopwatch();

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
            return instance;
        }

        public GameObject CreateGameObject(string name)
        {
            GameObject go = new GameObject(Project, this, root);
            root.AddChild(go);
            go.name = name;
            return go;
        }

        public GameObject CreateGameObject(string name, GameObject parent)
        {
            if(parent == null) { return null; }
            if(parent.Scene != this) { return null; }

            GameObject go = new GameObject(Project, this, parent);
            go.name = name;
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

            deltaTime = deltaWatch.ElapsedMilliseconds * 0.001f;
            deltaWatch.Restart();
            audioManager.Update();
            camera.Update();
        }

        public void Render(RenderTarget rt)
        {
            if (!isLoaded) { return; }

            foreach (GameObject gm in root.GetChildren())
            {
                gm.GetRenderables(renderManager);
            }

            renderManager.Render(rt);

            View oldView = new View(rt.GetView());
            rt.SetView(rt.DefaultView);

            renderManager.RenderOverlay(rt);

            rt.SetView(oldView);
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
