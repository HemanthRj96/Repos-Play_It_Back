using System;
using UnityEngine;


public enum ETimerMode
{
    Running,
    Repeating,
    Single
}

public static class Timer
{
    private class MonoBehaviourHook : MonoBehaviour
    {
        public event Action OnUpdate = delegate { };

        private void Update() => OnUpdate();

        public void MakePersistent() => DontDestroyOnLoad(this);
    }

    // Private fields

    private static MonoBehaviourHook s_hook = null;

    // Private methods

    private static TimerHandle createTimer(Action callback, float duration, ETimerMode mode)
    {
        // Check if hook exists
        if (s_hook == null)
        {
            var gameobject = new GameObject("-Timer-MonobehaviourHook-", typeof(MonoBehaviourHook));
            s_hook = gameobject.GetComponent<MonoBehaviourHook>();
            s_hook.MakePersistent();
        }

        var handle = new TimerHandle(callback, duration, mode);
        s_hook.OnUpdate += handle.Tick;
        return handle;
    }

    public static TimerHandle CreateCountdownTimer(Action callback, float duration)
        => createTimer(callback, duration, ETimerMode.Single);


    public static TimerHandle CreateRunningTimer()
        => createTimer(null, 0, ETimerMode.Running);


    public static TimerHandle CreateRepeatingTimer(Action callback, float duration)
        => createTimer(callback, duration, ETimerMode.Repeating);
}
