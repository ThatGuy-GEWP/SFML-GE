using SFML.Graphics;
using SFML.Window;
using SFML_Game_Engine.Editor;
using SFML_Game_Engine.System;
using SFML_Game_Engine.GUI;
using SFMLGE_Local_deps.Scripts;
using SFML_Game_Engine;
using SFML_Game_Engine.Scripts;
using SFML_Game_Engine.Components;

namespace SFMLGE_Local_deps
{
    public class Program
    {
        public static RenderWindow App { get; private set; } = new RenderWindow(new VideoMode(800, 400), "SFML-GE Template", Styles.Close | Styles.Titlebar);

        static void Main(string[] args)
        {
            bool appOpen = true;

            App.Closed += (a, args) => { App.Close(); appOpen = false; };
            App.SetFramerateLimit(144);

            Project mainProject = new Project("Res", App);
            Scene scene = mainProject.CreateSceneAndLoad("DefaultScene");

            GUIPanel buttonPanel = scene.CreateGameObject("HostHolder").AddComponent(new GUIPanel());

            buttonPanel.Position = new UDim2(0, 0, 15, 15);
            buttonPanel.Size = new UDim2(1f, 0, -30, 50);

            GUIButton asHost = scene.CreateGameObject("tr", buttonPanel.gameObject).AddComponent(new GUIButton());
            GUIButton asClient = scene.CreateGameObject("tr", buttonPanel.gameObject).AddComponent(new GUIButton());

            asClient.Size = new UDim2(0, 1f, 50, 0);
            asClient.Position = new UDim2(1f, 0, 0, 0);
            asClient.Anchor = new Vector2(1f, 0);

            asHost.Size = asClient.Size;
            

            GUIInputBox chatBox = scene.CreateGameObject("tr", buttonPanel.gameObject).AddComponent(new GUIInputBox());
            chatBox.displayedString = "";

            NetworkingManager netMan = new NetworkingManager(mainProject, "25.3.1.255", 49292);

            chatBox.Position = new UDim2(0.5f, 0.5f, 0, 0);
            chatBox.Anchor = new Vector2(0.5f, 0.5f);

            MoverScript moverObj = scene.CreateGameObject("Mover!").AddComponent(new MoverScript());
            moverObj.gameObject.AddComponent(new Sprite2D(50, 50));
            moverObj.gameObject.AddComponent(new ManagedNetworkComp(netMan));
            moverObj.Enabled = false;
            

            chatBox.OnTextEntered += (s, olds, box) =>
            {
                if (netMan.isClient)
                {
                    netMan.SendToSever(s);
                } else
                {
                    netMan.SendToClients(s);
                }
                chatBox.displayedString = "";
            };

            asHost.Anchor = new Vector2(0, 0);

            asHost.OnClick += (but) =>
            {
                if (netMan.Started) { return; }
                netMan.Start(true);
                App.SetTitle("Server");
                buttonPanel.gameObject.Destroy();
            };

            asClient.OnClick += (but) =>
            {
                if (netMan.Started) { return; }
                netMan.Start(false);
                moverObj.Enabled = true;
                moverObj.gameObject.GetComponent<ManagedNetworkComp>()!.Owned = true;
                App.SetTitle("Client");
                buttonPanel.gameObject.Destroy();
            };

            mainProject.Start();
            while (appOpen)
            {
                mainProject.Update();
                netMan.Update();
                App.Clear();
                mainProject.Render(App);
                App.Display();
            }
        }
    }
}