using System.Collections.Generic;
using UnityEngine;


public class AnalogTransform
{
    [System.Serializable]
    internal struct SaveData
    {
        public List<Vector3> Position;
        public List<Quaternion> Rotation;
        public List<Vector3> Scale;
        public List<float> Time;
        public float Duration;
    }

    public AnalogVector Position;
    public AnalogQuaternion Rotation;
    public AnalogVector Scale;
    public float Duration;
    public float SampleRate;

    public AnalogTransform()
    {
        Position = new AnalogVector();
        Rotation = new AnalogQuaternion();
        Scale = new AnalogVector();
    }


    /// <summary>
    /// Method to record transform
    /// </summary>
    /// <param name="transform">Target tranform</param>
    /// <param name="time">Target time</param>
    public void Record(Transform transform, float time)
    {
        Position.Add(transform.position, time);
        Rotation.Add(transform.rotation, time);
        Scale.Add(transform.localScale, time);
    }

    /// <summary>
    /// Call this method to change transform based on timeline
    /// </summary>
    /// <param name="transform">Target tranform to be updated</param>
    /// <param name="time">Target time in the timeline</param>
    public void CloneTransform(Transform transform, float time)
    {
        transform.position = GetPosition(time);
        transform.rotation = GetRotation(time);
        transform.localScale = GetScale(time);
    }

    /// <summary>
    /// Return the position from the timeline
    /// </summary>
    /// <param name="time">Target time in the timeline</param>
    public Vector3 GetPosition(float time) => Position.Get(time);

    /// <summary>
    /// Return the rotation from the timeline
    /// </summary>
    /// <param name="time">Target time in the timeline</param>
    public Quaternion GetRotation(float time) => Rotation.Get(time);

    /// <summary>
    /// Returns the scale from the timeline
    /// </summary>
    /// <param name="time">Target time in the timeline</param>
    public Vector3 GetScale(float time) => Scale.Get(time);

    public override string ToString()
    {
        return $" Position : {Position}\n Rotation : {Rotation}\n Scale : {Scale}\n TimeStamp : {Duration}";
    }
}
