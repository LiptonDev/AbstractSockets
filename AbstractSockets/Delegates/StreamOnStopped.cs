using AbstractSockets.Abstract;
using AbstractSockets.Enums;

namespace AbstractSockets.Delegates
{
    public delegate void StreamOnStopped<T>(IAbstractStream<T> stream, NetStoppedReason reason);
}
