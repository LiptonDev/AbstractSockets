using AbstractSockets.Abstract;
using System;

namespace AbstractSockets.Delegates
{
    public delegate void ServerOnDataReceived<T>(IAbstractServer<T> abstractServer, IAbstractStream<T> stream, T data);
}
