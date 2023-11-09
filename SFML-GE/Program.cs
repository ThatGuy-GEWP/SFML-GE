using SFML.Graphics;
using SFML.Window;

namespace SFML_GE
{
    internal class Program
    {
        public static RenderWindow App { get; private set; }

        static void OnClose(object? sender, EventArgs args)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        static void Main(string[] args)
        {
            App = new RenderWindow(new VideoMode(1280, 720), "SFML Template");
            App.Closed += (a, b) => { App.Close(); };

            Project mainProject = new Project("Res", App);
            Scene MainScene = mainProject.CreateSceneAndLoad("Test!");

            MainScene.MainCamera.Zoom = 2f;

            Prefab EnemyPrefab = new Prefab("Enemy", (proj, scene) =>
            {
                GameObject enemyBot = scene.CreateGameObject("Enemy");
                enemyBot.AddComponent(new Enemy(5));
                enemyBot.AddComponent(new Sprite2D(new Vector2(5, 5)));
                return enemyBot;
            });

            for (int i = 0; i < 850; i++)
            {
                MainScene.InstanciatePrefab(EnemyPrefab).Position = new Vector2(RandomGen.Next(50, 1200), RandomGen.Next(50, 700));
            }

            MainScene.Start();
            while (true)
            {
                App.DispatchEvents();
                App.Clear();

                mainProject.Update();
                mainProject.Draw(App);

                App.Display();
            }
        }
    }
}