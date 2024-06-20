using SFML.Audio;
using SFML_Game_Engine.Engine.System;

namespace SFML_Game_Engine.Engine.Resources
{
    public class SoundResource : Resource
    {
        public SoundBuffer Resource { get; private set; }

        public SoundResource(string path, string name)
        {
            Name = name;
            Resource = new SoundBuffer(path);
        }

        public override void Dispose()
        {
            Resource.Dispose();
        }

        public static implicit operator SoundBuffer(SoundResource res) { return res.Resource; }
    }
}
