using UnityEngine;

namespace FFG.Message.Internal
{
    [System.Serializable]
    public class MessageSettings
    {
        public string MessageName;
        public EMessageExecutionBegin MessageExecutionBegin;
        public bool ShouldCreateNewMessage;
        public float MessageInvokeDelay;
        public EMessageExecutionEnd MessageExecutionEnd;
        public string NextMessageName;
        public MessageComponent NextMessage;
        public float DestroyDelay;
#if UNITY_EDITOR
        [SerializeField]
        private int selection;
#endif
    }
}
