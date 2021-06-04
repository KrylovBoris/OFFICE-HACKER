using System.Collections.Generic;
using System.Linq;
using Agent;
using GlobalMechanics;
using JetBrains.Collections.Viewable;
using JetBrains.Lifetimes;
using UnityEngine;
using UnityEngine.Assertions;

namespace NPC
{
    public class Conversation : ISightController
    {
        public bool IsAbandoned => _interlocutors.Count < 2;
        private int _speakerIndex;
        private List<BaseAgent> _interlocutors;
        private LifetimeDefinition _conversationLifetime;
        
        public IReadOnlyList<BaseAgent> Interlocutors => _interlocutors;

        private bool _hasSpeakerFinished;
        public bool SpeakerSpeaking { get; private set; }

        public Conversation(int speaker, BaseAgent first, BaseAgent second)
        {
            _speakerIndex = speaker;
            _interlocutors = new List<BaseAgent>(){first, second};
            _conversationLifetime = Lifetime.Eternal.CreateNested();
            first.SetConversation(this);
            second.SetConversation(this);
        }

        public BaseAgent GetSpeaker()
        {
            return _interlocutors[_speakerIndex];
        }

        public void RequestLine()
        {
            throw new System.NotImplementedException();
        }

        public void SpeakerStarted(BaseAgent agent)
        {
            Assert.IsTrue(agent == GetSpeaker(), "agent == GetSpeaker()");
            SpeakerSpeaking = true;
            _hasSpeakerFinished = false;
            var definition = Lifetime.Define(_conversationLifetime.Lifetime);
            agent.GetComponent<AnimationManager>().IsSpeaking.WhenFalse(definition.Lifetime,
                (lf) =>
                {
                    SpeakerFinished(agent);
                    definition.Terminate();
                });
        }

        public void SpeakerFinished(BaseAgent agent)
        {
            SpeakerSpeaking = false;
            _hasSpeakerFinished = true;
            FindNewSpeaker(agent);
        }

        private void FindNewSpeaker(BaseAgent previousSpeaker)
        {
            var speakerProbabilities = Interlocutors.Where(a => a != previousSpeaker).Select(i => (i, i.Personality.GetProbability("Line")))
                .OrderByDescending(f => f.Item2).ToArray();
            for (var i = 0; i < speakerProbabilities.Count(); i++)
            {
                var probability = speakerProbabilities[i].Item2;
                var e = new RandomEvent(() => probability);
                if (e.HasEventHappened())
                {
                    _speakerIndex = _interlocutors.IndexOf(speakerProbabilities[i].Item1);
                }
            }
        }

        public bool HasRequestedLine(BaseAgent baseAgent)
        {
            return baseAgent == GetSpeaker() || _hasSpeakerFinished;
        }

        public void Add(BaseAgent newSpeaker)
        {
            _interlocutors.Add(newSpeaker);
            newSpeaker.SetConversation(this);
        }

        public void Remove(BaseAgent speaker)
        {
            if (GetSpeaker() == speaker)
            {
                if (SpeakerSpeaking)
                {
                    speaker.StopTalking();
                }
            }
            _interlocutors.Remove(speaker);
        }

        public Vector3 GetSightTarget() => GetSpeaker().Head.position;
    }
}