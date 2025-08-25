using SFML.Audio;
using SFML_GE.Resources;

namespace SFML_GE.System
{
    // SOMETHING IS MAKING EXTRA AUDIO FIX IT 


    /// <summary>
    /// An instance of a SFML <see cref="Sound"/>, managed by the <see cref="AudioManager"/>.
    /// If <see cref="allowCleanup"/> is <c>true</c>, the <see cref="AudioManager"/> will automatically
    /// dispose the <see cref="SFML.Audio.Sound"/> instance when this sound is no longer playing.
    /// All <see cref="ManagedSound"/>'s will be stopped when a scene is unloaded.
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
        /// If true, this instance will be automatically disposed of when not playing.
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
        List<ManagedSound> activeSounds = new List<ManagedSound>(1024);
        Scene ownerScene;

        /// <summary>
        /// Returns a refrence to the list of active sounds that are managed.
        /// </summary>
        public List<ManagedSound> ActiveSounds { get { return activeSounds; } }

        internal AudioManager(Scene owner)
        {
            ownerScene = owner;
        }

        // seems dumb, fix eventually!
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
        /// Called when the <see cref="Scene"/> this AudioManager is attached to gets unloaded
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
        /// Tries to get the <see cref="SoundResource"/> that's nammed <paramref name="soundResourceName"/>, then plays it.
        /// </summary>
        /// <param name="soundResourceName">The name of the <see cref="SoundResource"/> to play.</param>
        /// <param name="volume">the volume of this sound, from 0 - 100</param>
        /// <param name="pitch">the pitch of this sound, where 1.0 is the default pitch.</param>
        /// <exception cref="NullReferenceException">if <see cref="SoundResource"/> could not be found</exception>
        public void PlaySound(string soundResourceName, float volume = 100f, float pitch = 1.0f)
        {
            SoundResource? res = ownerScene.Project.GetResource<SoundResource>(soundResourceName);
            if (res == null)
            {
                throw new NullReferenceException($"Sound Resource \"{soundResourceName}\" could not be found!");
            }
            PlaySound(res, volume, pitch);
        }

        /// <summary>
        /// Tries to get the <see cref="SoundResource"/> that's nammed <paramref name="soundResourceName"/>,
        /// Then returns a newly created <see cref="ManagedSound"/> instance.
        /// </summary>
        /// <returns><see cref="ManagedSound"/>, or null if active sounds is greater then 200</returns>
        /// <param name="soundResourceName">The name of the <see cref="SoundResource"/> to play.</param>
        /// <param name="volume">the volume of this sound, from 0 - 100</param>
        /// <param name="pitch">the pitch of this sound, where 1.0 is the default pitch.</param>
        /// <exception cref="NullReferenceException">if <see cref="SoundResource"/> could not be found</exception>
        public ManagedSound? CreateSound(string soundResourceName, float volume = 100f, float pitch = 1.0f)
        {
            SoundResource? res = ownerScene.Project.GetResource<SoundResource>(soundResourceName);
            if (res == null)
            {
                throw new NullReferenceException($"Sound Resource \"{soundResourceName}\" could not be found!");
            }
            return CreateSound(res, volume, pitch);
        }

        /// <summary>
        /// Plays a given <paramref name="sound"/> at a given <paramref name="volume"/> from 0-100
        /// </summary>
        /// <param name="sound">The <see cref="SoundResource"/> to play</param>
        /// <param name="volume">the volume of this sound, from 0 - 100</param>
        /// <param name="pitch">the pitch of this sound, where 1.0 is the default pitch.</param>
        public void PlaySound(SoundResource sound, float volume = 100f, float pitch = 1.0f)
        {
            if (activeSounds.Count > 200) { return; }
            ManagedSound inst = new ManagedSound(sound.Name, sound);
            inst.sound.Volume = volume;
            inst.sound.Pitch = pitch;
            inst.sound.Play();
            activeSounds.Add(inst);
        }

        /// <summary>
        /// Creates a <see cref="ManagedSound"/> from a given <see cref="SoundResource"/>
        /// </summary>
        /// <param name="sound">The <see cref="SoundResource"/> to instance.</param>
        /// <param name="volume">the volume of this sound, from 0 - 100</param>
        /// <param name="pitch">the pitch of this sound, where 1.0 is the default pitch.</param>
        /// <returns><see cref="ManagedSound"/>, or null if active sounds is greater then 200</returns>
        public ManagedSound? CreateSound(SoundResource sound, float volume = 100f, float pitch = 1.0f)
        {
            if (activeSounds.Count > 200) { return null; }
            ManagedSound inst = new ManagedSound(sound.Name, sound);
            inst.allowCleanup = false;
            inst.sound.Volume = volume;
            inst.sound.Pitch = pitch;
            activeSounds.Add(inst);
            return inst;
        }
    }
}
