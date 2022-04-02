using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FFG.Message;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    [Min(0)]
    private float _moveSpeed = 0;
    [SerializeField]
    [Min(0)]
    private float _jumpForce = 0;
    [SerializeField]
    [Range(0, 1)]
    private float _airControl = 0;
    [SerializeField]
    private LayerMask _groundLayerMask;


    private float _axisCached = 0;

    private Vector3 _spawnLocation;

    private Rigidbody2D _rb = null;
    private SpriteRenderer _sp = null;

    private List<Collider2D> _cachedGroundColliders = new List<Collider2D>();

    private bool _bHasMoved = false;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sp = GetComponent<SpriteRenderer>();

        _spawnLocation = GameObject.FindGameObjectWithTag("Spawn").transform.position;
        transform.position = _spawnLocation;

        TransformRecorder.Instance.AddTransformToRecorder(transform, "Player");
        Message.CreateMessage(onLevelReset, "LR:");
    }

    private void Start()
    {
        //Message.AddMessageListener("TR:save", onRecordsSaved);
    }


    private void OnDisable() => _bHasMoved = false;

    private void Update()
    {
        _axisCached = horizontal();

        if(_axisCached != 0 && _bHasMoved == false)
        {
            _bHasMoved = true;
            Message.InvokeMessage("TR:start-rec");
        }

        //Update jump
        if (jump() && onGround())
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        //Update movement
        if (_axisCached != 0 && !onGround())
            _axisCached *= _airControl;

        transform.position += Vector3.right * _moveSpeed * _axisCached * Time.deltaTime;
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

    private void onLevelReset(IMessage obj)
    {
        // Handle player death
        gameObject.SetActive(false);
        transform.position = _spawnLocation;
        gameObject.SetActive(true);
    }

    private void onRecordsSaved(IMessage obj)
    {
        
    }
}
