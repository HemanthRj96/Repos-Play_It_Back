using System;


namespace FFG.Message.Internal
{
    public struct MessageHandler
    {
        public MessageHandler(Action<IMessage> listener , bool multiple)
        {
            Listener = delegate { };
            Listener = listener;
            Multiple = multiple;
        }

        public Action<IMessage> Listener;
        public bool Multiple;
    }
}