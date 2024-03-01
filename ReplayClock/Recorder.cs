using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Walter;

namespace ReplayClock
{
    internal record Recorder
    {
        public Recorder()
        {
             CreatedUtc= IClock.Instance.UtcNow;
        }
        public DateTime CreatedUtc { get; }
        public int X { get; init ; }
        public int Y { get; init; }
    }
}
