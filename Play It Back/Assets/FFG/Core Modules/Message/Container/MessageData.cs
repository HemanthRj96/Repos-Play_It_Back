using UnityEngine;


namespace FFG.Message
{
    /// <summary>
    /// Container struct used to parse data that has to be passed
    /// </summary>
    public struct MessageData : IMessage
    {
        public MessageData(object data = null, GameObject source = null)
        {
            Data = data;
            Source = source;
        }

        public object Data { get; set; }
        public GameObject Source { get; set; }
    }
}