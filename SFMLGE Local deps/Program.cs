using SFML.Graphics;
using SFML.Window;
using SFML_Game_Engine.GUI;

namespace SFML_Game_Engine
{
    internal class Program
    {
        public static RenderWindow App { get; private set; } = new RenderWindow(new VideoMode(1280, 720), "SFML Template", Styles.Close | Styles.Titlebar);

        static void Main(string[] args)
        {
            bool appOpen = true;

            App.Closed += (a, args) => { App.Close(); appOpen = false; };
            App.SetFramerateLimit(144);

            Project mainProject = new Project("Res", App);
            Scene scene = mainProject.CreateSceneAndLoad("Test!");


            GameObject GUIBase = scene.CreateGameObject("Testy!~");

            GUIContext context = GUIBase.AddComponent(new GUIContext(1280, 720));


            GUITextLabel textLabel = new GUITextLabel("The quick brown fox\nJumped over the\nLazy dog");

            context.AddComponent(textLabel);

            float t = -0.1f;

            float ft = 0;
            while (appOpen)
            {
                App.Clear();
                t += scene.deltaTime * 2f;

                ft = t > 1.0f ? 1.0f : t < 0.0f ? 0.0f : t;
                t = t > 2.0f ? 0.0f : t;

                textLabel.padding = MathGE.Interpolation.QuadraticEaseOut(5, 15f, ft*2f);

                textLabel.transform.origin = new Vector2(0.5f, 0.5f);
                textLabel.transform.WorldPosition = scene.GetMouseWorldPosition();

                mainProject.Update();
                mainProject.Render(App);

                App.Display();
            }
        }
    }
}