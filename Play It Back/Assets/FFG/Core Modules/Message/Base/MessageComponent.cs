using UnityEngine;
using FFG.Message.Internal;


namespace FFG.Message
{
    /// <summary>
    /// Component should only be used by inheritance of this base class
    /// </summary>
    public abstract class MessageComponent : MonoBehaviour
    {
        #region Editor
#if UNITY_EDITOR
        // Custom property drawer
        [HideInInspector] public bool isChained = false;
        [HideInInspector] public MessageComponent chainedComponent = null;

        public EMessageExecutionBegin actionExecutionMode => _messageSettings.MessageExecutionBegin;
#endif
        #endregion Editor

        /*.............................................Serialized Fields....................................................*/

#pragma warning disable 0649
        [SerializeField] private bool dontDestroyOnLoad = false;
        [SerializeField] private MessageSettings _messageSettings;
#pragma warning restore 0649

        /*.............................................Private Fields.......................................................*/

        private IMessage _outputData = null;
        private IMessage _inputData = null;

        /*.............................................Private Methods......................................................*/

        /// <summary>
        /// Bootstraps action data and initializes everything properly
        /// </summary>
        private void bootstrapper()
        {
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(this);
            if (_messageSettings.MessageExecutionBegin == EMessageExecutionBegin.ExecuteExternally || _messageSettings.ShouldCreateNewMessage)
                Message.CreateMessage(internalMessageHandler, _messageSettings.MessageName, false);
        }

        /// <summary>
        /// Method to invoke action with delay
        /// </summary>
        /// <param name="actionData"></param>
        private void internalMessageHandler(IMessage actionData = null)
        {
            _inputData = actionData;
            Invoke(nameof(internalInvoke), _messageSettings.MessageInvokeDelay);
        }

        /// <summary>
        /// Helper method for internalMessageHandler
        /// </summary>
        private void internalInvoke()
        {
            onInvoke(_inputData);

            if (_messageSettings.MessageExecutionEnd == EMessageExecutionEnd.ExecuteAnotherMessage)
            {
                if (_messageSettings.NextMessage != null)
                    _messageSettings.NextMessage.internalMessageHandler(_outputData);
                else
                    Message.InvokeMessage(_messageSettings.NextMessageName, _outputData.Data, _outputData.Source);
            }
            else if (_messageSettings.MessageExecutionEnd == EMessageExecutionEnd.DestroySelf)
            {
                Message.DeleteMessage(_messageSettings.MessageName);
                Destroy(gameObject, _messageSettings.DestroyDelay);
            }
        }

        /*.............................................Protected Methods....................................................*/

        /// <summary>
        /// Call base method to prevent unknown behaviour
        /// </summary>
        protected virtual void Awake()
        {
            bootstrapper();
        }

        /// <summary>
        /// Call base method to prevent unknown behaviour
        /// </summary>
        protected virtual void Start()
        {
            if (_messageSettings.MessageExecutionBegin == EMessageExecutionBegin.ExecuteOnStart)
                internalMessageHandler();
        }

        /// <summary>
        /// Call base method to prevent unknown behaviour
        /// </summary>
        protected virtual void Update()
        {
            if (_messageSettings.MessageExecutionBegin == EMessageExecutionBegin.ExecuteOnUpdate)
                internalInvoke();
        }

        /// <summary>
        /// Call base method to prevent unknown behaviour
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (_messageSettings.MessageExecutionBegin == EMessageExecutionBegin.ExecuteOnFixedUpdate)
                internalInvoke();
        }

        /// <summary>
        /// Call base method to prevent unknown behaviour
        /// </summary>
        protected virtual void OnDestroy()
        {
            Message.DeleteMessage(_messageSettings.MessageName);
        }

        /// <summary>
        /// Call this method create an outgoing data
        /// </summary>
        /// <param name="data">Data to be passed</param>
        /// <param name="instigatingObject">The instigating object</param>
        protected void ConstructOutgoingData(object data = null, GameObject instigatingObject = null)
        {
            _outputData = new MessageData(data, instigatingObject);
        }

        /// <summary>
        /// Function must be implemented that runs all the custom logic for this message component
        /// </summary>
        /// <param name="parameters">Parameters received</param>
        protected abstract void onInvoke(IMessage parameters);
    }
}