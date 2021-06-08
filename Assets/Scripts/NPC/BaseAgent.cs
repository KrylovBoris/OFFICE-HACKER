using System;
using System.Collections.Generic;
using System.Linq;
using GlobalMechanics;
using HierarchicalTaskNetwork;
using Interactions;
using NPC.Conversations;
using NPC.Tokens;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace NPC
{
    public partial class BaseAgent : MonoBehaviour
    {
        [SerializeField]
        private float standardStoppingDistance = 0.05f;
        
        
        private uint _id;
        private static uint _agentsCount;
        protected HTN_planner HtnPlanner;

        public WorkPlace workingPlace;
        public Department department;

        [SerializeField]
        private Transform _head;

        public Transform Head => _head;

        public Personality Personality => _personality;
        
        public delegate HtnTask TaskCreationProcedure();
        protected Dictionary<string, TaskCreationProcedure> AgentTaskDictionary;
        private float _waitTimer = 0f;
        private Transform _navMeshDestination;
        
        private AiToken _activeToken;
        
        private TokenDispenser _activeZone;

        private IWaitingZone WaitingZone
        {
            get
            {
                try
                {
                    Assert.IsNotNull(_activeZone, $"{name} has no active zone");
                    return _activeZone.WaitingZone;
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }
        
        private NavMeshAgent _navMeshAgent;
        private AnimationManager _animationManager;
        [SerializeField]private Personality _personality;

        private InterruptionFlagsHandler _handler;
        public InterruptionFlagsHandler Handler => _handler;
        
        //Action Flags
        private bool _isSitting = false;
        private bool _isTyping = false;
        private bool _isTalking = false;
        private bool _authenSuccess;
        
        //Command flags
        private bool _hasToStop;
        [SerializeField] private float minWaitTime;
        [SerializeField] private float maxWaitTime;
        
        private Conversation _currentConversation;
        
        public uint ID => _id;
        public bool IsOccupied => 
            _isTyping || _isTalking || IsSearchingArchives() || _isSitting;

        [SerializeField]
        private Transform head;

        private bool _hasToWorkOnPc;

        // Start is called before the first frame update
        protected void Awake()
        {
            _handler = new InterruptionFlagsHandler();
            _id = _agentsCount++;
            SetDictionary();
        }

        void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animationManager = GetComponent<AnimationManager>();
            
            var noteForgottenEvent = new RandomEvent(() => _personality.GetProbability("NoteLost"));
            if (noteForgottenEvent.HasEventHappened())
            {
                workingPlace.SpawnExternalNote(_personality.NoteText());
            }
        }

        // Update is called once per frame
        void Update()
        {
            _animationManager.UpdateMoveAnimation(_navMeshAgent.velocity.normalized);
            if (_waitTimer > 0) _waitTimer -= Time.deltaTime;
        }

        public void RegistryHtnPlanner(HTN_planner planner)
        {
            if (HtnPlanner == null) HtnPlanner = planner;
        }

        private bool IsAgentInPosition(Vector3 pos)
        {
            // if ((transform.position - pos).magnitude <= 0.05)
            //     Debug.Log((transform.position - pos).magnitude);
            return (transform.position - pos).magnitude <= _navMeshAgent.stoppingDistance;
        }
        public HtnTask GetTaskByName(string task)
        {
            if (AgentTaskDictionary.ContainsKey(task))
            {
                return AgentTaskDictionary[task].Invoke();
            } 
            return CreateEmptyTask();
        }
         
        protected virtual void SetDictionary()
        {
            AgentTaskDictionary = new Dictionary<string, TaskCreationProcedure>();
            var agentType = this.GetType();
            AgentTaskDictionary = agentType.GetMethods().Where(m => m.GetCustomAttributes(typeof(HtnRootTaskAttribute), false).Any())
                .ToDictionary(
                    methodInfo =>
                    {
                        var rootTask =
                            (HtnRootTaskAttribute) methodInfo.GetCustomAttributes(typeof(HtnRootTaskAttribute), false).First();
                        return rootTask.TaskName;
                    }, 
                    info =>
                    {
                        return (TaskCreationProcedure) (() => (HtnTask)info.CreateDelegate(typeof(Func<HtnTask>), this).DynamicInvoke());
                    });
        }

        public void FinalizeArchiveSearch()
        {
            _navMeshAgent.enabled = true;
            _activeZone = null;
            _activeToken.Finish();
            _activeToken = null;
            _hasToWorkOnPc = true;
        }

        public void FinalizeFaxSearch()
        {
            _navMeshAgent.enabled = true;
            _activeZone = null;
            _activeToken.Finish();
            _activeToken = null;
            _hasToWorkOnPc = true;
        }

        private void SetRandomActiveArchiveZone()
        {
            _activeZone = department.GetRandomArchiveNavMeshDestination();
        }

        private void SetRandomActiveFaxZone()
        {
            _activeZone = department.GetRandomFaxMachineDestination();
        }

        private void AddAgentToZone(BaseAgent agent)
        {
            WaitingZone.AddInterlocutorCandidate(agent);
        }

        private Vector3 GetDestination()
        {
            return _navMeshDestination.position;
        }

        public void SetConversation(Conversation c)
        {
            _currentConversation = c;
        }

        public void DestroyConversation()
        {
            _currentConversation = null;
        }
    }
}