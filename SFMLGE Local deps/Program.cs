using SFML.Graphics;
using SFML.Window;
using SFMLGE_Local_deps.Engine;
using SFMLGE_Local_deps.Scripts;

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

            GameObject testSlider = scene.CreateGameObject("Test slider");
            testSlider.AddComponent(new TestSlider());


            GameObject testObject = scene.CreateGameObject("Test object!");
            testObject.AddComponent(new Sprite2D(new Vector2(450, 450), new Vector2(0.5f, 0.5f))).Texture = mainProject.GetResource<TextureResource>("hardaf");
            testObject.Position = new Vector2(640, 360);

            ShaderResource testShad = mainProject.GetResource<ShaderResource>("shader.f");
            testShad.Resource.SetUniform("resolution", new SFML.Graphics.Glsl.Vec2(1280, 720));
            testShad.Resource.SetUniform("timeScale", new Vector2(5f, 0.1f));

            RenderTexture screenBuffer = new RenderTexture(1280, 720);
            Sprite screenSprite = new Sprite(mainProject.GetResource<TextureResource>("hardaf"));


            float time = 0;

            while (appOpen)
            {
                App.Clear();
                screenBuffer.Clear();

                time += mainProject.ActiveScene.deltaTime;
                mainProject.Update();
                mainProject.Render(screenBuffer);

                screenBuffer.Display();

                screenSprite.Texture = screenBuffer.Texture;
                screenSprite.Texture.Smooth = true;

                testShad.Resource.SetUniform("time", time);
                testShad.Resource.SetUniform("texture", Shader.CurrentTexture);

                App.Draw(screenSprite, testShad);

                App.Display();
            }
        }
    }
}