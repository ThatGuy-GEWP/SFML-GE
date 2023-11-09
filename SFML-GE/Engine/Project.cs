


using SFML.Graphics;

namespace SFML_GE
{
    /// <summary>
    /// A Project holds scenes, and all resources.
    /// </summary>
    public class Project
    {
        public ResourceCollection Resources;

        public Scene? ActiveScene;

        public RenderWindow App;
        
        public Project(string ResourceDir, RenderWindow app) 
        {
            App = app;
            Resources = new ResourceCollection(ResourceDir);
        }

        public T GetResource<T>(string name) where T : Resource
        {
            return Resources.GetResourceByName<T>(name);
        }

        public Scene CreateScene()
        {
            return new Scene("Untitled", this);
        }

        public Scene CreateScene(string sceneName)
        {
            return new Scene(sceneName, this);
        }

        public Scene CreateSceneAndLoad()
        {
            ActiveScene = CreateScene();
            return ActiveScene;
        }

        public Scene CreateSceneAndLoad(string sceneName)
        {
            ActiveScene = CreateScene(sceneName);
            return ActiveScene;
        }

        public void Start()
        {
            if (ActiveScene is null) { return; }
            ActiveScene.Start();
        }

        public void Update()
        {
            if (ActiveScene is null) { return; }
            ActiveScene.Update();
        }

        public void Draw(RenderTarget rt)
        {
            if(ActiveScene is null) { return; }
            ActiveScene.Render(rt);
        }
    }
}
