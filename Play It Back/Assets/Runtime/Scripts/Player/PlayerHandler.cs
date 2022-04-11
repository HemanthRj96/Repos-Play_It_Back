using System;
using UnityEngine;


public class PlayerHandler : MonoBehaviour
{
    private Transform _t = null;
    private Vector4 _worldBounds;
    private bool _bHasInvoked = false;
    private bool _bPlayerDead = false; 

    private PlayerController _controller = null;
    private GlobalEventSystem _eventSystem = null;
    private ObjectRuntimeTransformRecorder _recorder = null;

    private bool _bHasMoved => _controller.HasMoved();

    // Private methods

    private void Awake()
    {
        _t = transform;
        _controller = GetComponent<PlayerController>();
        _worldBounds = Camera.main.GetCameraBoundsInWorldSpace();
        _eventSystem = GameObject.FindGameObjectWithTag("Event System").GetComponent<GlobalEventSystem>();
        _recorder = GetComponent<ObjectRuntimeTransformRecorder>();
    }

    private void Start()
    {
        GlobalEventSystem.Instance.AddListener("Kill-Player", onKillPlayer);
        GlobalEventSystem.Instance.AddListener("Key-Pickup", onKeyPickup);
    }

    private void onKillPlayer()
    {
        var rb = _controller.Rb;
        rb.bodyType = RigidbodyType2D.Static;
        _controller.DisableMoving();
        Timer.CreateCountdownTimer(() => { _eventSystem.InvokeEvent("Level-Reset"); }, 0.5f);
    }

    private void Update()
    {
        if (_bPlayerDead)
            return;

        if (validSpace() == false)
        {
            _bPlayerDead = true;
            GlobalEventSystem.Instance.InvokeEvent("Kill-Player");
        }

        if (_bHasInvoked == false && _bHasMoved)
        {
            _bHasInvoked = true;
            GlobalEventSystem.Instance.InvokeEvent("Player-BeginMove");
            _recorder.StartRecording();
        }
    }

    private bool validSpace()
    {
        var currentPos = _t.position;

        if (currentPos.y < _worldBounds.y || currentPos.y > _worldBounds.w)
            return false;
        else if (currentPos.x < _worldBounds.x || currentPos.x > _worldBounds.z)
            return false;
        return true;
    }

    private void onKeyPickup() => _recorder.StopRecord();
}
