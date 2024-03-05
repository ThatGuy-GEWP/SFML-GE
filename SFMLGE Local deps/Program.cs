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


            GameObject GUIBase = scene.CreateGameObject("Testy!~");

            GUIContext context = GUIBase.AddComponent(new GUIContext(1280, 720));


            GUITextLabel textLabel = new GUITextLabel(context, "The quick brown fox\nJumped over the\nLazy dog", new Vector2(20, 20));


            GUIButton testButton = new GUIButton(context, new Vector2(120 * 2, 80 * 2));
            testButton.transform.WorldPosition = new Vector2(20, 110);


            GUIPanel buttonPannel = new GUIPanel(context, new Vector2(120*2, 80*2));
            buttonPannel.transform.parent = testButton.transform;

            buttonPannel.panelContent = mainProject.GetResource<TextureResource>("testImg");

            testButton.OnHoveringStart += (button) =>
            {
                buttonPannel.backgroundColor = GUIComponent.defaultPressed;
            };

            testButton.OnHoveringEnd += (button) =>
            {
                buttonPannel.backgroundColor = GUIComponent.defaultBackground;
            };

            for(int i = 0; i < 5; i++)
            {
                GUIButtonPannel butPan = new GUIButtonPannel(context);
                butPan.transform.WorldPosition = new Vector2(350, 40 + 60*i);
            }

            GUIButtonLabel butPanLabel = new GUIButtonLabel(context, "Test label button");
            butPanLabel.transform.WorldPosition = new Vector2(350, 40 + 60 * 5);

            float t = -0.1f;

            float ft = 0;
            while (appOpen)
            {
                App.Clear();
                t += scene.deltaTime * 2f;

                ft = t > 1.0f ? 1.0f : t < 0.0f ? 0.0f : t;
                t = t > 2.0f ? 0.0f : t;

                mainProject.Update();
                mainProject.Render(App);

                App.Display();
            }
        }
    }
}