using System;

namespace CoreDang
{
    public abstract class BaseUserData : IObservable
    {
        /// <summary>
        /// Marked data changed and should save to disk
        /// </summary>
        [NonSerialized] public bool dirty = false;

        /// <summary>
        /// Marked data can write on disk
        /// </summary>
        [NonSerialized] public bool saveLocally = true;

        /// <summary>
        /// Marked data can not deleted on disk ( by code )
        /// </summary>
        [NonSerialized] public bool preventDelete = false;

        /// <summary>
        /// Observer provider
        /// </summary>
        [NonSerialized] private ObserverProvider<BaseUserData> provider = new ObserverProvider<BaseUserData>();

        public event Action OnDelete = delegate { };

        /// <summary>
        /// Invoke when new UserData was created
        /// </summary>
        protected internal virtual void OnInit()
        {
            dirty = true;
        }

        /// <summary>
        /// Invoke when UserData was loaded from disk
        /// </summary>
        protected internal virtual void OnLoad() { }

        /// <summary>
        /// Invoke when Application Pause
        /// </summary>
        protected internal virtual void OnPause() { }

        /// <summary>
        /// Incoke when Application Quit
        /// </summary>
        protected internal virtual void OnQuit() { }

        public void Save()
        {
            dirty = false;
            LocalData.Save(this);
        }

        public void Subscribe(string scope, Action<BaseUserData> handler, bool notify = true)
        {
            provider.Subcribe(scope, handler);
            if(notify) handler.Invoke(this);
        }

        public void Unsubscribe(string scope, Action<BaseUserData> handler)
        {
            provider.Unsubscribe(scope, handler);
        }

        internal virtual void OnWillDeleted()
        {
            OnDelete();
        }

        public void Subscribe(IObserver observer, params string[] scopes)
        {
            provider.Subscribe(observer, scopes);
            observer.OnNotify(this, scopes);
        }

        public void Subscribe(IObserver observer, bool first, params string[] scopes)
        {
            provider.Subscribe(observer, first, scopes);
            observer.OnNotify(this, scopes);
        }

        public void Unsubscribe(IObserver observer) => provider.Unsubscribe(observer);

        public void Notify(params string[] scopes) => provider.Notify(this, scopes);

        public void SaveAndNotify(params string[] scopes)
        {
            Save();
            provider.Notify(this, scopes);
        }

        public void DirtyAndNotify(params string[] scopes)
        {
            dirty = true;
            Notify(scopes);
        }
    }
}