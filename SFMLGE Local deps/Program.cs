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
            mainProject.Start();

            GameObject GUIBase = scene.CreateGameObject("Testy!~");

            GUIContext context = GUIBase.AddComponent(new GUIContext(1280, 720));


            GUITextLabel textLabel = new GUITextLabel(context, "The quick brown fox\nJumped over the\nLazy dog", new Vector2(20, 20));

            GUITextLabel textLabel2 = new GUITextLabel(context, "\r\n\r\nLorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque vel ligula consectetur nisi efficitur convallis eu ut enim. Donec nec ligula tincidunt, bibendum ligula vitae, ornare nisi. Donec scelerisque quam eget imperdiet luctus. Quisque tempor ipsum sit amet molestie laoreet. Nullam pretium euismod luctus. Curabitur interdum tellus sit amet iaculis lacinia. Maecenas dui mi, bibendum vitae suscipit nec, vulputate quis orci. Fusce vel lacinia ipsum, nec viverra lacus. Nullam at turpis justo. Vestibulum tincidunt, diam et tincidunt blandit, lectus quam feugiat tortor, eu porta purus justo id lectus. Aliquam luctus mi quis ex elementum maximus.\r\n\r\nQuisque maximus felis ac elit scelerisque, nec blandit augue volutpat. Sed vestibulum purus turpis, eget imperdiet urna elementum non. Aliquam fermentum nisl id nulla condimentum auctor. Fusce dignissim purus ut fringilla dapibus. Phasellus mauris turpis, auctor a risus et, accumsan pulvinar metus. Nulla felis justo, tempor tincidunt aliquam ac, ullamcorper ac lectus. Suspendisse ac nulla lobortis, fermentum arcu ac, fermentum arcu.\r\n\r\nMorbi ut fermentum risus, sit amet tristique erat. Phasellus purus orci, tincidunt et arcu id, suscipit suscipit urna. Nam molestie orci ut nisi pharetra, vitae placerat est gravida. Quisque tristique, nunc gravida efficitur pellentesque, ligula nulla posuere lacus, sed commodo neque erat ac dui. Donec in feugiat elit, sit amet convallis leo. Sed dictum pulvinar tortor id facilisis. Donec ullamcorper elit neque, at sodales risus vehicula ac. Donec at elementum justo. Nam sit amet posuere mi. Vivamus dictum dapibus gravida. Integer semper velit non faucibus accumsan.\r\n\r\nUt pretium quis turpis in blandit. Phasellus nec turpis feugiat, dictum enim nec, mollis velit. Nunc dictum vel erat eu varius. Duis eu elit a ex porta ultrices. Ut sodales ultrices purus, faucibus porttitor mi porta in. Pellentesque massa diam, cursus ut nulla at, auctor euismod elit. Curabitur eget malesuada nibh. Pellentesque condimentum eros et lectus ultrices ultricies. Integer id cursus urna, ac feugiat nisi. Quisque pellentesque eu tortor non cursus.\r\n\r\nInteger vulputate tortor ipsum, a egestas elit lacinia id. Sed efficitur neque arcu. Aliquam lobortis porta risus vel volutpat. Duis non erat iaculis, porta ligula placerat, mattis ex. Aliquam scelerisque felis non malesuada interdum. Nullam efficitur pretium arcu eleifend maximus. Quisque sed sagittis arcu. Duis luctus tristique semper. In convallis nibh finibus, dignissim risus eu, volutpat orci. Sed eget leo sagittis, lacinia lorem nec, sodales felis. Sed sed urna ante. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Morbi scelerisque sollicitudin sodales. Nunc non dolor erat. ", new Vector2(600, 20));
            textLabel2.charSize = 5;
            textLabel2.transform.WorldPosition = new Vector2(20, 600);

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

                butPan.button.OnClick += (button) =>
                {
                    butPan.panel.outlineThickness = butPan.panel.outlineThickness > 2f ? 2f : 5f;
                    butPan.panel.indentedCorners = !butPan.panel.indentedCorners;
                };
            }

            GUIButtonLabel butPanLabel = new GUIButtonLabel(context, "Slide da thing");
            butPanLabel.transform.WorldPosition = new Vector2(350, 40 + 60 * 5);

            GUIPanel pinPan = new GUIPanel(context);

            GUIScroller scroll = new GUIScroller(context);

            GUIButtonPannel scrollAdder = new GUIButtonPannel(context);

            scrollAdder.transform.WorldPosition = new Vector2(765, 20);
            scrollAdder.button.OnClick += (button) =>
            {
                scroll.content.Add("Test:" + RandomGen.Next(0,2000)*0.1f);
            };

            bool swipswap = false;
            float t = 0.0f;

            butPanLabel.button.OnClick += (button) =>
            {
                swipswap = !swipswap;
            };

            while (appOpen)
            {
                App.Clear();

                t = swipswap ? MathGE.Lerp(t, 1.0f, scene.deltaTime * 5f) : MathGE.Lerp(t, 0.0f, scene.deltaTime * 5f);

                pinPan.transform.WorldPosition = new Vector2(MathGE.Interpolation.SmoothStep(350, 500, t), 500);

                mainProject.Update();
                mainProject.Render(App);

                App.Display();
            }
        }
    }
}