using System;
using UnityEngine;


public class testing_01 : MonoBehaviour
{
    private void Start()
    {
        GlobalEventSystem.Instance.AddListener("Testing Event", testingMethod);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            GlobalEventSystem.Instance.InvokeEvent("Testing Event", Time.time);
    }

    private void testingMethod(object data)
    {
        print($"Invoked at : {(float)data}");
    }
}
