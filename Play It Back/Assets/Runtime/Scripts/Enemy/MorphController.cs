using FFG.TrasformRecorder.Internal;
using System.Collections;
using UnityEngine;


public class MorphController : MonoBehaviour
{
    [SerializeField]
    private Color startingColor;
    [SerializeField]
    private Color endingColor;

    private readonly float _recordRate = 60;

    private AnalogTransform _aTransform;
    private SpriteRenderer _sp;

    private void Awake()
    {
        _sp = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(locomotionUpdate());
        setDefaultColor();
    }

    private void setDefaultColor() => _sp.color = startingColor;

    private IEnumerator locomotionUpdate()
    {
        _aTransform = TransformRecorder.Instance.ReadData("Player");
        float timer = 0;

        if (_aTransform != null)
        {
            float totalTime = _aTransform.GetTimeStamp().y;
            timer = totalTime;
            while (timer > 0)
            {
                yield return new WaitForSeconds(1 / _recordRate);
                _aTransform.MorphTransform(transform, timer);
                timer -= 1 / _recordRate;

                _sp.color = Color.Lerp(startingColor, endingColor, 1 - timer / totalTime);
            }
        }

        gameObject.SetActive(false);
    }
}
