using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadDog.Audio.Libraries
{
    public interface ITracks
    {
        IEnumerable<Track> GetTracks();
    }
}
