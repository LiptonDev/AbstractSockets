using AbstractSockets.Abstract;
using System;

namespace AbstractSockets.Delegates
{
    public delegate void ServerOnClientConnected<T>(IAbstractServer<T> abstractServer, Guid guid);
}
