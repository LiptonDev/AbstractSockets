using AbstractSockets.Abstract;

namespace AbstractSockets.Delegates
{
    public delegate void AbstractStreamOnReceived<T>(IAbstractStream<T> stream, T data);
}
