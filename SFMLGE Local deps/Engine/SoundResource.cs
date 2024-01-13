﻿using SFML.Audio;

namespace SFML_Game_Engine
{
    public class SoundResource : Resource
    {
        public SoundBuffer resource;

        public SoundResource(string path, string name) 
        {
            this.name = name;
            resource = new SoundBuffer(path);
        }

        public override void Dispose()
        {
            resource.Dispose();
        }

        public static implicit operator SoundBuffer(SoundResource res) { return res.resource; }
    }
}
