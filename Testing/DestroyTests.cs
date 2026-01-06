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
        public void TestGameObjectDestroy()
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

        [TestMethod]
        public void TestSceneDestroy()
        {
            GEWindow app = new GEWindow(new SFML.Window.VideoMode(512, 512), "Destroy Testing");
            Project newProject = new Project(null, app);

            Scene sceneA = newProject.CreateSceneAndLoad("SceneA");
            Scene sceneB = newProject.CreateScene("SceneB");
            Scene sceneC = newProject.CreateScene("SceneC");

            newProject.Start();

            newProject.Update(); // scene A gets time time to do whatever
            sceneA.Destory(); // component calls to destroy sceneA

            newProject.Update(); // scene A should be cleaned up by now

            if(newProject.ActiveScene == sceneA) { throw new Exception("Active scene is a destroyed scene!"); }
            newProject.LoadScene("SceneB");
            newProject.Update(); // sceneB loaded
            newProject.LoadScene("SceneC"); // component loads another scene
            sceneB.Destory(); // component destroys old scene

            newProject.Update(); // project gets a tick to respond

            if(newProject.ActiveScene == sceneB) { throw new Exception("Active scene is a destroyed scene!"); }
            if (sceneA.IsDestroyed == false) { throw new Exception("Scene wasnt destroyed properly!"); }
            if (sceneB.IsDestroyed == false) { throw new Exception("Scene wasnt destroyed properly!"); }

            if (newProject.GetScene("SceneA") != null) { throw new Exception("Destroyed Scene still in list!"); }
            if (newProject.GetScene("SceneAB") != null) { throw new Exception("Destroyed Scene still in list!"); }

            app.Close();
            return;
        }
    }
}
