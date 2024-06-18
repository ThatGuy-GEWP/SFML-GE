using SFML.Graphics;
using SFML.Window;
using SFML_Game_Engine;
using SFML_Game_Engine.Editor;
using SFML_Game_Engine.GUI;
using SFMLGE_Local_deps.Scripts;

namespace SFMLGE_Local_deps
{
    public class Program
    {
        public static RenderWindow App { get; private set; } = new RenderWindow(new VideoMode(1600, 900), "SFML-GE Template", Styles.Close | Styles.Titlebar);

        static void Main(string[] args)
        {
            bool appOpen = true;

            App.Closed += (a, args) => { App.Close(); appOpen = false; };
            App.SetFramerateLimit(144);

            Project mainProject = new Project("Res", App);
            Scene scene = mainProject.CreateSceneAndLoad("DefaultScene");

            GUIEditor editor = scene.CreateGameObject("Editor").AddComponent(new GUIEditor(mainProject));

            GUIInteractiveWindow interWin = scene.CreateGameObject("TestPan").AddComponent(new GUIInteractiveWindow());
            interWin.Position = new UDim2(0, 0, 500, 200);

            GUIInputBox testInputBox = scene.CreateGameObject("TestInput").AddComponent(new TestComp()).gameObject.AddComponent(new GUIInputBox());
            testInputBox.Position = new UDim2(0, 0, 500, 400);

            mainProject.Start();
            while (appOpen)
            {
                mainProject.Update();

                App.Clear();
                mainProject.Render(App);
                App.Display();
            }
        }
    }
}