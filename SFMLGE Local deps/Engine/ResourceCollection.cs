﻿using SFML.Graphics;
using SFMLGE_Local_deps.Engine;
using System.Runtime.CompilerServices;

namespace SFML_Game_Engine
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
        /// Collects resources from a directory, pass <see cref="null"/> if you plan on adding your own manualy.
        /// </summary>
        /// <param name="dirToCollect"></param>
        public ResourceCollection(string? dirToCollect, Project project)
        {
            linkedProject = project;
            CollectDir(dirToCollect);

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

        void searchFolder(string path, bool isRoot)
        {
            if (!Directory.Exists(path)) { return; }

            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file);
                string fileName = Path.GetFileName(file);

                bool loadedSomething = false;

                string name = file.Replace(extension, "").Replace(rootName + "\\", "").Replace("\\", "/");
                Console.Write("loading ");

                if (extension == ".png" || extension == ".jpg")
                {
                    Console.Write("" + name);
                    resources.Add(new TextureResource(file, name));
                    loadedSomething = true;
                }

                if (extension == ".wav" || extension == ".ogg")
                {
                    Console.Write("" + name);
                    if (extension == ".wav")
                    {
                        Console.Write(" | Warning! .wav files are slow to load, use .ogg instead!");
                    }

                    resources.Add(new SoundResource(file, name));
                    loadedSomething = true;
                }

                if (extension == ".frag" || extension == ".vert")
                {
                    if(extension == ".frag")
                    {
                        resources.Add(new ShaderResource(name + ".f", null, null, file));
                        Console.Write("" + name + ".f");
                    }
                    if (extension == ".vert")
                    {
                        resources.Add(new ShaderResource(name + ".v", file, null, null));
                        Console.Write("" + name + ".v");
                    }
                }

                if(extension == ".ttf")
                {
                    Console.Write("" + name);
                    resources.Add(new FontResource(file, name));
                    loadedSomething = true;
                }
                Console.Write("\n");
                if (!loadedSomething) { continue; }
            }

            string[] dirs = Directory.GetDirectories(path);

            foreach (string directory in dirs)
            {
                searchFolder(directory, false);
            }
        }

        public void CollectDir(string? dirToCollect)
        {
            if (dirToCollect == null) { Console.Write("Loading no resources."); return; }
            rootName = dirToCollect;

            Console.WriteLine($"Loading folder {dirToCollect}...");
            searchFolder(dirToCollect, true);
            for(int i = 0; i < resources.Count; i++) // duplicates check
            {
                Resource curRes = resources[i];
                for(int j = 0; j < resources.Count; j++)
                {
                    if(j == i) { continue; }
                    if (resources[j].name == curRes.name)
                    {
                        Console.WriteLine("Duplicate " + resources[j].name + ", removing...");
                        resources.RemoveAt(j);
                    }
                }
            }
            Console.WriteLine($"\rFinished loading "+(resources.Count-1)+" resources in "+dirToCollect+"!");
        }

        /// <summary>
        /// Adds a <see cref="Resource"/> to this collection.
        /// </summary>
        /// <param name="resource"></param>
        public void AddResource(Resource resource)
        {
            resources.Add(resource);
        }

        /// <summary>
        /// Finds a <see cref="Resource"/> by name or full path, case sensitive, raises exception if resource could not be found
        /// </summary>
        /// <typeparam name="T">Type of resource</typeparam>
        /// <param name="name">Name of the resource</param>
        /// <returns></returns>
        public T GetResource<T>(string name) where T : Resource
        {
            Resource? secondaryMatch = null;

            foreach (Resource resource in resources)
            {
                if (resource.name == name)
                {
                    if (resource is T)
                    {
                        return (T)resource;
                    }
                }
                if (resource.name.Contains(name))
                {
                    if (resource is T)
                    {
                        secondaryMatch = resource;
                    }
                }
            }
            if(secondaryMatch != null)
            {
                return (T)secondaryMatch;
            }
            
            throw new NullReferenceException($"Resource '{name}' could not be found!");
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
                if (r.GetType() == typeof(T) && r.name.Contains(filter))
                {
                    foundResources.Add((T)r);
                }
            });
            return foundResources;
        }
    }
}
