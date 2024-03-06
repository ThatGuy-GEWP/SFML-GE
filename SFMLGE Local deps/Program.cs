using SFML.Graphics;
using SFML.Window;
using SFML_Game_Engine.Editor;

namespace SFML_Game_Engine
{
    public class Program
    {
        public static RenderWindow App { get; private set; } = new RenderWindow(new VideoMode(1280, 720), "SFML Template", Styles.Close | Styles.Titlebar);

        static void Main(string[] args)
        {
            bool appOpen = true;

            App.Closed += (a, args) => { App.Close(); appOpen = false; };
            App.SetFramerateLimit(144);

            Project mainProject = new Project("Res", App);
            EditorContext editContext = new EditorContext(mainProject);

            Scene scene = mainProject.CreateSceneAndLoad("Test!");

            GameObject testy = scene.CreateGameObject("Test object!");

            testy.AddComponent(new Sprite2D(new Vector2(500, 2550)));


            editContext.Start();

            float t = 0;
            while (appOpen)
            {
                App.Clear();
                t += scene.deltaTime;

                testy.transform.WorldPosition = new Vector2(MathF.Sin(t) * 50f, 0);

                editContext.Update();
                editContext.Render(App);

                App.Display();
            }
        }
    }
}