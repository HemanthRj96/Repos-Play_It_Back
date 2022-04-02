using FFG.Message;
using UnityEngine;


public class PlayerTriggerHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.CompareTag("Key"))
            {
                print("Key");
                Message.InvokeMessage("TR:stop-rec");
            }
            //if (collision.CompareTag("Goal"))
            //    Message.InvokeMessage("TR:start-pb");
            //if (collision.CompareTag("Enemy"))
            //    Message.InvokeMessage("LR:");
        }
    }
}
