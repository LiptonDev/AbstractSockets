using AbstractSockets.Abstract;
using System;

namespace AbstractSockets.Delegates
{
    public delegate void AbstractStreamOnStarted<T>(IAbstractStream<T> stream);
}
