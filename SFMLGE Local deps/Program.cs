using SFML.Graphics;
using SFML.Window;

namespace SFML_Game_Engine
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
            App = new RenderWindow(new VideoMode(800, 400), "SFML Template");
            App.Closed += (a, b) => { App.Close(); };

            Project mainProject = new Project("Res", App);

            Prefab EnemyPrefab = new Prefab("Enemy", (proj, scene) =>
            {
                GameObject enemyBase = scene.CreateGameObject("Enemy");
                enemyBase.AddComponent(new Enemy(5));
                enemyBase.AddComponent(new Sprite2D(new Vector2(5, 5)));
                return enemyBase;
            });

            Prefab RotatersTest = new Prefab("Rotater", 
                (proj, scene) => {
                    GameObject baseObj = scene.CreateGameObject("Base");
                    GameObject secdObj = baseObj.CreateChild("Sncd");
                    baseObj.Rotation = 45;
                    baseObj.AddComponent(new Sprite2D(new Vector2(8, 8)));
                    secdObj.AddComponent(new Sprite2D(new Vector2(5, 5)));
                    secdObj.LocalPosition = new Vector2(100, 0);
                    secdObj.AddComponent(new Spinner());

                    return baseObj;
                });

            Scene MainScene = mainProject.CreateSceneAndLoad("Test!");
            MainScene.Start();

            GameObject test = MainScene.InstanciatePrefab(RotatersTest);
            GameObject test2 = MainScene.InstanciatePrefab(RotatersTest);

            MainScene.InstanciatePrefab(RotatersTest).Position = new Vector2(100, 100);
            MainScene.InstanciatePrefab(RotatersTest).Position = new Vector2(200, 100);
            MainScene.InstanciatePrefab(RotatersTest).Position = new Vector2(300, 100);

            for(int i = 0; i < 50; i++)
            {
                MainScene.InstanciatePrefab(EnemyPrefab).Position = new Vector2(150 + (10 * i), 160);
            }

            float t = 0;

            mainProject.Start();
            while (true)
            {
                App.DispatchEvents();
                App.Clear();

                MainScene.camera.SetPosition(new Vector2(MathF.Sin(t*5) * 50, 0));

                mainProject.Update();
                mainProject.Draw(App);
                
                t += MainScene.deltaTime;

                App.Display();
            }
        }
    }
}