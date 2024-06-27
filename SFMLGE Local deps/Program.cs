using SFML.Graphics;
using SFML.Window;
using SFML_Game_Engine.System;
using SFML_Game_Engine.GUI;
using SFML_Game_Engine;
using SFML_Game_Engine.Scripts;
using SFML_Game_Engine.Components;
using SFML_Game_Engine.Resources;

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