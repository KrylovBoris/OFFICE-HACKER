using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using HierarchicalTaskNetwork;
using NPC;

namespace Agent
{
    public partial class BaseAgent : MonoBehaviour
    {
        private uint _id;
        private static uint _agentsCount;
        protected HTN_planner HtnPlanner;

        public WorkPlace workingPlace;
        public Department department;
        
        [SerializeField]
        private float standardStoppingDistance = 0.05f;

        public delegate HtnTask TaskCreationProcedure();
        protected Dictionary<string, TaskCreationProcedure> AgentTaskDictionary;
        private float _waitTimer = 0f;
        private Transform _navMeshDestination;
        
        private AiToken _activeToken;
        private ArchiveTokenDispenser _activeArchiveZone;
        private FaxMachineTokenDispenser _activeFaxZone;

        private NavMeshAgent _navMeshAgent;
        private AnimationManager _animationManager;
        private Personality _personality;

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

        public bool IsOccupied
        {
            get { return _isTyping || _isTalking || IsSearchingArchives() || _isSitting; }
        }


        // Start is called before the first frame update
        protected void Awake()
        {
            _handler = new InterruptionFlagsHandler();
            _id = _agentsCount++;
            SetDictionary();
        }

        void Start()
        {
            _personality = GetComponent<Personality>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animationManager = GetComponent<AnimationManager>();
            
            var noteForgottenEvent = new RandomEvent(() => _personality.NoteLostProbability());
            if (noteForgottenEvent.HasEventHappened())
            {
                workingPlace.SpawnExternalNote(_personality.NoteText());
            }
        }

        public uint ID => _id;

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
            return (transform.position - pos).magnitude <= 0.05;
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
                        return (TaskCreationProcedure) (() => (HtnTask)info.CreateDelegate(typeof(Func<HtnTask>)).DynamicInvoke());
                    });


        }

        public void FinalizeArchiveSearch()
        {
            _navMeshAgent.enabled = true;
            _activeArchiveZone = null;
            _activeToken.Finish();
            _activeToken = null;
        }

        public void FinalizeFaxSearch()
        {
            _navMeshAgent.enabled = true;
            _activeFaxZone = null;
            _activeToken.Finish();
            _activeToken = null;
        }
    }
}