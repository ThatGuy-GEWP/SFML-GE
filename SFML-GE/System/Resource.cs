using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_GE.System
{
    /// <summary>
    /// Base class for all Resources.
    /// </summary>
    public abstract class Resource : IDisposable
    {
        /// <summary>
        /// The name of this <see cref="Resource"/>
        /// </summary>
        public string Name { get; protected set; } = string.Empty;

        /// <summary>
        /// The Description of this <see cref="Resource"/>, usually contains info on what it contains.
        /// </summary>
        public string? Description { get; protected set; }

        /// <summary>
        /// How many times this resource has been gotten from <see cref="Project.GetResource{T}(string)"/>
        /// </summary>
        public uint requests = 0;

        // Most resources will not be disposed, not with how im doing things
        // but just in case anyone else wants to dispose of stuff,
        // All resources have this method just so memory leaks dont pile up.
        /// <summary>
        /// Disposes of a <see cref="Resource"/>
        /// </summary>
        public abstract void Dispose();
    }
}
