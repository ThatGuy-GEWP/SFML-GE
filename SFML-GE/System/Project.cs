using SFML.Graphics;
using SFML.Window;
using SFML_GE.Debugging;
using SFML_GE.GUI;
using System.Diagnostics;

namespace SFML_GE.System
{
    /// <summary>
    /// A Project holds scenes, and all resources.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// A <see cref="Stopwatch"/> That is started once this <see cref="Project"/> instance is created.
        /// </summary>
        public Stopwatch sinceProjectCreation { get; private set; } = Stopwatch.StartNew();

        /// <summary>
        /// The <see cref="ResourceCollection"/> holding all the <see cref="Resource"/>s of the project.
        /// </summary>
        public ResourceCollection Resources;

        /// <summary>
        /// The <see cref="StylingDef"/> all newly created <see cref="GUIPanel"/>'s will use.
        /// </summary>
        public StylingDef GUIStyling;

        /// <summary>
        /// The active <see cref="Scene"/> of the project, null if no scene is loaded.
        /// </summary>
        public Scene? ActiveScene;

        List<Scene> scenes = new List<Scene>();

        /// <summary>
        /// The <see cref="GEWindow"/> this project uses for inputs.
        /// </summary>
        public GEWindow App;

        /// <summary>
        /// A Dictionary containing all monitored inputs,
        /// You should use <see cref="AddInput(string, Keyboard.Key)"/> to add any,
        /// And only use this Dictionary to find used ones.
        /// </summary>
        public Dictionary<string, Keyboard.Key> inputs = new Dictionary<string, Keyboard.Key>()
        {
            {"exit" ,      Keyboard.Key.Escape},
            {"move_up" ,   Keyboard.Key.W},
            {"move_down",  Keyboard.Key.S},
            {"move_left",  Keyboard.Key.A},
            {"move_right", Keyboard.Key.D},
            {"ui_confirm", Keyboard.Key.Enter}
        };

        private readonly Dictionary<string, bool> inputPressed = new Dictionary<string, bool>();
        private readonly Dictionary<string, bool> inputJustPressed = new Dictionary<string, bool>();
        private readonly Dictionary<string, bool> inputJustReleased = new Dictionary<string, bool>();

        /// <summary>
        /// The Delta of the scroll wheel
        /// </summary>
        public float ScrollDelta = 0.0f;

        /// <summary>
        /// The size of the <see cref="RenderTarget"/> this project last rendered to.
        /// </summary>
        public Vector2 RenderTargetSize { get; private set; } = new Vector2(50, 50);
        // Defaults to 50,50 as for some GODFORSAKEN REASON the build will just
        // *hang* on github actions
        // im guessing its a division error or something as its the same with 1 or zero just not 50
        // il look into it when i dont want to waste 6 hours of github server time, for now this works and stops the hang.

        /// <summary>
        /// If false, the project has yet to be started.
        /// </summary>
        public bool Started { get; private set; } = false;

        /// <summary>
        /// If true, the project will write stuff to the console
        /// </summary>
        public bool writeToConsole = true;

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

        /// <summary>
        /// The path (as a string) given when this project was created to collect resources from.
        /// </summary>
        public string? ResourceDir { get; private set; } = null;

        Cursor.CursorType _cursorState = Cursor.CursorType.Arrow;

        /// <summary>
        /// The current <see cref="Cursor.CursorType"/> of the cursor.
        /// </summary>
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
        public Project(string? ResourceDir, GEWindow app)
        {
            App = app;
            this.ResourceDir = ResourceDir;
            GUIStyling = new StylingDef();
            Resources = new ResourceCollection(this.ResourceDir);

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
#if DEBUG
            Gizmo.LinkToProject(this);
#endif
        }

        /// <summary>
        /// Gets a <see cref="Resource"/> <typeparamref name="T"/> from the project's resources.
        /// </summary>
        /// <typeparam name="T">the type of resouce to find</typeparam>
        /// <param name="name">the name of the resource</param>
        public T? GetResource<T>(string name) where T : Resource
        {
            return Resources.GetResource<T>(name);
        }

        /// <summary>
        /// Creates and returns new scene for the project.
        /// </summary>
        /// <param name="sceneName">the name of this scene, should be unique</param>
        /// <returns>the newly created scene</returns>
        public Scene CreateScene(string sceneName = "Untitled")
        {
            Scene scn = new Scene(sceneName, this);
            scenes.Add(scn);
            return scn;
        }

        /// <summary>
        /// Creates and loads a scene.
        /// </summary>
        /// <param name="sceneName">the name of this scene, should be unique</param>
        /// <returns>the newly created scene</returns>
        public Scene CreateSceneAndLoad(string sceneName = "Untitled")
        {
            ActiveScene = CreateScene(sceneName);
            ActiveScene.LoadScene();
            return ActiveScene;
        }

        /// <summary>
        /// Loads a scene using the given <paramref name="scene"/>
        /// </summary>
        /// <param name="scene">The scene to load.</param>
        public void LoadScene(Scene scene)
        {
            if (ActiveScene == null)
            {
                ActiveScene = scene;
                ActiveScene.LoadScene();
                if (Started) { ActiveScene.Start(); }
                return;
            }
            UnloadScene();
            ActiveScene = scene;
            ActiveScene.LoadScene();
            if (Started) { ActiveScene.Start(); }
        }

        /// <summary>
        /// Unloads the active scene if any.
        /// </summary>
        public void UnloadScene()
        {
            if (ActiveScene == null) { return; }
            var scn = ActiveScene;
            ActiveScene = null;
            scn.UnloadScene();
        }

        /// <summary>
        /// Finds the first scene named <paramref name="sceneName"/> then loads it.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load</param>
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
            DebugLogger.LogWarning("Failed to load scene '" + sceneName + "'!");
        }

        /// <summary>
        /// Finds the first scene named <paramref name="sceneName"/> then returns it.
        /// </summary>
        /// <param name="sceneName">The name of the scene to get</param>
        /// <returns></returns>
        public Scene? GetScene(string sceneName)
        {
            foreach (Scene scn in scenes)
            {
                if (scn.Name == sceneName)
                {
                    return scn;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a new keyboard input to monitor with the given <paramref name="inputName"/> and <paramref name="key"/><para></para>
        /// you can get the state of the new input with <see cref="IsInputJustPressed(string)"/>
        /// </summary>
        /// <param name="inputName"></param>
        /// <param name="key"></param>
        public void AddInput(string inputName, Keyboard.Key key)
        {
            inputs.Add(inputName, key);
        }

        /// <summary>
        /// Starts this project and the loaded scene.
        /// If no scene is loaded, nothing will happen.
        /// </summary>
        public void Start()
        {
            InputUpdate();
            Started = true;
            if (ActiveScene is null) { return; }
            ActiveScene.Start();
            if (writeToConsole) 
            {
                DebugLogger.LogInfoPriority("Project started");
                Console.WriteLine("---------------------------"); 
            }
        }

        /// <summary>
        /// Updates this project and its loaded scene.
        /// </summary>
        public void Update()
        {
            if (!Started)
            {
                if (writeToConsole) { 
                    DebugLogger.LogWarning("Update called without being started, manually starting..."); 
                }
                Start();
            }

            App.DispatchEvents();

            for(int i = 0; i < scenes.Count; i++)
            {
                var scene = scenes[i];
                if(scene.destroyQueued) // we need to start cleaning up
                {
                    if(ActiveScene == scene)
                    {
                        UnloadScene();
                    }
                    scene.FinalizeDestory();

                    scenes.Remove(scene);
                    i--; continue;
                }
            }

            if (ActiveScene is null) { return; }

            if (!ActiveScene.Started) { ActiveScene.Start(); return; }

            InputUpdate();
            ActiveScene.Update();
            ScrollDelta = 0;
        }

        /// <summary>
        /// Renders the current scene to the given <see cref="RenderTarget"/>, <paramref name="rt"/>
        /// </summary>
        /// <param name="rt">The target this project will render too.</param>
        public void Render(RenderTarget rt)
        {
            if (ActiveScene is null) { return; }
            RenderTargetSize = (Vector2)rt.Size;
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
        /// The ZIndex the mouse is hovering over.
        /// </summary>
        public int MouseHoveringZ;

        /// <summary>
        /// The ZIndex the mouse is clicking.
        /// </summary>
        public int MouseClickingZ;

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
        /// Returns true the first frame a mouse button is pressed down
        /// </summary>
        /// <param name="button">the button to check as an int, 0 is left, 1 is right, 2 is middle, 3-4 are the extra buttons</param>
        /// <returns></returns>
        public bool IsMouseButtonPressed(int button)
        {
            button = (int)MathGE.Clamp(button, 0, 4); // eh fuck it why not
            return pressedDict[(Mouse.Button)button];
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
