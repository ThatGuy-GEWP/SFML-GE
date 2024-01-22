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

            Project mainProject = new Project("Res", App);
            Scene scene = mainProject.CreateSceneAndLoad("Test!");

            mainProject.Start();

            GameObject testObject = scene.CreateGameObject("Test object!");
            testObject.AddComponent(new Sprite2D(new Vector2(25, 25), new Vector2(0.5f, 0.5f)));
            testObject.Position = new Vector2(640, 360);

            while (appOpen)
            {
                App.Clear();

                mainProject.Update();
                mainProject.Render(App);

                App.Display();
            }
        }
    }
}