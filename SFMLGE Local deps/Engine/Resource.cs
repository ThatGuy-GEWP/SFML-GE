using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
{
    /// <summary>
    /// Base class for all Resources.
    /// </summary>
    public abstract class Resource : IDisposable
    {
        public string name { get; protected set; } = string.Empty;
        public string? Description { get; protected set; }

        public uint requests = 0;

        // Most resources will not be disposed, not with how im doing things
        // but just in case anyone else wants to dispose of stuff,
        // All resources have this method just so memory leaks dont pile up.
        public abstract void Dispose();
    }
}
