using Agent;
using UnityEngine;

namespace NPC
{
    public interface IWaitingZone
    {
        bool HasInterlocutorCandidate();
        void AddInterlocutorCandidate();
        Interlocutor GetInterlocutor();
    }

    public readonly struct Interlocutor
    {
        public readonly Transform Head;
        public readonly Personality Personality;
        public readonly BaseAgent Agent;

        public Interlocutor(Transform head, Personality personality, BaseAgent agent)
        {
            Head = head;
            Personality = personality;
            Agent = agent;
        }
    }
}