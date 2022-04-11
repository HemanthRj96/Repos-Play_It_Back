using System.Collections.Generic;
using UnityEngine;


public class CloneHandler : MonoBehaviour
{
    // Serialized fields

    [SerializeField]
    private GameObject _clonePrefab = null;
    [SerializeField]
    private float _cloningPeriod = 0;

    // Private fields

    private Vector3 _cloneSpawn;

    private bool _canClone = false;
    private float _timer = 0;
    private Queue<CloneController> _pool = new Queue<CloneController>();

    // Private methods

    private void Awake() => _cloneSpawn = GameObject.FindGameObjectWithTag("Key").transform.position;

    private void Start()
    {
        GlobalEventSystem.Instance.AddListener("Kill-Player", stopCloning);
        GlobalEventSystem.Instance.AddListener("TR-Save", warmupPool);
    }

    private void Update()
    {
        if (_canClone)
        {
            if (_timer > _cloningPeriod)
            {
                _timer = 0;
                spawnFromPool();
            }
            else
                _timer += Time.deltaTime;
        }
    }

    private void spawnFromPool()
    {
        var clone = _pool.Dequeue();
        _pool.Enqueue(clone);

        clone.transform.position = _cloneSpawn;
        clone.gameObject.SetActive(true);
    }

    private void warmupPool(object data, GameObject source)
    {
        if (source.tag != "Player")
            return;

        if (data != null)
        {
            AnalogTransform aT = (AnalogTransform)data;

            float duration = aT.Duration;
            int count = Mathf.CeilToInt(duration * 1.15f / _cloningPeriod);

            for (int i = 0; i < count; i++)
            {
                var clone = Instantiate(_clonePrefab).GetComponent<CloneController>();
                clone.SetAnalogTransform(aT);
                clone.gameObject.SetActive(false);
                _pool.Enqueue(clone);
            }

            // Perhaps some kind of delay or transition
            startMorphing();
        }
    }

    private void startMorphing() => _canClone = true;

    private void stopCloning() => _canClone = false;
}
