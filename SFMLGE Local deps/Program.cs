using SFML.Graphics;
using SFML.Window;
using SFML_Game_Engine.GUI;
using static SFML.Window.Mouse;

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



            mainProject.Start();

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