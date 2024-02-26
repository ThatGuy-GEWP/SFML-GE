using SFML.Graphics;
using SFML.Window;

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

            GameObject testGo = scene.CreateGameObject("test!");

            testGo.transform.position = new Vector2(100, 100);
            testGo.AddComponent(new Sprite2D(new Vector2(25, 25)));

            GameObject secondGo = scene.CreateGameObject("secondGo", testGo);
            secondGo.AddComponent(new Sprite2D(new Vector2(50, 50)));
            secondGo.transform.WorldPosition = new Vector2(55, 0);

            mainProject.Start();

            float t = 0;

            while (appOpen)
            {
                App.Clear();
                t += scene.deltaTime;

                secondGo.transform.WorldPosition = new Vector2(0, 0);

                testGo.transform.position = new Vector2(100, 100 + MathF.Sin(t)*50);

                mainProject.Update();
                mainProject.Render(App);

                App.Display();
            }
        }
    }
}