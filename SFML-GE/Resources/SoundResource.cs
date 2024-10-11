using SFML.Audio;
using SFML_GE.System;

namespace SFML_GE.Resources
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
