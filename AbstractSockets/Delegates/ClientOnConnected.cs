using AbstractSockets.Abstract;

namespace AbstractSockets.Delegates
{
    public delegate void ClientOnConnected<T>(IAbstractClient<T> abstractClient);
}
