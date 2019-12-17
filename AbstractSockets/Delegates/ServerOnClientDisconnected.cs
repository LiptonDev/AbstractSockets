using AbstractSockets.Abstract;
using AbstractSockets.Enums;
using System;

namespace AbstractSockets.Delegates
{
    public delegate void ServerOnClientDisconnected<T>(IAbstractServer<T> abstractServer, Guid guid, NetStoppedReason reason);
}
