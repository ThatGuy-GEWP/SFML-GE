using SFML.Graphics;
using SFML_GE.Resources;

namespace SFML_GE.System
{
    /// <summary>
    /// A Collection of <see cref="Resource"/>'s, used primarily to have all your assets in one place.
    /// </summary>
    public class ResourceCollection
    {
        /// <summary>
        /// A list containing all the resources in this project.
        /// </summary>
        readonly List<Resource> allResources = new List<Resource>();
        readonly Dictionary<string, Resource> nameResPairs = new Dictionary<string, Resource>();

        readonly string rootName = string.Empty;

        static bool _writeWarnings = true;

        /// <summary>
        /// If true, warnings about file types will be writen to console.
        /// </summary>
        public static bool WriteWarnings
        {
            get { return _writeWarnings; }
            set { _writeWarnings = value; }
        }

        /// <summary>
        /// Collects resources from a directory, pass <c>null</c> if you plan on adding your own manualy.
        /// </summary>
        public ResourceCollection(string? dirToCollect)
        {
            DebugLogger.LogInfo("Max Texture size is : " + Texture.MaximumSize + "x" + Texture.MaximumSize);

            DebugLogger.LogInfo($"Loading folder {dirToCollect}...");
            if (dirToCollect != null) { rootName = dirToCollect; CollectFolder(dirToCollect); }

            Image defaultSpriteImg = new Image(25, 25);
            for (uint x = 0; x < 25; x++)
            {
                for (uint y = 0; y < 25; y++)
                {
                    defaultSpriteImg.SetPixel(x, y, Color.White);
                }
            }

            Add(new TextureResource(new Texture(defaultSpriteImg), "DefaultSprite"));
            defaultSpriteImg.Dispose();

            DebugLogger.LogInfo($"Finished loading {allResources.Count} resources in '{dirToCollect}'!");
        }

        void CollectFolder(string path)
        {
            if (!Directory.Exists(path)) { return; }

            EnumerationOptions enumOps = new EnumerationOptions();
            enumOps.AttributesToSkip = FileAttributes.Hidden | FileAttributes.System;
            enumOps.RecurseSubdirectories = true;

            List<string> filteredFiles = Directory
                .EnumerateFiles(path, "*", enumOps)
                .Where(file =>
                    file.ToLower().EndsWith(".png") ||
                    file.ToLower().EndsWith(".jpg") ||
                    file.ToLower().EndsWith(".jpeg") ||
                    file.ToLower().EndsWith(".wav") ||
                    file.ToLower().EndsWith(".ogg") ||
                    file.ToLower().EndsWith(".ttf") ||
                    file.ToLower().EndsWith(".otf") ||
                    file.ToLower().EndsWith(".vert") ||
                    file.ToLower().EndsWith(".frag")
                    )
                .ToList();

            string[] files = filteredFiles.ToArray();

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file);

                bool loadedSomething = false;

                string name = file.Replace(extension, "").Replace(rootName + "\\", "").Replace("\\", "/");

                loadedSomething = LoadFile(file, name, extension);
            }
        }

        private bool LoadFile(string file, string name, string extension)
        {
            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                try
                {
                    TextureResource res = new TextureResource(file, name);
                    Add(res);
                    return true;
                }
                catch (Exception ex)
                {
                    DebugLogger.LogError("Failed to load " + name + "! Exception:" + ex.ToString());
                }
            }

            if (extension == ".wav" || extension == ".ogg")
            {
                if (extension == ".wav" && WriteWarnings)
                {
                    DebugLogger.LogWarning($".wav files [{file}] are slow to load, use .ogg instead!");
                }

                Add(new SoundResource(file, name));
                return true;
            }

            if (extension == ".frag" || extension == ".vert")
            {
                if (extension == ".frag")
                {
                    Add(new ShaderResource(name + ".frag", null, null, file));
                }
                if (extension == ".vert")
                {
                    Add(new ShaderResource(name + ".vert", file, null, null));
                }
            }

            if (extension == ".ttf" || extension == ".otf")
            {
                Add(new FontResource(file, name));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a <see cref="Resource.Name"/> is taken.
        /// </summary>
        /// <param name="name">The name to check</param>
        /// <returns>True if resource of name <paramref name="name"/> is within this collection</returns>
        public bool NameTaken(string name)
        {
            return nameResPairs.ContainsKey(name);
        }

        /// <summary>
        /// Gets a list of all resources in this collection.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of <see cref="Resource"/>'s this collection represents</returns>
        public List<Resource> GetAllResources()
        {
            return allResources;
        }

        /// <summary>
        /// Tries to add a <see cref="Resource"/> to this collection.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>True if resource name is not already taken and was added, false otherwise</returns>
        public bool Add(Resource resource)
        {
            DebugLogger.LogInfo("Loaded " + resource.Name + " as " + resource.GetType().Name);

            if (!nameResPairs.ContainsKey(resource.Name))
            {
                allResources.Add(resource);
                nameResPairs.Add(resource.Name, resource);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a <see cref="Resource"/> from this collection.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>True if resource is removed, false otherwise</returns>
        public bool Remove(Resource resource)
        {
            // should be fine
            bool removed = false;

            removed |= allResources.Remove(resource);
            removed |= nameResPairs.Remove(resource.Name);

            return removed;
        }

        /// <summary>
        /// Finds a <see cref="Resource"/> by name, case sensitive.
        /// returns null if the resource could not be found
        /// </summary>
        /// <typeparam name="T">Type of resource</typeparam>
        /// <param name="name">Name of the resource</param>
        /// <returns>The found resource cast to <typeparamref name="T"/>, or null if not found.</returns>
        public T? GetResource<T>(string name) where T : Resource
        {
            bool got = nameResPairs.TryGetValue(name, out Resource? res);
            if (got && res != null)
            {
                res.requests++;
                return (T)res;
            }
            DebugLogger.LogDebug($"Could not get resource with name \"{name}\"");
            return null;
        }

        /// <summary>
        /// Finds a <see cref="Resource"/> by name, case sensitive.
        /// returns null if the resource could not be found
        /// </summary>
        /// <param name="name">Name of the resource</param>
        /// <returns>The found resource as a generic <see cref="Resource"/>, or null if not found.</returns>
        public Resource? GetResource(string name)
        {
            bool got = nameResPairs.TryGetValue(name, out Resource? res);
            if (got && res != null)
            {
                res.requests++;
                return res;
            }

            return null;
        }

        /// <summary>
        /// Creates a list containing all resources that are of a type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to filter</typeparam>
        /// <returns>A list of found resources, empty if none are found.</returns>
        public List<T> GetResourcesWithType<T>() where T : Resource
        {
            List<T> found = new List<T>();

            for (int i = 0; i < allResources.Count; i++)
            {
                if (allResources[i].GetType() == typeof(T) || allResources[i] is T)
                {
                    found.Add((T)allResources[i]);
                }
            }

            return found;
        }

        /// <summary>
        /// Creates a list containing all resources that are of a given type <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type to filter</param>
        /// <returns>A list of found resources, empty if none are found.</returns>
        public List<Resource> GetResourcesWithType(Type type)
        {
            List<Resource> found = new List<Resource>();

            for (int i = 0; i < allResources.Count; i++)
            {
                if (allResources[i].GetType() == type)
                {
                    found.Add(allResources[i]);
                }
            }

            return found;
        }
    }
}
