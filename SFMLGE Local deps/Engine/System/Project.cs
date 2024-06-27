using SFML.Graphics;
using SFML.Window;
using SFML_Game_Engine.Engine.Networking;
using SFML_Game_Engine.GUI;
using SFML_Game_Engine.System;
using System.Diagnostics;

namespace SFML_Game_Engine.System
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
            {"ui_confirm", Keyboard.Key.Enter}
        };

        Dictionary<string, bool> inputPressed = new Dictionary<string, bool>();
        Dictionary<string, bool> inputJustPressed = new Dictionary<string, bool>();
        Dictionary<string, bool> inputJustReleased = new Dictionary<string, bool>();

        public float ScrollDelta = 0.0f;

        public bool Started { get; private set; } = false;

        Dictionary<Mouse.Button, bool> pressedDict = new Dictionary<Mouse.Button, bool>();
        Dictionary<Mouse.Button, bool> releasedDict = new Dictionary<Mouse.Button, bool>();
        Dictionary<Mouse.Button, bool> heldDict = new Dictionary<Mouse.Button, bool>();

        /// <summary>Called the first frame a mouse button is pressed.</summary>
        public event Action<Mouse.Button>? OnMouseButtonPressed;

        /// <summary>Called the first frame a mouse button is released.</summary>
        public event Action<Mouse.Button>? OnMouseButtonReleased;

        /// <summary>Called every frame a mouse button is held down.</summary>
        public event Action<Mouse.Button>? OnMouseButtonHeld;

        /// <summary> if false, input querys will always return false. </summary>
        public bool allowInputs = true;

        public string? ResourceDir { get; private set; } = null;

        public NetworkingManager networkingManager;

        public bool useNetworking = false;

        Cursor.CursorType _cursorState = Cursor.CursorType.Arrow;

        public Cursor.CursorType CursorState
        {
            get
            {
                return _cursorState;
            }
            set
            {
                if (value != _cursorState)
                {
                    Cursor cursor = new Cursor(value);
                    App.SetMouseCursor(cursor);
                    cursor.Dispose();
                    _cursorState = value;
                }
            }
        }

        /// <summary>
        /// Creates a new project and loads a directory of resources.
        /// </summary>
        /// <param name="ResourceDir"></param>
        /// <param name="app"></param>
        public Project(string ResourceDir, RenderWindow app)
        {
            App = app;
            this.ResourceDir = ResourceDir;
            Resources = new ResourceCollection(this.ResourceDir, this);
            if (Directory.Exists("Engine"))
            {
                Resources.LoadDir("Engine");
            }

            app.MouseWheelScrolled += (sender, args) =>
            {
                ScrollDelta = args.Delta;
            };

            pressedDict[Mouse.Button.Left] = false;
            pressedDict[Mouse.Button.Right] = false;
            pressedDict[Mouse.Button.Middle] = false;

            releasedDict[Mouse.Button.Left] = false;
            releasedDict[Mouse.Button.Right] = false;
            releasedDict[Mouse.Button.Middle] = false;

            heldDict[Mouse.Button.Left] = false;
            heldDict[Mouse.Button.Right] = false;
            heldDict[Mouse.Button.Middle] = false;

            networkingManager = new NetworkingManager(this, "127.0.0.1", 25565);
        }

        public T GetResource<T>(string name) where T : Resource
        {
            return Resources.GetResource<T>(name);
        }

        public Scene CreateScene(string sceneName = "Untitled")
        {
            Scene scn = new Scene(sceneName, this);
            scenes.Add(scn);
            return scn;
        }

        public Scene CreateSceneAndLoad(string sceneName = "Untitled")
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
                if (Started) { ActiveScene.Start(); }
                return;
            }
            ActiveScene.UnloadScene();
            ActiveScene = scene;
            ActiveScene.LoadScene();
            if (Started) { ActiveScene.Start(); }
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
            Started = true;
            if (ActiveScene is null) { return; }
            ActiveScene.Start();
        }

        public void Update()
        {
            if (!Started)
            {
                Start();
                return;
            }

            App.DispatchEvents();

            if (ActiveScene is null) { return; }

            if (!ActiveScene.Started) { ActiveScene.Start(); return; }

            InputUpdate();
            ActiveScene.Update();
            ScrollDelta = 0;

            networkingManager.Update();
        }

        public void Render(RenderTarget rt)
        {
            if (ActiveScene is null) { return; }
            ActiveScene.Render(rt);
        }

        void MouseInputUpdate()
        {
            foreach (KeyValuePair<Mouse.Button, bool> kvp in pressedDict)
            {
                Mouse.Button curButton = kvp.Key;
                bool curPressed = kvp.Value;
                bool curReleased = releasedDict[curButton];
                bool curHeld = heldDict[curButton];

                bool mouseState = Mouse.IsButtonPressed(curButton);

                releasedDict[curButton] = false;

                if (mouseState && curPressed && curHeld)
                {
                    pressedDict[curButton] = false;
                }

                if (mouseState && !curPressed && !curHeld)
                {
                    pressedDict[curButton] = true;
                    OnMouseButtonPressed?.Invoke(curButton);
                }

                if (!Mouse.IsButtonPressed(curButton) && heldDict[curButton])
                {
                    OnMouseButtonReleased?.Invoke(curButton);
                    releasedDict[curButton] = true;
                    pressedDict[curButton] = false;
                    heldDict[curButton] = false;
                }

                heldDict[curButton] = mouseState;

                if (heldDict[curButton])
                {
                    OnMouseButtonHeld?.Invoke(curButton);
                }
            }
        }

        /// <summary>
        /// Returns true the first frame a mouse button is pressed down
        /// </summary>
        /// <param name="button">the button to check</param>
        /// <returns></returns>
        public bool IsMouseButtonPressed(Mouse.Button button)
        {
            return pressedDict[button];
        }

        /// <summary>
        /// Returns true the first frame a mouse button is released
        /// </summary>
        /// <param name="button">the button to check</param>
        /// <returns></returns>
        public bool IsMouseButtonReleased(Mouse.Button button)
        {
            return releasedDict[button];
        }

        /// <summary>
        /// Returns true while a mouse button is pressed
        /// </summary>
        /// <param name="button">the button to check</param>
        /// <returns></returns>
        public bool IsMouseButtonHeld(Mouse.Button button)
        {
            return Mouse.IsButtonPressed(button);
        }

        void InputUpdate()
        {
            if (!allowInputs) { return; }
            MouseInputUpdate();

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

        /// <summary>
        /// Returns true while the input <paramref name="inputName"/> is held down.
        /// </summary>
        /// <param name="inputName"></param>
        /// <returns></returns>
        public bool IsInputPressed(string inputName)
        {
            if (!App.HasFocus() || !allowInputs) { return false; }
            return inputPressed[inputName];
        }

        /// <summary>
        /// Returns true every time the input <paramref name="inputName"/> has just been pressed down
        /// </summary>
        /// <param name="inputName"></param>
        /// <returns></returns>
        public bool IsInputJustPressed(string inputName)
        {
            if (!App.HasFocus() || !allowInputs) { return false; }
            return inputJustPressed[inputName];
        }

        /// <summary>
        /// Returns true every time the input <paramref name="inputName"/> has been released
        /// </summary>
        /// <param name="inputName"></param>
        /// <returns></returns>
        public bool IsInputJustReleased(string inputName)
        {
            if (!App.HasFocus() || !allowInputs) { return false; }
            return inputJustReleased[inputName];
        }

        /// <summary>
        /// Returns a normalized vector2 based on the provided input names
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
