using UnityEngine;


public class AnalogQuaternion
{
    public AnimationCurve X;
    public AnimationCurve Y;
    public AnimationCurve Z;
    public AnimationCurve W;

    public AnalogQuaternion()
    {
        X = new AnimationCurve();
        Y = new AnimationCurve();
        Z = new AnimationCurve();
        W = new AnimationCurve();
    }

    public void Add(Quaternion rotation, float time)
    {
        X.AddKey(time, rotation.x);
        Y.AddKey(time, rotation.y);
        Z.AddKey(time, rotation.z);
        W.AddKey(time, rotation.w);
    }

    public int GetKeyCount() => X.length;

    public Quaternion Get(float time) => new Quaternion(X.Evaluate(time), Y.Evaluate(time), Z.Evaluate(time), W.Evaluate(time));
}
