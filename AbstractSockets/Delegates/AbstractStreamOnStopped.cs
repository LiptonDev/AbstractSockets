using AbstractSockets.Abstract;
using AbstractSockets.Enums;

namespace AbstractSockets.Delegates
{
    public delegate void AbstractStreamOnStopped<T>(IAbstractStream<T> stream, NetStoppedReason reason);
}
