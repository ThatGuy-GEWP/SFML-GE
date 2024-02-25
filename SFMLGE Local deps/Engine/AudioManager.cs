using SFML.Audio;
using SFML_Game_Engine;
using System.Runtime.ConstrainedExecution;

namespace SFML_Game_Engine
{
    public class SoundInstance : IDisposable
    {
        public Sound sound;
        public string name;
        public bool Disposed = false;

        /// <summary>
        /// If true, this instance will be automatically disposed of.
        /// </summary>
        public bool allowCleanup = true;

        public SoundInstance(string name, Sound sound) { this.name = name; this.sound = sound; }

        public SoundInstance(string name, SoundResource sound)
        {
            this.name = name;
            this.sound = new Sound(sound);
        }

        /// <summary>
        /// Disposes of the soundInstance.
        /// </summary>
        public void Dispose()
        {
            sound.Stop();
            sound.Dispose();
            Disposed = true;
        }
    }


    /// <summary>
    /// Top level class that handles audio calls.
    /// SFML Threads audio, so a manager is required if you want to unload scenes and such.
    /// </summary>
    public class AudioManager
    {
        List<SoundInstance> activeSounds = new List<SoundInstance>(200);
        Scene ownerScene;

        public AudioManager(Scene owner)
        {
            ownerScene = owner;
        }

        public void Update()
        {
            List<SoundInstance> stillPlayingSounds = new List<SoundInstance>();
            for (int i = 0; i < activeSounds.Count; i++)
            {
                SoundInstance cur = activeSounds[i];

                if (cur.Disposed) { continue; }
                if (cur.sound == null) { continue; }
                if ((cur.sound.Status == SoundStatus.Stopped) && cur.allowCleanup)
                {
                    cur.Dispose();
                    continue;
                }
                stillPlayingSounds.Add(cur);
            }
            activeSounds.Clear();
            activeSounds = stillPlayingSounds;
        }

        /// <summary>
        /// Plays a given <see cref="SoundResource"/>
        /// </summary>
        /// <param name="sound"></param>
        public void PlaySound(SoundResource sound)
        {
            if (activeSounds.Count > 200) { return; }
            SoundInstance inst = new SoundInstance(sound.name, sound);
            inst.sound.Play();
            activeSounds.Add(inst);
        }

        /// <summary>
        /// Plays a given <paramref name="sound"/> at a given <paramref name="volume"/> from 0-100
        /// </summary>
        /// <param name="sound"></param>
        public void PlaySound(SoundResource sound, float volume)
        {
            if (activeSounds.Count > 200) { return; }
            SoundInstance inst = new SoundInstance(sound.name, sound);
            inst.sound.Volume = volume;
            inst.sound.Play();
            activeSounds.Add(inst);
        }

        /// <summary>
        /// Creates a <see cref="SoundInstance"/> from a given <see cref="SoundResource"/>
        /// </summary>
        /// <param name="sound"></param>
        /// <returns><see cref="SoundInstance"/>, or null if active sounds is greater then 200</returns>
        public SoundInstance CreateSound(SoundResource sound)
        {
            if (activeSounds.Count > 200) { return null; }
            SoundInstance inst = new SoundInstance(sound.name, sound);
            inst.allowCleanup = false;
            activeSounds.Add(inst);
            return inst;
        }

        /// <summary>
        /// Creates a <see cref="SoundInstance"/> from a given <see cref="SoundResource"/>
        /// </summary>
        /// <param name="sound"></param>
        /// <returns><see cref="SoundInstance"/>, or null if active sounds is greater then 200</returns>
        public SoundInstance CreateSound(SoundResource sound, float volume)
        {
            if (activeSounds.Count > 200) { return null; }
            SoundInstance inst = new SoundInstance(sound.name, sound);
            inst.allowCleanup = false;
            inst.sound.Volume = volume;
            activeSounds.Add(inst);
            return inst;
        }
    }
}
