using AbstractSockets.Abstract;
using System;

namespace AbstractSockets.Delegates
{
    public delegate void ServerOnException<T>(IAbstractServer<T> abstractServer, Exception exception);
}
