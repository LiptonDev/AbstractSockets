using System;
using System.Collections.Generic;

namespace AbstractSockets
{
    static class GuidHelper
    {
        static object locker = new object();
        static List<Guid> guids { get; } = new List<Guid>();

        public static Guid GetGuid()
        {
            var g = Guid.NewGuid();

            if (guids.Contains(g))
                return GetGuid();
            else
            {
                guids.Add(g);
                return g;
            }
        }

        public static void RemoveGuid(Guid guid)
        {
            lock (locker)
            {
                if (guids.Contains(guid))
                    guids.Remove(guid);
            }
        }
    }
}
