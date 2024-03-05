using SFML.Graphics;
using SFML.Window;
using SFML_Game_Engine.GUI;

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


            GUITextLabel textLabel = new GUITextLabel("The quick brown fox\nJumped over the\nLazy dog");

            context.AddComponent(textLabel);

            GUIButton testButton = new GUIButton();

            testButton.transform.WorldPosition = new Vector2(100, 100);


            GUIPanel buttonPannel = new GUIPanel(new Vector2(150, 50));
            buttonPannel.transform.parent = testButton.transform;


            testButton.OnHoveringStart += (button) =>
            {
                buttonPannel.backgroundColor = GUIComponent.defaultPressed;
            };

            testButton.OnHoveringEnd += (button) =>
            {
                buttonPannel.backgroundColor = GUIComponent.defaultBackground;
            };

            context.AddComponent(buttonPannel);
            context.AddComponent(testButton);

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