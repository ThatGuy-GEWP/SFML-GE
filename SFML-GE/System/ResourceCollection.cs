using SFML.Graphics;
using SFML_GE.Resources;

namespace SFML_GE.System
{
    /// <summary>
    /// A Collection of <see cref="Resource"/>'s, used primarily to have all your assets in one place.
    /// (also preventing memory leaks/buildup with SFML's texture and SoundBuffer class)
    /// </summary>
    public class ResourceCollection
    {
        public List<Resource> resources = new List<Resource>();

        Project linkedProject;

        string rootName = string.Empty;

        /// <summary>
        /// Collects resources from a directory, pass <c>null</c> if you plan on adding your own manualy.
        /// </summary>
        public ResourceCollection(string? dirToCollect, Project project)
        {
            linkedProject = project;

            Console.WriteLine("Max Texture size is : " + Texture.MaximumSize + "x" + Texture.MaximumSize);

            LoadDir(dirToCollect);

            Image defaultSpriteImg = new Image(25, 25);
            for (uint x = 0; x < 25; x++)
            {
                for (uint y = 0; y < 25; y++)
                {
                    defaultSpriteImg.SetPixel(x, y, Color.White);
                }
            }

            resources.Add(new TextureResource(new Texture(defaultSpriteImg), "DefaultSprite"));
            defaultSpriteImg.Dispose();
        }

        void CollectFolder(string path, bool isRoot)
        {
            if (!Directory.Exists(path)) { return; }

            EnumerationOptions enumOps = new EnumerationOptions();
            enumOps.AttributesToSkip = FileAttributes.Hidden;
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
                    file.ToLower().EndsWith(".vert") ||
                    file.ToLower().EndsWith(".frag")
                    )
                .ToList();

            string[] files = filteredFiles.ToArray();

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file);
                string fileName = Path.GetFileName(file);

                bool loadedSomething = false;

                string name = file.Replace(extension, "").Replace(rootName + "\\", "").Replace("\\", "/");
                Console.Write("loading ");

                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                {
                    Console.Write("" + name + " as TextureResource");

                    try
                    {
                        TextureResource res = new TextureResource(file, name);
                        resources.Add(new TextureResource(file, name));
                        loadedSomething = true;
                        // Console.Write(" " + (res.Resource.Size.X + "x" + res.Resource.Size.Y) + "|" + MathF.Round(((res.Resource.Size.X + res.Resource.Size.Y)*4f)/1000f) + " megabytes");
                        // above just in case you need to keep track of mem usage.
                    }
                    catch (Exception ex)
                    {
                        Console.Write("Failed to load " + name + "! Exception:" + ex.ToString());
                    }
                }

                if (extension == ".wav" || extension == ".ogg")
                {
                    Console.Write("" + name + " as SoundResource");
                    if (extension == ".wav")
                    {
                        Console.Write(" | Warning! .wav files are uncompressed and slow to load, use .ogg for faster loading!");
                    }

                    resources.Add(new SoundResource(file, name));
                    loadedSomething = true;
                }

                if (extension == ".frag" || extension == ".vert")
                {
                    if (extension == ".frag")
                    {
                        resources.Add(new ShaderResource(name + ".f", null, null, file));
                        Console.Write("" + name + " as a Fragment only ShaderResource");
                    }
                    if (extension == ".vert")
                    {
                        resources.Add(new ShaderResource(name + ".v", file, null, null));
                        Console.Write("" + name + " as a Vertex only ShaderResource");
                    }
                }

                if (extension == ".ttf")
                {
                    Console.Write("" + name + " as a FontResource");
                    resources.Add(new FontResource(file, name));
                    loadedSomething = true;
                }
                Console.Write("\n");
                if (!loadedSomething) { continue; }
            }
        }

        public bool LoadResource(string file)
        {
            string? extension = Path.GetExtension(file);
            string? fileName = Path.GetFileName(file);

            if (extension == null || fileName == null) { return false; }

            string name = file.Replace(extension, "").Replace(rootName + "\\", "").Replace("\\", "/");
            Console.Write("loading ");

            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                Console.Write(name + " as TextureResource\n");

                try
                {
                    TextureResource res = new TextureResource(file, name);
                    resources.Add(new TextureResource(file, name));
                    return true;
                }
                catch (Exception ex)
                {
                    Console.Write("Failed to load " + name + "! Exception:" + ex.ToString());
                }
            }

            if (extension == ".wav" || extension == ".ogg")
            {
                Console.Write("" + name + " as SoundResource");
                if (extension == ".wav")
                {
                    Console.Write(" | Warning! .wav files are uncompressed and slow to load, use .ogg for faster loading!");
                }
                Console.Write("\n");
                resources.Add(new SoundResource(file, name));
                return true;
            }

            if (extension == ".frag" || extension == ".vert")
            {
                if (extension == ".frag")
                {
                    resources.Add(new ShaderResource(name + ".f", null, null, file));
                    Console.Write("" + name + " as a Fragment only ShaderResource\n");
                }
                if (extension == ".vert")
                {
                    resources.Add(new ShaderResource(name + ".v", file, null, null));
                    Console.Write("" + name + " as a Vertex only ShaderResource\n");
                }
                return true;
            }

            if (extension == ".ttf")
            {
                Console.Write("" + name + " as a FontResource\n");
                resources.Add(new FontResource(file, name));
                return true;
            }

            return false;
        }

        public void LoadDir(string? dirToCollect)
        {
            if (dirToCollect == null) { Console.Write("Loading no resources."); return; }
            int countAtStart = resources.Count;
            rootName = dirToCollect;

            Console.WriteLine($"Loading folder {dirToCollect}...");
            CollectFolder(dirToCollect, true);
            for (int i = 0; i < resources.Count; i++) // duplicates check
            {
                Resource curRes = resources[i];
                for (int j = 0; j < resources.Count; j++)
                {
                    if (j == i) { continue; }
                    if (resources[j].Name == curRes.Name)
                    {
                        Console.WriteLine("Duplicate " + resources[j].Name + ", removing...");
                        resources.RemoveAt(j);
                    }
                }
            }
            Console.WriteLine($"\rFinished loading " + (resources.Count - countAtStart) + " resources in " + dirToCollect + "!");
        }

        /// <summary>
        /// Adds a <see cref="Resource"/> to this collection.
        /// </summary>
        /// <param name="resource"></param>
        public void AddResource(Resource resource)
        {
            Console.WriteLine("(Runtime) Loaded " + resource.Name + " as " + resource.GetType().Name);
            resources.Add(resource);
        }

        /// <summary>
        /// Finds a <see cref="Resource"/> by name or full path, case sensitive, returns null if the resource could not be found
        /// </summary>
        /// <typeparam name="T">Type of resource</typeparam>
        /// <param name="name">Name of the resource</param>
        /// <returns></returns>
        public T? GetResource<T>(string name) where T : Resource
        {
            Resource? secondaryMatch = null;

            foreach (Resource resource in resources)
            {
                if (resource.Name == name)
                {
                    if (resource is T)
                    {
                        resource.requests++;
                        return (T)resource;
                    }
                }
                if (resource.Name.Contains(name))
                {
                    if (resource is T)
                    {
                        resource.requests++;
                        secondaryMatch = resource;
                    }
                }
            }
            if (secondaryMatch != null)
            {
                secondaryMatch.requests++;
                return (T)secondaryMatch;
            }

            return null;
        }

        /// <summary>
        /// Finds a <see cref="Resource"/> by name or full path, case sensitive, returns null if the resource could not be found
        /// </summary>
        /// <param name="resourceType">Type of resource</param>
        /// <param name="name">Name of the resource</param>
        /// <returns></returns>
        public Resource? GetResource(string name, Type resourceType)
        {
            Resource? secondaryMatch = null;

            foreach (Resource resource in resources)
            {
                if (resource.Name == name)
                {
                    if (resource.GetType() == resourceType)
                    {
                        resource.requests++;
                        return resource;
                    }
                }
                if (resource.Name.Contains(name))
                {
                    if (resource.GetType() == resourceType)
                    {
                        resource.requests++;
                        secondaryMatch = resource;
                    }
                }
            }
            if (secondaryMatch != null)
            {
                secondaryMatch.requests++;
                return secondaryMatch;
            }

            return null;
        }

        /// <summary>
        /// Gets all resources whos name contains <paramref name="filter"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<T> GetResourcesWith<T>(string filter) where T : Resource // still dont know why i made this return list but whatever
        {
            List<T> foundResources = new List<T>();

            resources.ForEach(r =>
            {
                if (r.GetType() == typeof(T) && r.Name.Contains(filter))
                {
                    foundResources.Add((T)r);
                }
            });
            return foundResources;
        }
    }
}
