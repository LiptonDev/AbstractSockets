using AbstractSockets.Abstract;

namespace AbstractSockets.Delegates
{
    public delegate void StreamOnReceived<T>(IAbstractStream<T> stream, T data);
}
