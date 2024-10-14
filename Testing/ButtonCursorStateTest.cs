using Microsoft.VisualStudio.TestTools.UnitTesting;
using SFML.Graphics;
using SFML_GE.Editor;
using SFML_GE.GUI;
using SFML_GE.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    [TestClass]
    public class ButtonCursorStateTest
    {
        // For manually testing if button stuffs work, il set up automation later when im not sleep deprived.
        [TestMethod]
        public void TestButton()
        {
            if (true) { return; }

#pragma warning disable CS0162 // Unreachable code detected
            RenderWindow app = new RenderWindow(new SFML.Window.VideoMode(1280, 720), "Editor Testing");
            Project newProject = new Project(null, app);
            Scene testScene = newProject.CreateSceneAndLoad("TestScene");

            GUIButton testButton = testScene.CreateGameObjectWithComp(new GUIButton(), "test");


            app.SetFramerateLimit(60);

            for (int i = 0; i < 1800; i++) // 60fps, 300 frames should take 5 seconds or less
            {
                Assert.IsTrue(newProject.ActiveScene != null);
                newProject.Update();

                app.Clear();
                newProject.Render(app);
                app.Display();
            }

            app.Close();
            return;
#pragma warning restore CS0162 // Unreachable code detected
        }

    }
}
