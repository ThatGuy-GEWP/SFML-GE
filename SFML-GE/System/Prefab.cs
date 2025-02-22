namespace SFML_GE.System
{

    // this class is probably not secure at all but i dont give a shit!
    // if anything has somehow gotten deep enough to set anonymous function its someones elses problem!

    /// <summary>
    /// Prefab's are instructions for creating GameObject's
    /// To do so, simply create a function for the <see cref="CreatePrefab"/> delagate
    /// </summary>
    public class Prefab : Resource
    {
        /// <summary>
        /// Creates this prefab the returns the <see cref="GameObject"/> containing it.
        /// </summary>
        public Func<Project, Scene, GameObject> CreatePrefab;

        /* Example code for people who are new to C#
         * 
         * Prefab myPrefab = new Prefab("myPrefab", (project, scene) => { return scene.CreateGameObject("test!"); });
         *                                                                  ^- you MUST return a GameObject, dont forget!
         * GameObject prefabInstance = myScene.InstanciatePrefab(myPrefab)
         */

        /// <summary>
        /// Creates a new prefab.
        /// </summary>
        /// <param name="name">the (SHOULD BE UNIQUE) name of this <see cref="Resource"/></param>
        /// <param name="createPrefab">
        ///     A Function that takes a <see cref="Project"/> and a <see cref="Scene"/>
        ///     then returns a <see cref="GameObject"/> containing the prefab you want to instance.
        /// </param>
        public Prefab(string name, Func<Project, Scene, GameObject> createPrefab)
        {
            Name = name;
            CreatePrefab = createPrefab;
        }

        /// <summary>
        /// Does nothing for this <see cref="Resource"/> type.
        /// </summary>
        public override void Dispose()
        {
            return;
        }
    }
}
