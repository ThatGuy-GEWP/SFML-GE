using SFML.Graphics;
using SFML.Window;
using SFML_Game_Engine.Components;
using SFML_Game_Engine.Editor;
using SFML_Game_Engine.Engine.GUI.InputBoxes;
using SFML_Game_Engine.System;

namespace SFMLGE_Local_deps
{
    public class Program
    {
        public static RenderWindow App { get; private set; } = new RenderWindow(new VideoMode(1280, 720), "SFML-GE Template", Styles.Close | Styles.Titlebar);

        static void Main(string[] args)
        {
            bool appOpen = true;

            App.Closed += (a, args) => { App.Close(); appOpen = false; };
            App.SetFramerateLimit(144);

            Project mainProject = new Project("Res", App);
            Scene scene = mainProject.CreateSceneAndLoad("DefaultScene");


            //GameObject TestBox = scene.CreateGameObject("Test Box!");
            //TestBox.AddComponent(new Sprite2D(500f, 150f));

            InputBoxVector2 vecBox = scene.CreateGameObjectWithComp(new InputBoxVector2());
            vecBox.Size = new SFML_Game_Engine.GUI.UDim2(0, 0, 200, 25);
            vecBox.Position = new SFML_Game_Engine.GUI.UDim2(0.5f, 0.5f, 0, 0);

            GUIEditor editor = scene.CreateGameObjectWithComp(new GUIEditor(mainProject));


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