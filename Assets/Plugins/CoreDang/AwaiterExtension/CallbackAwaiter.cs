using DG.Tweening;
using System;
using System.Runtime.CompilerServices;

namespace CoreDang
{
    //public class CallbackAwaiter : INotifyCompletion
    //{
    //    Action continuation = delegate { };

    //    public bool IsCompleted { get; private set; }

    //    public CallbackAwaiter(Tweener tweener)
    //    {
    //        tweener.onComplete = () => { IsCompleted = true; continuation(); };
    //    }

    //    public void GetResult() { }

    //    public void OnCompleted(Action continuation)
    //    {
    //        this.continuation = continuation;
    //    }
    //}

    //public static class CallbackAwaiterExtension
    //{
    //    public static CallbackAwaiter GetAwaiter(this Tweener tween)
    //    {
    //        return new CallbackAwaiter(tween);
    //    }

    //    public static void ConfigAwaiter(this Tweener tween, bool value) { }
    //}
}