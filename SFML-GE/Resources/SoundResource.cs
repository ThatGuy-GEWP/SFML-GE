using SFML.Audio;
using SFML_GE.System;

namespace SFML_GE.Resources
{
    /// <summary>
    /// A Resource that represents a <see cref="SFML.Audio.SoundBuffer"/> in memory.
    /// </summary>
    public class SoundResource : Resource
    {
        /// <summary>
        /// The <see cref="SFML.Audio.SoundBuffer"/> this <see cref="Resource"/> represents.
        /// </summary>
        public SoundBuffer Resource { get; private set; }

        /// <summary>
        /// Creates a new <see cref="SoundResource"/> from a file path.<para/>
        /// Supported audio formats are:
        /// <para>
        /// ogg, wav, flac, aiff, au, raw, paf, svx, nist, voc, ircam, w64, mat4, mat5 pvf, htk, sds, avr, sd2, caf, wve, mpc2k, rf64
        /// </para>
        /// </summary>
        /// <param name="path">the relative or absolute path to the file</param>
        /// <param name="name">The (SHOULD BE UNIQUE) name of this resource</param>
        public SoundResource(string path, string name)
        {
            Name = name;
            Resource = new SoundBuffer(path);
        }

        /// <summary>
        /// Disposes of the <see cref="SoundBuffer"/> object this <see cref="SoundResource"/> represents.
        /// Will break anything using it or expecting it.
        /// </summary>
        public override void Dispose()
        {
            Resource.Dispose();
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static implicit operator SoundBuffer(SoundResource res) { return res.Resource; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
