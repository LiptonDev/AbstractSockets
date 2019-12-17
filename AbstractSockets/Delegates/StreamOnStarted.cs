using AbstractSockets.Abstract;

namespace AbstractSockets.Delegates
{
    public delegate void StreamOnStarted<T>(IAbstractStream<T> stream);
}
