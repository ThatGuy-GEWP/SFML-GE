using SFML.Graphics;
using SFML.Window;
using SFMLGE_Local_deps.Scripts;

namespace SFML_Game_Engine
{
    internal class Program
    {
        public static RenderWindow App { get; private set; } = new RenderWindow(new VideoMode(1280, 720), "SFML Template", Styles.Close | Styles.Titlebar);

        static void Main(string[] args)
        {
            bool appOpen = true;
            App.Closed += (a, args) => { App.Close(); appOpen = false; };
            App.SetFramerateLimit(60);

            Project mainProject = new Project("Res", App);
            Scene scene = mainProject.CreateSceneAndLoad("Test!");

            mainProject.Start();

            scene.CreateGameObject("TestInterp").AddComponent(new InterpTestin(100, 0));
            scene.CreateGameObject("TestInterp").AddComponent(new InterpTestin(150, 1));
            scene.CreateGameObject("TestInterp").AddComponent(new InterpTestin(200, 2));
            scene.CreateGameObject("TestInterp").AddComponent(new InterpTestin(250, 3));
            scene.CreateGameObject("TestInterp").AddComponent(new InterpTestin(300, 4));

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