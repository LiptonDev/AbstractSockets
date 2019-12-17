using AbstractSockets.Abstract;
using AbstractSockets.Enums;

namespace AbstractSockets.Delegates
{
    public delegate void ClientOnDisconnected<T>(IAbstractClient<T> abstractClient, NetStoppedReason reason);
}
