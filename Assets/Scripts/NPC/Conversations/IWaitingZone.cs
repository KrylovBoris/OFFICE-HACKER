using Agent;
using UnityEngine;

namespace NPC
{
    public interface IWaitingZone
    {
        float Radius { get; }
        bool HasInterlocutorCandidate();
        void AddInterlocutorCandidate(BaseAgent interlocutor);
        Conversation GetConversation(BaseAgent interlocutor);
        bool HasConversation(BaseAgent baseAgent);
        void RemoveAgent(BaseAgent agent);
    }
}