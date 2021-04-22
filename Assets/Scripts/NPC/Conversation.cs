using Agent;

namespace NPC
{
    public class Conversation
    {
        private bool _isFirstSpeaking;
        private readonly Interlocutor First;
        private readonly Interlocutor Second;

        public Conversation(Interlocutor first, Interlocutor second)
        {
            First = first;
            Second = second;
        }

        public Interlocutor GetInterlocutor(BaseAgent baseAgent)
        {
            if (First.Agent == baseAgent)
            {
                return Second;
            }
            return First;
        }

        public void RequestLine()
        {
            throw new System.NotImplementedException();
        }

        public bool HasRequestedLine(BaseAgent baseAgent)
        {
            throw new System.NotImplementedException();
        }
    }
}