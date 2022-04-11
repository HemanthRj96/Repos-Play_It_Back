using System.Collections;
using System.IO;
using UnityEngine;


public class ObjectRuntimeTransformRecorder : MonoBehaviour
{
    // Serialized fields

    [SerializeField]
    private float _recordRate = 60;

    // Private fields

    private Transform _t;
    private float _startTime = 0;
    private bool _bIsRecording = false;

    private AnalogTransform _aT = new AnalogTransform();

    // Properties

    private string _filePath => $"{Application.persistentDataPath}/Temp";

    // Private methods

    private void Awake()
    {
        _t = transform;
        if (Directory.Exists(_filePath) == false)
            Directory.CreateDirectory(_filePath);
    }

    private IEnumerator recorder()
    {
        _aT = new AnalogTransform();
        _startTime = Time.time;

        while (_bIsRecording)
        {
            yield return new WaitForSeconds(1 / _recordRate);
            _aT.Record(_t, Time.time - _startTime);
        }

        _aT.Duration = Time.time - _startTime;
        _aT.SampleRate = _recordRate;

        // Message
        GlobalEventSystem.Instance.InvokeEvent("TR-Save", _aT, gameObject);
    }

    // Public methods

    public void StartRecording()
    {
        if (_bIsRecording == true)
            return;
        _bIsRecording = true;
        StartCoroutine(recorder());
    }

    public void StopRecord()
    {
        if (_bIsRecording == false)
            return;
        _bIsRecording = false;
    }
}
