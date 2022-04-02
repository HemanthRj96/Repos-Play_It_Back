using System;
using UnityEngine;


namespace FFG.Timer
{
    /// <summary>
    /// Use this class to create function timers
    /// </summary>
    public static class Timer
    {
        /// <summary>
        /// Hook component which is used to hook the TimerHandles to update method
        /// </summary>
        private class MonoBehaviourHook : MonoBehaviour
        {
            public event Action OnUpdate = delegate { };

            private void Update() => OnUpdate();

            public void MakePersistent() => DontDestroyOnLoad(this);
        }

        /*.............................................Private Fields.......................................................*/

        private static MonoBehaviourHook s_hook = null;

        /*.............................................Private Methods......................................................*/

        /// <summary>
        /// Timer construction helper
        /// </summary>
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

        /*.............................................Public Methods.......................................................*/

        /// <summary>
        /// Creates a countdown timer which invoked the callback after duration (seconds)
        /// </summary>
        /// <param name="callback">Method to be called after timer completes</param>
        /// <param name="duration">Total duration of timer</param>
        /// <returns>Returns the timer handle which can be used to pause, resume, and know the elapsed time</returns>
        public static TimerHandle CreateCountdownTimer(Action callback, float duration)
            => createTimer(callback, duration, ETimerMode.Single);


        /// <summary>
        /// Creates a timer that'll run forever. Used generally to run a background timer without any duration or callbacks
        /// </summary>
        /// <returns>Returns the timer handle which can be used to pause, resume, and know the elapsed time</returns>
        public static TimerHandle CreateRunningTimer() 
            => createTimer(null, 0, ETimerMode.Running);


        /// <summary>
        /// Creates a repeating timer which invokes callback every duration (seconds)
        /// </summary>
        /// <param name="callback">Method to be called after timer completes</param>
        /// <param name="duration">Duration between each timer repeatition</param>
        /// <returns>Returns the timer handle which can be used to pause, resume, and know the elapsed time</returns>
        public static TimerHandle CreateRepeatingTimer(Action callback, float duration)
            => createTimer(callback, duration, ETimerMode.Repeating);
    }
}