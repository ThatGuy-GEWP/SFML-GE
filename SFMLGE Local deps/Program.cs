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

        static void PrefabSetup(Project project)
        {
            Prefab cardPrefabA = new Prefab("cardPrefabA",
                (proj, scene) =>
                {
                    GameObject newGo = scene.CreateGameObject();

                    Sprite2D spr = new Sprite2D(project.GetResource<TextureResource>("cardA"));
                    newGo.AddComponent(spr);

                    spr.fitTexture = false;
                    spr.size *= 3;
                    newGo.transform.rotation = RandomGen.Next(-15, 15);

                    newGo.AddComponent(new MoverScript(newGo.AddComponent(new NetworkedTransform()), spr.size));
                    newGo.AddComponent(new CardScript());

                    return newGo;
                });

            project.Resources.AddResource(cardPrefabA);

            Prefab cardPrefabB = new Prefab("cardPrefabB",
                (proj, scene) =>
                {
                    GameObject newGo = scene.CreateGameObject();

                    Sprite2D spr = new Sprite2D(project.GetResource<TextureResource>("cardB"));
                    newGo.AddComponent(spr);

                    spr.fitTexture = false;
                    spr.size *= 3;
                    newGo.transform.rotation = RandomGen.Next(-15, 15);

                    newGo.AddComponent(new MoverScript(newGo.AddComponent(new NetworkedTransform()), spr.size));
                    newGo.AddComponent(new CardScript());

                    return newGo;
                });

            project.Resources.AddResource(cardPrefabB);

        }


        static void Main(string[] args)
        {
            bool appOpen = true;

            App.Closed += (a, args) => { App.Close(); appOpen = false; };
            App.SetFramerateLimit(144);

            Project mainProject = new Project("Res", App);
            Scene scene = mainProject.CreateSceneAndLoad("DefaultScene");
            PrefabSetup(mainProject);

            GameObject background = scene.CreateGameObject("bg");
            Sprite2D bckSpr = background.AddComponent(new Sprite2D(mainProject.GetResource<TextureResource>("tableBackground")));
            bckSpr.fitTexture = false;
            bckSpr.size *= 4;
            bckSpr.anchor = Vector2.zero;
            background.ZOrder = -100;

            Console.WriteLine("--SFML-GE Project loaded--\nApplication Output:\n");

            GUIPanel buttonPanel = scene.CreateGameObject("HostHolder").AddComponent(new GUIPanel());

            GUIPanel startedPanel = scene.CreateGameObject("StartedHolder").AddComponent(new GUIPanel());
            startedPanel.gameObject.enabled = false;

            GUIButtonLabel spawnBut = scene.CreateGameObject("SpawnButton", startedPanel.gameObject).AddComponent(new GUIButtonLabel("Spawn card"));
            spawnBut.Size = new UDim2(0.5f, 1f, 0, 0);

            GUIButtonLabel spawnBut2 = scene.CreateGameObject("SpawnButton", startedPanel.gameObject).AddComponent(new GUIButtonLabel("Spawn Effect"));
            spawnBut2.Size = new UDim2(0.5f, 1f, 0, 0);
            spawnBut2.Position = new UDim2(0.5f, 0, 0, 0);

            startedPanel.Position = new UDim2(0, 1f, 15, -15);
            startedPanel.Size = new UDim2(0, 0, 240, 60);
            startedPanel.Anchor = new Vector2(0f, 1f);

            buttonPanel.Position = new UDim2(0, 0, 15, 15);
            buttonPanel.Size = new UDim2(0, 0, 120, 60);

            GUIButtonLabel asHost = scene.CreateGameObject("tr", buttonPanel.gameObject).AddComponent(new GUIButtonLabel("Host"));
            GUIButtonLabel asClient = scene.CreateGameObject("tr", buttonPanel.gameObject).AddComponent(new GUIButtonLabel("Join"));

            asClient.Size = new UDim2(0.5f, 1f, 0, 0);
            asClient.Position = new UDim2(1f, 0, 0, 0);
            asClient.Anchor = new Vector2(1f, 0);

            asHost.Size = asClient.Size;

            for(int i = 0; i < 4; i++)
            {
                GameObject go = scene.CreateGameObject();
                go.transform.WorldPosition = new Vector2(-50, -50);
                go.AddComponent(new PlayerCursor("cursor" + (i + 1), i - 1));
            }


            NetworkingManager netMan = mainProject.networkingManager;

            asHost.OnClick += (but) =>
            {
                if (netMan.Started) { return; }
                App.SetMouseCursorVisible(false);
                netMan.Start(true, "25.3.1.255", 49292);
                App.SetTitle("Server {-1}");
                buttonPanel.gameObject.Destroy();
                startedPanel.gameObject.enabled = true;
            };

            asClient.OnClick += (but) =>
            {
                if (netMan.Started) { return; }
                App.SetMouseCursorVisible(false);
                netMan.Start(false, "25.3.1.255", 49292);
                netMan.OnConnected += () =>
                {
                    App.SetTitle("Client {"+netMan.MyID+"}");
                };
                buttonPanel.gameObject.Destroy();
                startedPanel.gameObject.enabled = true;
            };

            spawnBut.OnClick += (b) =>
            {
                string cardName = RandomGen.Next() > 0.5f ? "cardPrefabA" : "cardPrefabB";
                GameObject inst = netMan.InstanceSharedPrefab(cardName, "cardTestInstance" + (RandomGen.Next(999999).ToString()), new Vector2(100, 100))!;
            };

            mainProject.Start();
            while (appOpen)
            {
                mainProject.Update();
                App.Clear();
                mainProject.Render(App);
                App.Display();
            }

            netMan.Close();
        }
    }
}