using UnityEngine;


public class MovingObstacles : MonoBehaviour
{
    [SerializeField]
    private float _maxDisplacement;
    [SerializeField]
    private Vector2 _direction;
    [SerializeField, Range(0, 10)]
    private float _duration;

    private Vector3 _initialPosition;
    private float _timer = 0;
    private bool _bShouldUpdate = false;
    private Transform _t;


    private void Awake() => _t = transform;

    private void Update()
    {
        if (_bShouldUpdate)
        {
            _t.position = Vector3.Lerp
                (
                    _initialPosition,
                    _initialPosition + (Vector3)_direction.normalized * _maxDisplacement,
                    Mathf.Clamp01((Time.time - _timer) / _duration)
                );
        }
    }

    private void onKeyAcquire()
    {
        _initialPosition = _t.position;
        _timer = Time.time;
        _bShouldUpdate = true;
    }
}
