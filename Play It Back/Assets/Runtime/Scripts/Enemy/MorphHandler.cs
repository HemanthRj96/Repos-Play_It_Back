using FFG.Message;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MorphHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject _morphPrefab = null;
    [SerializeField]
    private Transform _morphSpawn;
    [SerializeField]
    private float _morphPeriod = 0;

    private Vector3 _morphSpawnLocation;

    private bool _canMorph = false;
    private float _timer = 0;
    private Queue<GameObject> _pool = new Queue<GameObject>();


    private void Awake()
    {
        _morphSpawnLocation = _morphSpawn.position;
    }

    private void Start()
    {
        Message.AddMessageListener("LR:", onLevelReset);
        Message.AddMessageListener("TR:save", warmupPool);
        Message.AddMessageListener("TR:start-pb", startMorphing);
        Message.AddMessageListener("TR:stop-pb", stopMorphing);
    }

    private void Update()
    {
        if (_canMorph)
        {
            if (_timer > _morphPeriod)
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
        GameObject go = _pool.Dequeue();
        _pool.Enqueue(go);

        go.transform.position = _morphSpawnLocation;
        go.SetActive(true);
    }

    private void warmupPool(IMessage message)
    {
        if (message != null)
        {
            Vector2 timeStamp = (Vector2)message.Data;
            float duration = timeStamp.y - timeStamp.x;
            int count = Mathf.CeilToInt((duration + 1) / _morphPeriod);

            for (int i = 0; i < count; i++)
            {
                GameObject go = Instantiate(_morphPrefab);
                go.SetActive(false);
                _pool.Enqueue(go);
            }

            Message.InvokeMessage("TR:start-pb");
        }
    }

    private void startMorphing(IMessage message) => _canMorph = true;

    private void stopMorphing(IMessage message) => _canMorph = false;

    private void onLevelReset(IMessage obj)
    {
        _canMorph = false;

        _pool.ToList().ForEach(p =>
        {
            p.SetActive(false);
            Destroy(p);
        });
        _pool.Clear();
    }
}
