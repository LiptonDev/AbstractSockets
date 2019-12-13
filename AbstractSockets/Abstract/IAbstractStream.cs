using AbstractSockets.Delegates;
using System;
using System.Net;

namespace AbstractSockets.Abstract
{
    public interface IAbstractStream<T>
    {
        event AbstractStreamOnStarted<T> OnStarted;
        event AbstractStreamOnStopped<T> OnStopped;
        event AbstractStreamOnReceived<T> OnReceived;

        Guid Guid { get; }

        bool IsActive { get; }

        EndPoint EndPoint { get; }

        void Start();
        void Stop();
    }
}
