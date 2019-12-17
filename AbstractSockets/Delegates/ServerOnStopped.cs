using AbstractSockets.Abstract;
using AbstractSockets.Enums;

namespace AbstractSockets.Delegates
{
    public delegate void ServerOnStopped<T>(IAbstractServer<T> abstractServer, NetStoppedReason reason);
}
