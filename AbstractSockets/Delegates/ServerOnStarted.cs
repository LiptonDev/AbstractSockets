using AbstractSockets.Abstract;

namespace AbstractSockets.Delegates
{
    public delegate void ServerOnStarted<T>(IAbstractServer<T> abstractServer);
}
