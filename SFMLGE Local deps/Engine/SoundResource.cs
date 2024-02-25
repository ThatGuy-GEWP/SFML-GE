using SFML.Audio;

namespace SFML_Game_Engine
{
    public class SoundResource : Resource
    {
        public SoundBuffer Resource { get; private set; }

        public SoundResource(string path, string name)
        {
            this.name = name;
            Resource = new SoundBuffer(path);
        }

        public override void Dispose()
        {
            Resource.Dispose();
        }

        public static implicit operator SoundBuffer(SoundResource res) { return res.Resource; }
    }
}
