using System.Collections.Generic;
using System.Linq;
using JetBrains.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;

namespace NPC
{
    public class WaitingZone : MonoBehaviour, IWaitingZone
    {
        [SerializeField]
        private float radius;
        private readonly HashSet<BaseAgent> _candidates = new HashSet<BaseAgent>();
        private readonly Dictionary<BaseAgent, Conversation> _activeConversations = 
            new Dictionary<BaseAgent, Conversation>();

        public bool HasInterlocutorCandidate()
        {
            return (_candidates.Any());
        }

        public float Radius => radius; 

        public void AddInterlocutorCandidate(BaseAgent interlocutor)
        {
            if (Random.Range(0f, 1f) > 0.5f)
            {
                _candidates.Add(interlocutor);
                MakeConversationFromScratch();
            }
            else
            {
                JoinConversation(interlocutor, out var success);
                if (!success)
                {
                    _candidates.Add(interlocutor);
                    MakeConversationFromScratch();
                }
            }
        }

        private void JoinConversation(BaseAgent interlocutor, out bool success)
        {
            foreach (var conversation in 
                _activeConversations.Values.Distinct())
            {
                var joiningEvent = new RandomEvent(() =>
                {
                    return interlocutor.Personality.ConversationJoiningProbability(conversation);
                });
                if (joiningEvent.HasEventHappened())
                {
                    success = true;
                    return;
                }
            }

            success = false;
        }

        private void MakeConversationFromScratch()
        {
            var candidateList = _candidates.ToList();
            var interlocutors = new List<BaseAgent>(){candidateList.First()};
            
            for (int j = 1; j < candidateList.Count; j++)
            {
                var j1 = j;
                var chatEvent = new RandomEvent(() =>
                    candidateList.First().Personality.ChatProbability(candidateList[j1].Personality));
                if (chatEvent.HasEventHappened()) 
                    interlocutors.Add(candidateList[j]);
            }

            if (interlocutors.Count > 1)
            {
                var conversation =
                    new Conversation(Random.Range(0, 1), interlocutors.First(), interlocutors.Last());
                foreach (var candidate in _candidates)
                {
                    var joiningEvent = new RandomEvent(
                        () => candidate.Personality.ConversationJoiningProbability(conversation));

                    if (joiningEvent.HasEventHappened())
                    {
                        conversation.Add(candidate);
                    }

                    foreach (var agent in conversation.Interlocutors)
                    {
                        _activeConversations.Add(agent, conversation);
                    }
                    _candidates.RemoveWhere(agent => 
                        conversation.Interlocutors.Contains(agent));
                }
            }
            
        }

        public Conversation GetConversation(BaseAgent interlocutor)
        {
            Assert.IsTrue(_activeConversations.ContainsKey(interlocutor),
                "_activeConversations.ContainsKey(interlocutor)");
            return _activeConversations[interlocutor];
        }

        public bool HasConversation(BaseAgent baseAgent)
        {
            return _activeConversations.ContainsKey(baseAgent);
        }
    }
}