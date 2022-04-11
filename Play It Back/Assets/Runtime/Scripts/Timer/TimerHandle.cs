using System;
using UnityEngine;


public class TimerHandle
{
    /*.............................................Constructor..........................................................*/

    public TimerHandle(Action callback, float duration, ETimerMode mode)
    {
        _callback = delegate { };
        _callback += callback;
        _duration = Mathf.Max(0.002f, duration);
        _mode = mode;
        _elapsedTime = -0.02f;
        _previousTime = Time.time;
        _canTick = true;
    }

    /*.............................................Private Fields.......................................................*/

    private event Action _callback;
    private ETimerMode _mode;

    private float _duration;
    private float _elapsedTime;
    private float _previousTime;

    private bool _canTick;

    // Public porperties

    public Action Tick { get { return tick; } }

    // Private methods

    private void tick()
    {
        if (!_canTick)
            return;

        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _duration)
        {
            switch (_mode)
            {
                case ETimerMode.Repeating:
                    {
                        float err = (Time.time - _previousTime) - _duration;
                        _elapsedTime = err;
                        _callback();
                        _previousTime = Time.time - err;
                        return;
                    }
                case ETimerMode.Single:
                    _canTick = false;
                    _callback();
                    return;
            }
        }
    }

    // Public methods

    public float GetElapsedTime() => _elapsedTime;


    public float GetProgress() => Mathf.Clamp01(_elapsedTime / _duration);

    public float AddTime(float amount) => _duration += Mathf.Abs(amount);

    public float DeductTime(float amount) => _duration += Mathf.Abs(amount);

    public void PauseTimer() => _canTick = false;

    public void ResumeTimer() => _canTick = true;
}
