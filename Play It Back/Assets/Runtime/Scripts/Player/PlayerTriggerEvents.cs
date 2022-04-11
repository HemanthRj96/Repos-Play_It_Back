using UnityEngine;


public class PlayerTriggerEvents : MonoBehaviour
{
    readonly float _trasitionDelay = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.CompareTag("Key"))
                GlobalEventSystem.Instance.InvokeEvent("Key-Pickup");
            if (collision.CompareTag("Goal"))
            {
                GlobalEventSystem.Instance.InvokeEvent("Goal");
                // Do something here

                Timer.CreateCountdownTimer(() => { GlobalEventSystem.Instance.InvokeEvent("Level-Complete"); }, _trasitionDelay);
            }
            if (collision.CompareTag("Enemy"))
                GlobalEventSystem.Instance.InvokeEvent("Kill-Player");
        }
    }
}
