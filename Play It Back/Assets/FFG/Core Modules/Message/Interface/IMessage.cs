using UnityEngine;


namespace FFG.Message
{
    public interface IMessage
    {
        object Data { get; set; }
        GameObject Source { get; set; }
    } 
}