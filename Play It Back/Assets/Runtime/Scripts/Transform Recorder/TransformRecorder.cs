using FFG.Message;
using FFG.TrasformRecorder.Internal;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class TransformRecorder : MonoBehaviour
{
    [System.Serializable]
    public struct SaveData
    {
        public List<Vector3> Position;
        public List<Quaternion> Rotation;
        public List<Vector3> Scale;
        public List<float> Time;
        public Vector2 TimeStamp;
    }

    public static TransformRecorder Instance = null;

    private static readonly string FILE_PATH = "/Temp";
    private readonly float RECORD_RATE = 24;

    private float _startTime = 0;
    private bool _isRecording = false;

    private Dictionary<string, Transform> _transformData = new Dictionary<string, Transform>();
    private Dictionary<string, AnalogTransform> _analogTransformData = new Dictionary<string, AnalogTransform>();
    private Vector2 _activeTimeStamp;

    public float FrameRate => RECORD_RATE;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        _transformData.Clear();
        _analogTransformData.Clear();

        Message.CreateMessage(startRecord, "TR:start-rec");
        Message.CreateMessage(stopRecord, "TR:stop-rec");
        Message.CreateMessage(null, "TR:save");
        Message.CreateMessage(null, "TR:start-pb");
        Message.CreateMessage(null, "TR:stop-pb");
    }

    private void OnDestroy()
    {
        _transformData.Clear();
        _analogTransformData.Clear();

        Message.DeleteMessage("TR:start-rec");
        Message.DeleteMessage("TR:stop-rec");
        Message.DeleteMessage("TR:start-pb");
        Message.DeleteMessage("TR:stop-pb");
    }

    private void OnDisable()
    {
        _transformData.Clear();
        _analogTransformData.Clear();
    }

    private void writeData(AnalogTransform data, string name)
    {
        int totalKeys = data.Position.GetKeyCount();

        List<Vector3> position = new List<Vector3>();
        List<Quaternion> rotation = new List<Quaternion>();
        List<Vector3> scale = new List<Vector3>();
        List<float> time = new List<float>();
        Vector2 timeStamp = data.TimeStamp;

        Keyframe[] xPos = data.Position.X.keys;
        Keyframe[] yPos = data.Position.Y.keys;
        Keyframe[] zPos = data.Position.Z.keys;

        Keyframe[] xRot = data.Rotation.X.keys;
        Keyframe[] yRot = data.Rotation.Y.keys;
        Keyframe[] zRot = data.Rotation.Z.keys;
        Keyframe[] wRot = data.Rotation.W.keys;

        Keyframe[] xSca = data.Scale.X.keys;
        Keyframe[] ySca = data.Scale.Y.keys;
        Keyframe[] zSca = data.Scale.Z.keys;


        for (int i = 0; i < totalKeys; ++i)
        {
            position.Add(new Vector3(xPos[i].value, yPos[i].value, zPos[i].value));
            rotation.Add(new Quaternion(xRot[i].value, yRot[i].value, zRot[i].value, wRot[i].value));
            scale.Add(new Vector3(xSca[i].value, ySca[i].value, zSca[i].value));
            time.Add(xPos[i].time);
        }

        SaveData saveData = new SaveData()
        {
            Position = position,
            Rotation = rotation,
            Scale = scale,
            Time = time,
            TimeStamp = timeStamp
        };

        if (!Directory.Exists(Application.persistentDataPath + FILE_PATH))
            Directory.CreateDirectory(Application.persistentDataPath + FILE_PATH);
        
        FFG.JsonSaver.JsonSaver.WriteData(saveData, Application.persistentDataPath + FILE_PATH, name);

        print($"Finished saving at : {Application.persistentDataPath + FILE_PATH}/{name}");
    }

    private IEnumerator recorder()
    {
        _analogTransformData.Clear();
        _transformData.ToList().ForEach(t => _analogTransformData.Add(t.Key, new AnalogTransform()));
        var keys = _transformData.Keys.ToList();

        print("Start recording");
        foreach (var key in _transformData)
            print(key);

        while (_isRecording)
        {
            yield return new WaitForSeconds(1 / RECORD_RATE);
            foreach (var key in keys)
                _analogTransformData[key].Record(_transformData[key], Time.time - _startTime);
        }

        //On recorder stop
        _activeTimeStamp = new Vector2(0, Time.time - _startTime);

        foreach(var aData in _analogTransformData.ToList())
        {
            aData.Value.TimeStamp = _activeTimeStamp;
            writeData(aData.Value, aData.Key);
        }

        Message.InvokeMessage("TR:save", _activeTimeStamp);
    }

    private void startRecord(IMessage obj)
    {
        if (_isRecording == true)
            return;
        _isRecording = true;
        StartCoroutine(recorder());
    }

    private void stopRecord(IMessage obj)
    {
        if (_isRecording == false)
            return;

        _isRecording = false;
    }

    public void AddTransformToRecorder(Transform transform, string name)
    {
        if (!_transformData.ContainsKey(name) && transform != null && !string.IsNullOrEmpty(name))
        {
            print($"Added : {name}");
            _transformData.Add(name, transform);
        }
    }

    public AnalogTransform ReadData(string name)
    {
        AnalogVector position = new AnalogVector();
        AnalogQuaternion rotation = new AnalogQuaternion();
        AnalogVector scale = new AnalogVector();

        // Read data
        FFG.JsonSaver.JsonSaver.ReadData(Application.persistentDataPath + "/Temp", name, out SaveData data);

        if (data.Position == null || data.Position.Count == 0)
        {
            Debug.LogError("Data loading failed!!");
            return null;
        }

        int count = data.Position.Count;

        for (int i = 0; i < count; ++i)
        {
            Vector3 pos = data.Position[i];
            Quaternion rot = data.Rotation[i];
            Vector3 sca = data.Scale[i];
            float time = data.Time[i];

            position.X.AddKey(time, pos.x);
            position.Y.AddKey(time, pos.y);
            position.Z.AddKey(time, pos.z);

            rotation.X.AddKey(time, rot.x);
            rotation.Y.AddKey(time, rot.y);
            rotation.Z.AddKey(time, rot.z);
            rotation.W.AddKey(time, rot.w);

            scale.X.AddKey(time, sca.x);
            scale.Y.AddKey(time, sca.y);
            scale.Z.AddKey(time, sca.z);
        }

        return new AnalogTransform()
        {
            Position = position,
            Rotation = rotation,
            Scale = scale,
            TimeStamp = data.TimeStamp
        };
    }
}