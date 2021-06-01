using System.Collections.Generic;
using System.Linq;
using Agent;

namespace NPC
{
    public class Conversation
    {
        private int _speakerIndex;
        private List<BaseAgent> _interlocutors;
        public IReadOnlyList<BaseAgent> Interlocutors => _interlocutors;

        public Conversation(int speaker, BaseAgent first, BaseAgent second)
        {
            _speakerIndex = speaker;
            _interlocutors = new List<BaseAgent>(){first, second};
        }

        public BaseAgent GetSpeaker()
        {
            return _interlocutors[_speakerIndex];
        }

        public void RequestLine()
        {
            throw new System.NotImplementedException();
        }

        public bool HasRequestedLine(BaseAgent baseAgent)
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            foreach (var i in _interlocutors)
            {
                i.DestroyConversation();
            }
        }

        public void Add(BaseAgent newSpeaker)
        {
            
        }
    }
}