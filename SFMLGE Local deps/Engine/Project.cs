


using SFML.Graphics;
using SFML.Window;

namespace SFML_Game_Engine
{
    /// <summary>
    /// A Project holds scenes, and all resources.
    /// </summary>
    public class Project
    {
        public ResourceCollection Resources;

        public Scene? ActiveScene;

        public RenderWindow App;

        public Dictionary<string, Keyboard.Key> inputs = new Dictionary<string, Keyboard.Key>()
        {
            {"exit" ,      Keyboard.Key.Escape},
            {"move_up" ,   Keyboard.Key.W},
            {"move_down",  Keyboard.Key.S},
            {"move_left",  Keyboard.Key.A},
            {"move_right", Keyboard.Key.D},
        };

        Dictionary<string, bool> inputPressed = new Dictionary<string, bool>();
        Dictionary<string, bool> inputJustPressed = new Dictionary<string, bool>();
        Dictionary<string, bool> inputJustReleased = new Dictionary<string, bool>();

        public Project(string ResourceDir, RenderWindow app) 
        {
            App = app;
            Resources = new ResourceCollection(ResourceDir);
        }

        public T GetResource<T>(string name) where T : Resource
        {
            return Resources.GetResourceByName<T>(name);
        }

        public Scene CreateScene()
        {
            return new Scene("Untitled", this);
        }

        public Scene CreateScene(string sceneName)
        {
            return new Scene(sceneName, this);
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

        public void Start()
        {
            if (ActiveScene is null) { return; }
            ActiveScene.Start();
        }

        public void Update()
        {
            if (ActiveScene is null) { return; }
            InputUpdate();
            ActiveScene.Update();
        }

        public void Draw(RenderTarget rt)
        {
            if(ActiveScene is null) { return; }
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
            return inputPressed[inputName];
        }

        public bool IsInputJustPressed(string inputName)
        {
            return inputJustPressed[inputName];
        }

        public bool IsInputJustReleased(string inputName)
        {
            return inputJustReleased[inputName];
        }
    }
}
