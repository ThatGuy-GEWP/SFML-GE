using SFML.Graphics;
using SFML.Window;
using System.Diagnostics;

namespace SFML_Game_Engine
{
    /// <summary>
    /// A Project holds scenes, and all resources.
    /// </summary>
    public class Project
    {
        public Stopwatch sinceProjectCreation { get; private set; } = Stopwatch.StartNew();

        public ResourceCollection Resources;

        public Scene? ActiveScene;

        List<Scene> scenes = new List<Scene>();

        public RenderWindow App;

        public Dictionary<string, Keyboard.Key> inputs = new Dictionary<string, Keyboard.Key>()
        {
            {"exit" ,      Keyboard.Key.Escape},
            {"move_up" ,   Keyboard.Key.W},
            {"move_down",  Keyboard.Key.S},
            {"move_left",  Keyboard.Key.A},
            {"move_right", Keyboard.Key.D},
            {"toggle_camera", Keyboard.Key.Space }
        };

        Dictionary<string, bool> inputPressed = new Dictionary<string, bool>();
        Dictionary<string, bool> inputJustPressed = new Dictionary<string, bool>();
        Dictionary<string, bool> inputJustReleased = new Dictionary<string, bool>();

        public bool started { get; private set; } = false;

        string? resourceDir = null;

        /// <summary>
        /// Creates a new project and loads a directory of resources.
        /// </summary>
        /// <param name="ResourceDir"></param>
        /// <param name="app"></param>
        public Project(string ResourceDir, RenderWindow app)
        {
            App = app;
            resourceDir = ResourceDir;
            Resources = new ResourceCollection(ResourceDir, this);
        }

        public T GetResource<T>(string name) where T : Resource
        {
            return Resources.GetResource<T>(name);
        }

        public Scene CreateScene()
        {
            Scene scn = new Scene("Untitled", this);
            scenes.Add(scn);
            return scn;
        }

        public Scene CreateScene(string sceneName)
        {
            Scene scn = new Scene(sceneName, this);
            scenes.Add(scn);
            return scn;
        }

        public Scene CreateSceneAndLoad()
        {
            ActiveScene = CreateScene();
            ActiveScene.LoadScene();
            return ActiveScene;
        }

        public Scene CreateSceneAndLoad(string sceneName)
        {
            ActiveScene = CreateScene(sceneName);
            ActiveScene.LoadScene();
            return ActiveScene;
        }

        public void LoadScene(Scene scene)
        {
            if (ActiveScene == null)
            {
                ActiveScene = scene;
                ActiveScene.LoadScene();
                if (started) { ActiveScene.Start(); }
                return;
            }
            ActiveScene.UnloadScene();
            ActiveScene = scene;
            ActiveScene.LoadScene();
            if (started) { ActiveScene.Start(); }
        }

        public void LoadScene(string sceneName)
        {
            foreach (Scene scn in scenes)
            {
                if (scn.Name == sceneName)
                {
                    LoadScene(scn);
                    return;
                }
            }
            Console.WriteLine("Failed to load scene '" + sceneName + "'!");
        }

        public void AddInput(string inputName, Keyboard.Key key)
        {
            inputs.Add(inputName, key);
        }

        public void Start()
        {
            InputUpdate();
            started = true;
            if (ActiveScene is null) { return; }
            ActiveScene.Start();
        }

        public void Update()
        {
            if(!started) 
            {
                Start();
                return; 
            }

            App.DispatchEvents();

            if (ActiveScene is null) { return; }

            if (!ActiveScene.started) { ActiveScene.Start(); return; }

            InputUpdate();
            ActiveScene.Update();
        }

        public void Render(RenderTarget rt)
        {
            if (ActiveScene is null) { return; }
            ActiveScene.Render(rt);
        }

        void InputUpdate()
        {
            foreach (string key in inputs.Keys)
            {
                if (inputPressed.ContainsKey(key) == false) { inputPressed.Add(key, false); }
                if (inputJustPressed.ContainsKey(key) == false) { inputJustPressed.Add(key, false); }
                if (inputJustReleased.ContainsKey(key) == false) { inputJustReleased.Add(key, false); }
            }

            foreach (string key in inputs.Keys)
            {
                inputJustPressed[key] = Keyboard.IsKeyPressed(inputs[key]) == true && inputPressed[key] == false;
                inputJustReleased[key] = Keyboard.IsKeyPressed(inputs[key]) == false && inputPressed[key] == true;
                inputPressed[key] = Keyboard.IsKeyPressed(inputs[key]);
            }
        }

        public bool IsInputPressed(string inputName)
        {
            if (!App.HasFocus()) { return false; }
            return inputPressed[inputName];
        }

        public bool IsInputJustPressed(string inputName)
        {
            if (!App.HasFocus()) { return false; }
            return inputJustPressed[inputName];
        }

        public bool IsInputJustReleased(string inputName)
        {
            if (!App.HasFocus()) { return false; }
            return inputJustReleased[inputName];
        }

        /// <summary>
        /// Returns a normalized vector2 based on the provided inputs
        /// </summary>
        /// <param name="xminus">the negative x axis input</param>
        /// <param name="xplus">the positive x axis input</param>
        /// <param name="yminus">the negative y axis input</param>
        /// <param name="yplus">the positive y axis input</param>
        /// <returns></returns>
        public Vector2 GetInputAxis(string xminus, string xplus, string yminus, string yplus)
        {
            bool xnegPressed = IsInputPressed(xminus);
            bool xposPressed = IsInputPressed(xplus);

            bool ynegPressed = IsInputPressed(yminus);
            bool yposPressed = IsInputPressed(yplus);

            return Vector2.Normalize(new Vector2(
                xnegPressed && xposPressed ? 0 : xnegPressed ? -1 : xposPressed ? 1 : 0,
                ynegPressed && yposPressed ? 0 : ynegPressed ? -1 : yposPressed ? 1 : 0));
        }
    }
}
