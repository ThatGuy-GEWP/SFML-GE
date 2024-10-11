using SFML.Graphics;
using SFML_GE.Editor;
using SFML_GE.System;

namespace Testing
{
    [TestClass]
    public class ProjectTests
    {
        [TestMethod]
        public void TestBasicLoop()
        {
            RenderWindow app = new RenderWindow(new SFML.Window.VideoMode(1280, 720), "Basic Loop Testing");
            Project newProject = new Project("", app);
            Scene testScene = newProject.CreateSceneAndLoad("TestScene");

            app.SetFramerateLimit(60);

            for(int i = 0; i < 60; i++) // should take a second or less
            {
                Assert.IsTrue(newProject.ActiveScene != null);
                newProject.Update();

                app.Clear();
                newProject.Render(app);
                app.Display();
            }

            app.Close();
            return;
        }

        [TestMethod]
        public void TestSceneTransitions()
        {
            RenderWindow app = new RenderWindow(new SFML.Window.VideoMode(1280, 720), "Scene-Transition Testing");
            Project newProject = new Project("", app);

            Scene SceneA = newProject.CreateScene("Scene A");
            Scene SceneB = newProject.CreateScene("Scene B");

            newProject.LoadScene(SceneA); // early load
            newProject.LoadScene(SceneB);
            newProject.LoadScene(SceneA);

            app.SetFramerateLimit(60);

            for (int i = 0; i < 60; i++) // should take a second or less
            {
                Assert.IsTrue(newProject.ActiveScene != null);
                if (i == 10)
                {
                    newProject.LoadScene(SceneB);
                }
                if(i == 20)
                {
                    Assert.AreEqual("Scene B", newProject.ActiveScene!.Name);
                    newProject.LoadScene(SceneA);
                }
                if(i == 25)
                {
                    Assert.AreEqual("Scene A", newProject.ActiveScene!.Name);
                }
                newProject.Update();

                app.Clear();
                newProject.Render(app);
                app.Display();
            }

            app.Close();
            return;
        }

        [TestMethod]
        public void TestSceneWithEditor()
        {
            RenderWindow app = new RenderWindow(new SFML.Window.VideoMode(1280, 720), "Editor Testing");
            Project newProject = new Project("", app);
            Scene testScene = newProject.CreateSceneAndLoad("TestScene");

            GUIEditor editor = testScene.CreateGameObjectWithComp(new GUIEditor(newProject), "EditorHolder");

            app.SetFramerateLimit(60);

            for (int i = 0; i < 60; i++) // 60fps, 300 frames should take 5 seconds or less
            {
                Assert.IsTrue(newProject.ActiveScene != null);
                newProject.Update();

                app.Clear();
                newProject.Render(app);
                app.Display();
            }

            app.Close();
            return;
        }
    }
}