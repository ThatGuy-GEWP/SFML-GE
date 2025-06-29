using SFML_GE.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    [TestClass]
    public class DestroyTests
    {
        [TestMethod]
        public void TestDestroy()
        {
            GEWindow app = new GEWindow(new SFML.Window.VideoMode(512, 512), "Destroy Testing");
            Project newProject = new Project(null, app);
            Scene testScene = newProject.CreateSceneAndLoad("TestScene");

            app.SetFramerateLimit(60);

            newProject.Start();

            newProject.Update();
            testScene.CreateGameObject("Test GO");
            testScene.CreateGameObject("Test GO 2", testScene.GetGameObject("Test GO"));
            newProject.Update();
            testScene.GetGameObject("Test GO")!.Destroy();
            newProject.Update();

            if (testScene.GetGameObject("Test GO") != null)
            {
                throw new Exception("Destroyed GameObject was gotten somehow!");
            }


            app.Close();
            return;
        }
    }
}
