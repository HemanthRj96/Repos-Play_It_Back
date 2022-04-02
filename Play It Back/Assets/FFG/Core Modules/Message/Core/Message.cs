using System;
using System.Collections.Generic;
using UnityEngine;
using FFG.Message.Internal;


namespace FFG.Message
{
    /// <summary>
    /// Class used to create & invoke messages and pass data
    /// </summary>
    public static class Message
    {
        /*.............................................Private Fields.......................................................*/

        private static Dictionary<string, MessageHandler> s_messageHandlers = new Dictionary<string, MessageHandler>();

        /*.............................................Public Methods.......................................................*/

        /// <summary>
        /// Method to create a messsage
        /// </summary>
        /// <param name="message">Target listener</param>
        /// <param name="messageName">Name for the message</param>
        /// <param name="multipleSubscription">Set this as true if you want multiple listeners</param>
        public static void CreateMessage(Action<IMessage> message, string messageName, bool multipleSubscription = true)
        {
            if (s_messageHandlers.ContainsKey(messageName) == false)
                s_messageHandlers.Add(messageName, new MessageHandler(message, multipleSubscription));
            else if (s_messageHandlers[messageName].Multiple == true)
            {
                MessageHandler handler = s_messageHandlers[messageName];
                handler.Listener += message;
                s_messageHandlers[messageName] = handler;
            }
            else
                Debug.LogWarning("Cannot create this message since it already exists!");
        }

        /// <summary>
        /// Method to invoke a message
        /// </summary>
        /// <param name="messageName">Name fo the message</param>
        /// <param name="messageData">Message data</param>
        /// <param name="messageInstigator">Target message instigator</param>
        public static void InvokeMessage(string messageName, object messageData = null, GameObject messageInstigator = null)
        {
            if(s_messageHandlers.ContainsKey(messageName))
                s_messageHandlers[messageName].Listener?.Invoke(new MessageData(messageData, messageInstigator));
            else
            {
                Debug.LogWarning($"This message [{messageName}] do not exist!");
                return;
            }
        }

        /// <summary>
        /// Method to add a message listener
        /// </summary>
        /// <param name="messageName">Name for the message</param>
        /// <param name="message">Target listener</param>
        public static void AddMessageListener(string messageName, Action<IMessage> message)
        {
            if (s_messageHandlers.ContainsKey(messageName))
            {
                if (s_messageHandlers[messageName].Multiple == true)
                {
                    MessageHandler handler = s_messageHandlers[messageName];
                    handler.Listener += message;
                    s_messageHandlers[messageName] = handler;
                }
                else
                    Debug.LogWarning("Cannot attach multiple listeners to this message!");
            }
            else
                CreateMessage(message, messageName);
        }

        /// <summary>
        /// Method to remove a message listener
        /// </summary>
        /// <param name="messageName">Name of the message listener</param>
        /// <param name="message">Listner to be removed</param>
        public static void RemoveListener(string messageName, Action<IMessage> message)
        {
            if (s_messageHandlers.ContainsKey(messageName) == false)
            {
                Debug.LogWarning($"This message [{messageName}] do not exist!");
                return;
            }

            MessageHandler handler = s_messageHandlers[messageName];
            handler.Listener -= message;
            s_messageHandlers[(messageName)] = handler;
        }

        /// <summary>
        /// Method to delete a message
        /// </summary>
        /// <param name="messageName">Name of the message</param>
        public static void DeleteMessage(string messageName)
        {
            if (s_messageHandlers.ContainsKey(messageName) == false)
            {
                Debug.LogWarning($"This message [{messageName}] do not exist!");
                return;
            }

            s_messageHandlers.Remove(messageName);
        }
    }
}
