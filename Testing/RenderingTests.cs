using SFML.Graphics;
using SFML_GE.Components;
using SFML_GE.Resources;
using SFML_GE.System;
using System.Diagnostics;

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

            for (int i = 0; i < 60; i++) // 60fps, 180 frames should take 3 seconds or less
            {
                newProject.Update();

                testObj.transform.GlobalPosition = new Vector2(15 + MathGE.ZeroSin(i * 0.5f) * (1280 - 165), 15 + MathGE.ZeroSin(i * 0.4f) * (720 - 165));

                if(i > 0 && i % 8 == 0)
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

        [TestMethod]
        public void TestDelegatedRendering()
        {
            RenderWindow app = new RenderWindow(new SFML.Window.VideoMode(512, 512), "Delegated Rendering test");
            Project newProject = new Project("", app);
            Scene testScene = newProject.CreateSceneAndLoad("TestScene");

            RectangleShape rs = new RectangleShape();
            rs.Size = new SFML.System.Vector2f(15, 15);

            app.SetFramerateLimit(60);

            Stopwatch totalTime = Stopwatch.StartNew();
            for (int i = 0; i < 60; i++) // 60fps, 180 frames should take 3 seconds or less
            {
                newProject.Update();

                testScene.RenderManager.AddToQueue((rt) => { rt.Draw(rs); }, 5);

                app.Clear();
                newProject.Render(app);
                app.Display();
            }
            totalTime.Stop();
            Console.WriteLine($"took {totalTime.Elapsed.TotalSeconds} to render whole scene, expected should be 1s");

            app.Close();
            return;
        }

        [TestMethod]
        public void TestRichTextRendering()
        {
            RenderWindow app = new RenderWindow(new SFML.Window.VideoMode(512, 512), "Rich-Text test");
            Project newProject = new Project("Res", app);
            Scene testScene = newProject.CreateSceneAndLoad("TestScene");

            app.SetFramerateLimit(60);

            RichText rt = new RichText(newProject.GetResource<FontResource>("Roboto-Regular")!.resource);

            rt.DisplayedString = "<crgb 255, 0, 0>Should be red<r> <crgb 0, 255, 0>Should be green<r>";
            rt.RichEnabled = true;

            Stopwatch totalTime = Stopwatch.StartNew();
            for (int i = 0; i < 60; i++)
            {
                newProject.Update();

                app.Clear();
                newProject.Render(app);
                rt.Draw(app, RenderStates.Default);
                app.Display();
            }
            totalTime.Stop();
            Console.WriteLine($"took {totalTime.Elapsed.TotalSeconds} to render whole scene, expected should be 1s");
        }
    }
}
