using SFML.Graphics;
using SFML_GE.System;
using SFML_GE_Editor.Editor;
using SFML_GE_Editor.Editor.GUI;

namespace SFML_GE_Editor
{
    internal class Program
    {
        public static bool isPlaying = true;


        static void OnResized(GEWindow app)
        {

        }

        static void OnClosed(GEWindow app)
        {
            isPlaying = false;
        }

        public static PlayInstance PlayInstance = null!;

        static void Main(string[] args)
        {
            GEWindow app = new GEWindow(new SFML.Window.VideoMode(1280, 720), "SFML-GE Edtior");

            app.Resized += (_, _) => { OnResized(app); };

            app.Closed += (_, _) => { OnClosed(app); };

            Project EditorProject = new Project("res", app);
            Scene EditorScene = EditorProject.CreateSceneAndLoad("Editor");
            EditorScene.ClearColor = new Color(15, 15, 17);

            PlayInstance = new PlayInstance("res_game", app);

            EditorScene.CreateGameObjectWithComp(new PlayInstancePreview(PlayInstance), "Play Instance Pannel");

            EditorProject.Start();
            while (isPlaying)
            {
                app.DispatchEvents();

                EditorProject.Update();

                app.Clear();
                
                EditorProject.Render(app);

                app.Display();
            }

            app.Close();
        }
    }
}
