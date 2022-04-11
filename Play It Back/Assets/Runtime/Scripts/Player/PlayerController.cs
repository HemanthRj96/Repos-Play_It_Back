using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    // Serialized fields

    [SerializeField, Min(0)]
    private float _moveSpeed = 0;
    [SerializeField, Min(0)]
    private float _jumpForce = 0;
    [SerializeField, Range(0, 1)]
    private float _airControl = 0;
    [SerializeField]
    private LayerMask _groundLayerMask;
    [SerializeField, Range(0, 0.2f)]
    private float _accelerationTime;
    [SerializeField, Range(0, 0.2f)]
    private float _deccelerationTime;
    [SerializeField]
    private AnimationCurve _accelerationCurve;
    [SerializeField]
    private AnimationCurve _deccelerationCurve;

    // Private fields

    private float _axisCached = 0;
    private float _modifiedAxis = 0;
    private float _accStartTime = -1;
    private float _dccStartTime = -1;

    private float _ma = 0;
    private float _absAxis = 0;

    private Vector3 _spawnLocation;

    private Rigidbody2D _rb = null;
    private SpriteRenderer _sp = null;
    private Transform _t = null;

    private List<Collider2D> _cachedGroundColliders = new List<Collider2D>();


    private bool _bCanMove = true;
    private bool _bHasMoved = false;


    // Properties

    public Rigidbody2D Rb => _rb;

    // Private methods

    private void Awake()
    {
        _spawnLocation = GameObject.FindGameObjectWithTag("Spawn").transform.position;
        _rb = GetComponent<Rigidbody2D>();
        _sp = GetComponent<SpriteRenderer>();
        _t = transform;
        _t.position = _spawnLocation;
    }

    private void Update()
    {
        if (_bCanMove == false)
        {
            _rb.bodyType = RigidbodyType2D.Static;
            return;
        }

        // Update input
        inputUpdate();
    }

    private void FixedUpdate()
    {
        if (_bCanMove == false)
        {
            _rb.bodyType = RigidbodyType2D.Kinematic;
            return;
        }

        // Update movement
        locomotionUpdate();
    }

    private float horizontal() => Input.GetAxisRaw("Horizontal");

    private bool jump() => Input.GetButtonDown("Jump");

    private bool onGround()
    {
        var center = _sp.bounds.center;
        var extents = _sp.bounds.extents;
        Vector2 bOrigin = new Vector2(center.x, center.y - extents.y);
        Vector2 bSize = new Vector2(extents.x * 2 - 0.2f, 0.01f);
        var hits = Physics2D.BoxCastAll(bOrigin, bSize, 0, Vector2.down, 0.01f, _groundLayerMask);

        if (hits.Length != 0)
        {
            _cachedGroundColliders.Clear();
            hits.ToList().ForEach(c => _cachedGroundColliders.Add(c.collider));
        }
        return hits.Length > 0;
    }

    private void inputUpdate()
    {
        _axisCached = horizontal();

        if (_axisCached != 0 && _bHasMoved == false)
            _bHasMoved = true;

        //Update jump
        if (jump() && onGround())
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }

    private void locomotionUpdate()
    {
        if (_axisCached != 0 && !onGround())
            _axisCached *= _airControl;

        if (_axisCached != 0)
        {

            if (_accStartTime == -1)
            {
                _dccStartTime = -1;
                _accStartTime = Time.time;

                _ma = Mathf.Abs(_modifiedAxis);
                _absAxis = Mathf.Abs(_axisCached);
            }

            _modifiedAxis = _axisCached * _accelerationCurve.Evaluate(Mathf.Lerp
                (
                    _ma,
                    _absAxis,
                    Mathf.Clamp01((Time.time - _accStartTime) / _accelerationTime)
                ));
        }
        else if (_axisCached == 0)
        {
            if (_dccStartTime == -1)
            {
                _accStartTime = -1;
                _dccStartTime = Time.time;

                _ma = _modifiedAxis;
            }

            _modifiedAxis = _ma * _deccelerationCurve.Evaluate(Mathf.Lerp
                (
                    0,
                    1,
                    Mathf.Clamp01((Time.time - _dccStartTime) / _deccelerationTime))
                );
        }

        transform.position += Vector3.right * _moveSpeed * _modifiedAxis * Time.deltaTime;
    }

    // Public methods

    public void DisableMoving() => _bCanMove = false;

    public void EnableMoving() => _bCanMove = true;

    public bool HasMoved() => _bHasMoved;
}
