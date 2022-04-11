using UnityEngine;


public static class ExtensionMethods
{
    public static Vector4 GetCameraBoundsInWorldSpace(this Camera camera)
    {
        float v = camera.orthographicSize;
        float h = v * Screen.width / Screen.height;

        return new Vector4(-h, -v, h, v);
    }
}