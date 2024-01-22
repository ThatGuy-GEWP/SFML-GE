using SFML.Audio;
using SFML_Game_Engine;

namespace SFMLGE_Local_deps.Engine
{
    public class SoundInstance
    {
        public Component? owner = null;
        public Sound sound;

        public bool hadOwner = false;

        public SoundInstance(Sound sound) { this.sound = sound; }

        public SoundInstance(SoundResource sound)
        {
            this.sound = new Sound(sound);
        }

        public SoundInstance(Sound sound, Component owner)
        {
            this.sound = sound;
            this.owner = owner;
            hadOwner = true;
        }

        public SoundInstance(SoundResource sound, Component owner)
        {
            this.sound = new Sound(sound);
            this.owner = owner;
            hadOwner = true;
        }

        public void Dispose()
        {
            sound.Stop();
            sound.Dispose();
        }
    }


    /// <summary>
    /// Top level class that handles audio calls.
    /// SFML Threads audio, so a manager is required if you want to unload scenes and such.
    /// </summary>
    public class AudioManager
    {
        Stack<SoundInstance> activeSounds = new Stack<SoundInstance>(200);
        Scene ownerScene;

        public AudioManager(Scene owner)
        {
            ownerScene = owner;
        }

        public void Update()
        {
            Stack<SoundInstance> stillPlayingSounds = new Stack<SoundInstance>(activeSounds.Count);
            for (int i = 0; i < activeSounds.Count; i++)
            {
                SoundInstance cur = activeSounds.Pop();

                if (cur.sound == null) { continue; }
                if(cur.sound.Status == SoundStatus.Stopped)
                {
                    cur.Dispose();
                    continue;
                }
                stillPlayingSounds.Push(cur);
            }
            activeSounds = stillPlayingSounds;
        }

        public void PlaySound(SoundResource sound)
        {
            if(activeSounds.Count > 200) { return; }
            SoundInstance inst = new SoundInstance(sound);
            inst.sound.Play();
            activeSounds.Push(inst);
        }

        public void PlaySound(SoundResource sound, float volume)
        {
            if (activeSounds.Count > 200) { return; }
            SoundInstance inst = new SoundInstance(sound);
            inst.sound.Volume = volume;
            inst.sound.Play();
            activeSounds.Push(inst);
        }
    }
}
