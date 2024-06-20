namespace SFML_Game_Engine.Engine.System
{

    // this class is probably not secure at all but i dont give a shit!
    // if anything has somehow gotten deep enough to set anonymous function its someones elses problem!

    /// <summary>
    /// Prefab's are instructions for creating GameObject's
    /// To do so, simply create a function for the <see cref="CreatePrefab"/> delagate
    /// </summary>
    public class Prefab : Resource
    {
        public Func<Project, Scene, GameObject> CreatePrefab;

        /* Example code for people who are new to C#
         * 
         * Prefab myPrefab = new Prefab("myPrefab", (project, scene) => { return scene.CreateGameObject("test!"); });
         *                                                                  ^- you MUST return a GameObject, dont forget!
         * GameObject prefabInstance = myScene.InstanciatePrefab(myPrefab)
         */

        public Prefab(string name, Func<Project, Scene, GameObject> createPrefab)
        {
            Name = name;
            CreatePrefab = createPrefab;
        }

        public override void Dispose()
        {
            return;
        }
    }
}
