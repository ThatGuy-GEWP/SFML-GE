using SFML.Graphics;
using SFML_GE.Components;
using SFML_GE.System;

namespace Testing
{
    [TestClass]
    public class RenderingTests
    {
        [TestMethod]
        public void TestSceneClearing()
        {
            RenderWindow app = new RenderWindow(new SFML.Window.VideoMode(512, 512), "Screen-Clearing test");
            Project newProject = new Project("", app);
            Scene testScene = newProject.CreateSceneAndLoad("TestScene");

            GameObject testObj = testScene.CreateGameObject("RenderTest");

            Sprite2D spirte = testObj.AddComponent(new Sprite2D(150, 150));
            spirte.offset = new Vector2(0, 0);

            app.SetFramerateLimit(60);

            app.RequestFocus();

            for (int i = 0; i < 300; i++) // 60fps, 180 frames should take 3 seconds or less
            {
                newProject.Update();

                testObj.transform.GlobalPosition = new Vector2(15 + MathGE.ZeroSin(i * 0.1f) * (1280 - 165), 15 + MathGE.ZeroSin(i * 0.4f) * (720 - 165));

                if(i > 0 && i % 60 == 0)
                {
                    testScene.ClearScreen = !testScene.ClearScreen;
                    testScene.ClearColor = new Color((byte)RandomGen.Next(255), (byte)RandomGen.Next(255), (byte)RandomGen.Next(255));
                }

                app.Clear();
                newProject.Render(app);
                app.Display();
            }

            app.Close();
            return;
        }
    }
}
