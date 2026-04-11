using System;
using System.Collections.Generic;
using System.Linq;

public interface IObserver
{
    void OnNotify(IObservable data, params string[] scopes);
}

public interface IObservable { }

public class ObserverProvider<T> where T : IObservable
{
    private readonly Dictionary<string, List<IObserver>> observerMaps = new Dictionary<string, List<IObserver>>();
    private readonly Dictionary<string, List<Action<T>>> handlerMaps = new Dictionary<string, List<Action<T>>>();

    public void Subscribe(IObserver observer, params string[] scopes)
    {
        Subscribe(observer, false, scopes);
    }

    public void Subcribe(string scope, Action<T> handler)
    {
        var handlers = HandlersByScope(scope);
        if(!handlers.Contains(handler))
        {
            handlers.Add(handler);
        }
    }

    public void Subscribe(IObserver observer, bool first = true, params string[] scopes)
    {
        if(scopes.Length == 0) scopes = new string[] { "" }; //default
        foreach(string scope in scopes)
        {
            var observers = ObserversByScope(scope);
            if(!observers.Contains(observer))
            {
                if(first) observers.Unshift(observer);
                else observers.Add(observer);
            }
        }
    }

    private List<IObserver> ObserversByScope(string scope) => observerMaps.GetOrCreate(scope, s => new List<IObserver>());
    private List<Action<T>> HandlersByScope(string scope) => handlerMaps.GetOrCreate(scope, s => new List<Action<T>>());

    public void Unsubscribe(IObserver observer)
    {
        foreach(List<IObserver> list in observerMaps.Values) list.Remove(observer);
    }

    public void Unsubscribe(string scope, Action<T> handler)
    {
        HandlersByScope(scope).Remove(handler);
    }

    public void Notify(T data, params string[] scopes)
    {
        List<string> _scopes = scopes.ToList();
        if(scopes.Length == 0)
            _scopes.Add("");

        var calledObservers = new List<IObserver>();
        foreach(string scope in _scopes)
        {
            var observers = ObserversByScope(scope).ToList();
            foreach(IObserver observer in observers)
            {
                if(!calledObservers.Contains(observer))
                {
                    observer.OnNotify(data, scopes);
                    calledObservers.Add(observer);
                }
            }

            var handlers = HandlersByScope(scope).ToList();
            foreach(var handler in handlers) handler.Invoke(data);
        }
    }
}
