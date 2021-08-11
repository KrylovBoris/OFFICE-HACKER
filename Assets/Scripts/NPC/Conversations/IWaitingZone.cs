// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

namespace NPC.Conversations
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