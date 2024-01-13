

using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFMLGE_Local_deps.Engine;
using System.Diagnostics;

namespace SFML_Game_Engine
{
    public class Scene
    {
        public string Name { get; set; }

        public Project Project { get; private set; }

        public List<GameObject> GameObjects = new List<GameObject>();

        public bool started = false;

        public bool isLoaded { get; private set; } = false;

        public AudioManager audioManager { get; private set; }

        public Camera camera { get; private set; }

        RenderManager renderManager = new RenderManager();

        Stopwatch deltaWatch = new Stopwatch();

        public float deltaTime { get; private set; } = 0;

        public Scene(string name, Project project)
        {
            this.Name = name;
            Project = project;
            camera = new Camera(project.App);
            audioManager = new AudioManager(this);
        }

        public GameObject InstanciatePrefab(Prefab prefab)
        {
            GameObject? instance = prefab.CreatePrefab?.Invoke(Project, this);
            if(instance == null) { return null; }
            GameObjects.Add(instance);

            return instance;
        }

        public GameObject CreateGameObject(string name)
        {
            GameObject go = new GameObject(Project, this);
            GameObjects.Add(go);
            go.name = name; 
            return go;
        }

        /// <summary>
        /// Searches entire gameObject tree for child with the matching name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject? GetGameObject(string name)
        {
            foreach (GameObject gm in GameObjects)
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

        public void Start()
        {
            deltaWatch.Start();
            foreach (var gameObject in GameObjects)
            {
                if(gameObject.started) { continue; }
                gameObject.Start();
                gameObject.started = true;
            }
            started = true;
        }

        public void UnloadScene()
        {
            deltaWatch.Stop();
            isLoaded = false;
        }

        public void LoadScene()
        {
            if (!deltaWatch.IsRunning && started)
            {
                deltaWatch.Start();
            }
            isLoaded = true;
        }

        public void Update()
        {
            if(!isLoaded) { return; }

            if(!started) { Start(); return; }

            for(int i = 0; i < GameObjects.Count; i++)
            {
                GameObject gameObject = GameObjects[i];
                if (!gameObject.enabled) continue;
                if (!gameObject.started) { gameObject.Start(); gameObject.started = true; continue; }
                gameObject.Update();
            }
            deltaTime = deltaWatch.ElapsedMilliseconds * 0.001f;
            deltaWatch.Restart();
            audioManager.Update();
            camera.Update();
        }

        public void Render(RenderTarget rt)
        {
            if (!isLoaded) { return; }

            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].GetRenderables(renderManager);
            }

            renderManager.Render(rt);
        }

        /// <summary> Gets the mouse position in Screen space </summary>
        public Vector2 GetMouseWindowPosition()
        {
            return (Vector2)Mouse.GetPosition(Project.App);
        }

        /// <summary> Gets the mouse position in World space </summary>
        public Vector2 GetMousePosition()
        {
            return Project.App.MapPixelToCoords((Vector2i)GetMouseWindowPosition());
        }

    }
}
