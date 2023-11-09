

using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Diagnostics;

namespace SFML_GE
{
    public class Scene
    {
        public string Name { get; set; }

        public Project Project { get; private set; }

        public List<GameObject> GameObjects = new List<GameObject>();

        RenderManager renderManager = new RenderManager();

        Stopwatch deltaWatch = new Stopwatch();

        public float deltaTime { get; private set; } = 0;

        public Scene(string name, Project project)
        {
            this.Name = name;
            Project = project;
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
                gameObject.Start();
                gameObject.started = true;
            }
        }

        public void Update()
        {
            for(int i = 0; i < GameObjects.Count; i++)
            {
                GameObject gameObject = GameObjects[i];
                if (!gameObject.enabled) continue;
                if (!gameObject.started) { gameObject.Start(); gameObject.started = true; continue; }
                gameObject.Update();
            }
            deltaTime = deltaWatch.ElapsedMilliseconds * 0.001f;
            deltaWatch.Restart();
        }

        public void Render(RenderTarget rt)
        {
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
