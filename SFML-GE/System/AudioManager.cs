using SFML.Audio;
using SFML_GE.Resources;

namespace SFML_GE.System
{
    /// <summary>
    /// An instance of a <see cref="Sound"/>
    /// </summary>
    public class ManagedSound : IDisposable
    {
        /// <summary>
        /// The <see cref="Sound"/> within this instance
        /// </summary>
        public Sound sound;
        /// <summary>
        /// The name of this sound
        /// </summary>
        public string name;


        /// <summary>
        /// false while the sound has yet to be disposed
        /// </summary>
        public bool Disposed { get; private set; } = false;

        /// <summary>
        /// If true, this instance will be automatically disposed of.
        /// </summary>
        public bool allowCleanup = true;

        /// <summary>
        /// Creates a ManagedSound from a <paramref name="name"/> and a <see cref="Sound"/>
        /// </summary>
        public ManagedSound(string name, Sound sound) { this.name = name; this.sound = sound; }

        /// <summary>
        /// Creates a ManagedSound from a <paramref name="name"/> and a <see cref="SoundResource"/>
        /// </summary>
        public ManagedSound(string name, SoundResource sound)
        {
            this.name = name;
            this.sound = new Sound(sound);
        }

        /// <summary> Plays the current <see cref="sound"/> </summary>
        public void Play()
        {
            sound.Play();
        }

        /// <summary> Stops the current <see cref="sound"/> </summary>
        public void Stop()
        {
            sound.Stop();
        }

        /// <summary> Sets the volume of the current <see cref="sound"/> </summary>
        public void SetVolume(float volume)
        {
            sound.Volume = volume;
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
    /// A Top level class that handles audio calls.
    /// SFML Automatically spawns threads to play audio, so this just keeps track and disposes of unused or finished sounds.
    /// </summary>
    public class AudioManager
    {
        List<ManagedSound> activeSounds = new List<ManagedSound>(200);
        Scene ownerScene;

        internal AudioManager(Scene owner)
        {
            ownerScene = owner;
        }

        /// <summary>
        /// Updates the internal state of this <see cref="AudioManager"/>
        /// </summary>
        internal void Update()
        {
            List<ManagedSound> stillPlayingSounds = new List<ManagedSound>();
            for (int i = 0; i < activeSounds.Count; i++)
            {
                ManagedSound cur = activeSounds[i];

                if (cur.Disposed) { continue; }
                if (cur.sound == null) { continue; }
                if (cur.sound.Status == SoundStatus.Stopped && cur.allowCleanup)
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
        /// Called when the <see cref="Scene"/> this AudioManager is attached to is unloaded
        /// </summary>
        internal void OnUnload()
        {
            for (int i = 0; i < activeSounds.Count; i++)
            {
                ManagedSound cur = activeSounds[i];
                cur.Stop();
            }
        }

        /// <summary>
        /// Plays a given <see cref="SoundResource"/>
        /// </summary>
        /// <param name="sound"></param>
        public void PlaySound(SoundResource sound)
        {
            if (activeSounds.Count > 200) { return; }
            ManagedSound inst = new ManagedSound(sound.Name, sound);
            inst.sound.Play();
            activeSounds.Add(inst);
        }

        /// <summary>
        /// Plays a given <paramref name="sound"/> at a given <paramref name="volume"/> from 0-100
        /// </summary>
        public void PlaySound(SoundResource sound, float volume)
        {
            if (activeSounds.Count > 200) { return; }
            ManagedSound inst = new ManagedSound(sound.Name, sound);
            inst.sound.Volume = volume;
            inst.sound.Play();
            activeSounds.Add(inst);
        }

        /// <summary>
        /// Creates a <see cref="ManagedSound"/> from a given <see cref="SoundResource"/>
        /// </summary>
        /// <param name="sound"></param>
        /// <returns><see cref="ManagedSound"/>, or null if active sounds is greater then 200</returns>
        public ManagedSound? CreateSound(SoundResource sound)
        {
            if (activeSounds.Count > 200) { return null; }
            ManagedSound inst = new ManagedSound(sound.Name, sound);
            inst.allowCleanup = false;
            activeSounds.Add(inst);
            return inst;
        }

        /// <summary>
        /// Creates a <see cref="ManagedSound"/> from a given <see cref="SoundResource"/>
        /// </summary>
        /// <returns><see cref="ManagedSound"/>, or null if active sounds is greater then 200</returns>
        public ManagedSound? CreateSound(SoundResource sound, float volume)
        {
            if (activeSounds.Count > 200) { return null; }
            ManagedSound inst = new ManagedSound(sound.Name, sound);
            inst.allowCleanup = false;
            inst.sound.Volume = volume;
            activeSounds.Add(inst);
            return inst;
        }
    }
}
