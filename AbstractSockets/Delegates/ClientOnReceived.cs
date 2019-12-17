using AbstractSockets.Abstract;

namespace AbstractSockets.Delegates
{
    public delegate void ClientOnReceived<T>(IAbstractClient<T> abstractClient, T data);
}
