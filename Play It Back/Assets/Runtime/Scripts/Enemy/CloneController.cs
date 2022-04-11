using System.Collections;
using UnityEngine;


public class CloneController : MonoBehaviour
{
    // Serialize field

    [SerializeField]
    private Color startingColor;
    [SerializeField]
    private Color endingColor;

    // Private field

    private SpriteRenderer _sp;
    private AnalogTransform _aT;

    // Private method

    private void Awake() => _sp = GetComponent<SpriteRenderer>();

    private void Start() => GlobalEventSystem.Instance.AddListener("Kill-Player", () => { StopAllCoroutines(); });

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(locomotionUpdate());
        _sp.color = startingColor;
    }

    private IEnumerator locomotionUpdate()
    {
        if (_aT != null)
        {
            float frameRate = _aT.SampleRate;
            float totalTime = _aT.Duration;
            float timer = 0;

            Invoke(nameof(deactivateSelf), totalTime + 0.5f);
            timer = totalTime;

            while (timer > 0)
            {
                yield return new WaitForSeconds(1 / frameRate);
                _aT.CloneTransform(transform, timer);
                timer -= 1 / frameRate;

                _sp.color = Color.Lerp(startingColor, endingColor, 1 - timer / totalTime);
            }
        }
    }

    private void deactivateSelf() => gameObject.SetActive(false);

    // Public methods

    public void SetAnalogTransform(AnalogTransform aT) => _aT = aT;
}
