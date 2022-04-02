using FFG.Message;
using FFG.TrasformRecorder.Internal;
using System.Collections;
using UnityEngine;


public class testing_01 : MonoBehaviour
{
    private void Start()
    {
        TransformRecorder.Instance.AddTransformToRecorder(transform, "Test");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Message.InvokeMessage("TR:start-rec");
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Message.InvokeMessage("TR:stop-rec");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            StartCoroutine(beginPlayback());
        }
    }

    private IEnumerator beginPlayback()
    {
        print("Begin playback");
        AnalogTransform analogTransform = new AnalogTransform();
        analogTransform = TransformRecorder.Instance.ReadData("Test");
        float timer = 0;
        float frameTime = 1 / 60f;

        if (analogTransform != null)
        {
            print(analogTransform.TimeStamp);
            timer = analogTransform.TimeStamp.y;

            while (timer > 0)
            {
                yield return new WaitForSeconds(frameTime);
                analogTransform.MorphTransform(transform, timer);
                timer -= frameTime;
                print(timer);
            }
        }
        print("Finish");
    }
}
