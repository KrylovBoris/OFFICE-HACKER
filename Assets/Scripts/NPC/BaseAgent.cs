using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using HierarchicalTaskNetwork;

namespace Agent
{
    public partial class BaseAgent : MonoBehaviour
    {
        private uint _id;
        private static uint _agentsCount;
        protected HTN_planner HtnPlanner;
        protected CoroutineStarter CoroutineStarter;

        public WorkPlace workingPlace;
        public Department department;

        
        public delegate Task TaskCreationProcedure();
        protected Dictionary<string, TaskCreationProcedure> AgentTaskDictionary;
        private float _waitTimer = 0f;
        private Transform _archivePosition;
        private NavMeshAgent _navMeshAgent;
        private AnimationManager _animationManager;
        private Personality _personality;

        private InterruptionFlagsHandler _handler;

        public InterruptionFlagsHandler Handler => _handler;
        //Action Flags
        private bool _isSitting = false;
        private bool _isTyping = false;
        private bool _isSearchingArchives = false;
        private bool _isTalking = false;
        private bool _authenSuccess;

        //HTN conditions
        public bool IsSitting => _isSitting;
        public bool IsTyping => _isTyping;
        public bool IsSearchingArchives => _isSearchingArchives;
        public bool IsTalking => _isTalking;



        //Command flags
        private bool _hasToStop;

        public bool HasToStop
        {
            get
            {
                if (_hasToStop)
                {
                    _hasToStop = false;
                    return true;
                }

                return false;
            }
        }

        public bool IsDoingSomething
        {
            get { return _isTyping || _isTalking || _isSearchingArchives; }
        }

        public bool IsOccupied
        {
            get { return _isTyping || _isTalking || _isSearchingArchives || _isSitting; }
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
            CoroutineStarter = GetComponent<CoroutineStarter>();
            _animationManager = GetComponent<AnimationManager>();
            
            var noteForgottenEvent = new RandomEvent(() => _personality.NoteLostProbability());
            if (noteForgottenEvent.HasEventHappened())
            {
                workingPlace.SpawnExternalNote(_personality.NoteText());
            }
        }

        public uint ID
        {
            get { return _id; }
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


        #region Conditions

        public bool IsAgentInPosition(Vector3 pos)
        {
            if ((transform.position - pos).magnitude <= 0.05)
                Debug.Log((transform.position - pos).magnitude);
            return (transform.position - pos).magnitude <= 0.05;
        }

        #endregion

        #region Actions
        //Make vulnerable to peeking and enable UI
        private void StartTypingPassword()
        {
            workingPlace.StartTypingPassword(_personality.passwordTypingTime);
        }

        private void LogOut()
        {
            workingPlace.LogOut();
        }

        //Make invulnerable and disable UI
        private void SubmitPassword()
        {
            bool flag;
            workingPlace.LogIn(_personality.LogInId, _personality.KnownPassword, out flag);
            if (!flag) _handler.RaiseFlag(InterruptionFlag.WrongPassword);
            workingPlace.StopTypingPassword();
        }

        private void SitOnChair()
        {
            _animationManager.SitDownOnChair();
            _isSitting = true;
            _navMeshAgent.enabled = false;
            transform.parent = workingPlace.chair.transform;
            workingPlace.CharacterSeated(_personality);
        }

        private void StandUpFromChair()
        {
            _isSitting = false;
            _navMeshAgent.enabled = true;
            transform.parent = null;
            _animationManager.StandUpFromChair();
        }

        private void AlignWithVector(Vector3 vector, Vector3 position)
        {
            Debug.Log("Unfinished action \"align with chair\"");
            transform.position = position;
            transform.forward = vector;
        }

        private void StartTyping()
        {
            _isTyping = true;
            _animationManager.StartTyping();
        }

        private void TurnToComputer()
        {
            workingPlace.chair.Turn(90.0f);
        }

        private void TurnOnChair(float angle)
        {
            workingPlace.chair.ResetSeat();
        }

        private void TurnOnComputer()
        {
            workingPlace.computer.TurnOn();
        }
        

        private void StopTyping()
        {
            _isTyping = false;
            _animationManager.StopTyping();
        }

        public void StartSearchingArchives()
        {
            _isSearchingArchives = true;
            _navMeshAgent.enabled = false;
            _animationManager.StartSearchingFillingCabinet();
        }

        private void GoToArchives()
        {
            _archivePosition = department.GetRandomArchiveNavMeshDestination();
            _navMeshAgent.SetDestination(_archivePosition.position);
        }

        public void StopSearchingArchives()
        {
            _isSearchingArchives = false;
            _navMeshAgent.enabled = true;
        }

        #endregion

        #region Decompositions

        public Task[] DecomposeStopActivities()
        {
            var tasks = new List<Task>();

            if (_isTyping)
            {

                tasks.Add(CreateStopTyping());
            }
            else
            {
                if (_isSearchingArchives)
                {
                    tasks.Add(CreateStopSearchingArchives());
                }
            }

            if (_isSitting)
            {
                if (workingPlace.IsLoggedInCorrectProfile)
                {
                    tasks.Add(CreateTurnOff());
                }
                if (!workingPlace.CharacterCanStandUp())
                    tasks.Add(CreateTurnAwayFromComputer());
                tasks.Add(CreateStandUpFromChair());
            }

            if (_isTalking)
            {
                Debug.Log("Talking isn't emplemented");
            }

            tasks.Add(CreateEmptyTask());

            return tasks.ToArray();
        }

        protected Task[] DecomposeWork()
        {
            var tasks = new List<Task>();
            //if (IsOccupied) tasks.Add(CreateStopActivities());
            var t = UnityEngine.Random.Range(0f, 1f);
            if (t > 0.5f) tasks.Add(CreateSearchArchives());
            else tasks.Add(CreateWorkAtComputer());
            //tasks.Add(CreateSearchArchives());
            return tasks.ToArray();
        }

        protected Task[] DecomposeWorkAtComputer()
        {
            var tasks = new List<Task>();
            if (IsSearchingArchives)
                tasks.Add(CreateStopActivities());
            if (!IsAgentInPosition(workingPlace.navMeshDestination.position) && 
                !(_isSitting && transform.parent == workingPlace.chair.transform))
            {
                tasks.Add(CreateGoToWorkingPlace());
            }

            if (!IsSitting)
            {
                tasks.Add(CreateAlignWithWorkingPlaceChair());
                tasks.Add(CreateSitDown());
            }

            if (!workingPlace.IsChairFacingComputer())
            {
                tasks.Add(CreateTurnToComputer());
            }

            if (!workingPlace.computer.IsOn)
            {
                tasks.Add(CreateTurnOn());
            }
            
            if (!IsTyping)
            {
                tasks.Add(CreateStartTyping());
            }

            if (!workingPlace.IsLoggedInCorrectProfile)
            {
                tasks.Add(CreateLogIn());
            }
            tasks.Add(CreateWaitSomeTime(UnityEngine.Random.Range(10.0f, 30.0f)));
            //tasks.Add(CreateStopTyping());
            return tasks.ToArray();
        }

        protected Task[] DecomposeSearchArchives()
        {
            var tasks = new List<Task>();
            tasks.Add(CreateStopActivities());
            tasks.Add(CreateGoToArchives());
            tasks.Add(CreateAlignWithArchives());
            tasks.Add(CreateStartSearchingArchives());
            return tasks.ToArray();
        }

        protected Task[] DecomposeLogIn()
        {
            var tasks = new List<Task>();
            if (!workingPlace.IsLoggedInCorrectProfile)
            {
                tasks.Add(CreateLogOut());
                tasks.Add(CreateStartTypingPassword());
                tasks.Add(CreateTypingPassword());
                tasks.Add(CreateSubmitPassword());
            }
            tasks.Add(CreateEmptyTask());
            return tasks.ToArray();
        }


        #endregion

        #region Tasks

        public Task GetTaskByName(string task)
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
            AgentTaskDictionary.Add("Work", CreateWork);
            AgentTaskDictionary.Add("TalkThenWork", CreateTalkThenWork);
            AgentTaskDictionary.Add("SendPlayerAwayThenWork", CreateSendPlayerAwayThenWork);
            //TODO AgentTaskDictionary.Add("TalkThenLunch", CreateTalkThenLunch);
            //TODO AgentTaskDictionary.Add("SendPlayerAwayThenBreak", CreateSendPlayerAwayThenBreak);
        }

        protected SimpleTask CreateStopSearchingArchives()
        {
            Task.Condition[] finishConditions =
            {
                () => !IsSearchingArchives,
            };

            SimpleTask.TaskAction action = _animationManager.ForceStop;

            var task = new SimpleTask(
                this.name + "StopSearchingArchives",
                CoroutineStarter,
                action,
                Task.EmptyCondition,
                Task.EmptyCondition,
                finishConditions);
            return task;
        }
        
        protected SimpleTask CreateWaitSomeTime(float waitTime)
        {
            Task.Condition[] finishConditions = {() => _waitTimer <= 0};
            SimpleTask.TaskAction action = () =>
            {
                if (_waitTimer > 0) _waitTimer += waitTime;
                else _waitTimer = waitTime;
            };

            var task = new SimpleTask(
                this.name + "WaitSomeTime" + waitTime,
                CoroutineStarter,
                action,
                Task.EmptyCondition,
                Task.EmptyCondition,
                finishConditions);
            return task;
        }
        
        protected SimpleTask CreateSitDown()
        {
            Task.Condition[] finishConditions =
            {
                () => !_animationManager.IsAnimatingAction
            };

            SimpleTask.TaskAction action = SitOnChair;

            var task = new SimpleTask(
                this.name + "SitDown",
                CoroutineStarter,
                action,
                Task.EmptyCondition,
                Task.EmptyCondition,
                finishConditions);
            return task;
        }
        
        protected SimpleTask CreateStandUpFromChair()
        {
            Task.Condition[] finishConditions =
            {
                () => !_animationManager.IsAnimatingAction
            };

            SimpleTask.TaskAction action = StandUpFromChair;

            var task = new SimpleTask(
                this.name + "StandUp",
                CoroutineStarter,
                action,
                Task.EmptyCondition,
                Task.EmptyCondition,
                finishConditions);
            return task;
        }
        
        protected SimpleTask CreateGoToWorkingPlace()
        {
            Task.Condition[] finishCondition =
            {
                () => HasToStop ||
                      HtnPlanner.AllMustChangeActivity ||
                      IsAgentInPosition(workingPlace.navMeshDestination.position)
            };

            SimpleTask.TaskAction action = () =>
            {
                _navMeshAgent.SetDestination(workingPlace.navMeshDestination.position);
            };

            var task = new SimpleTask(
                this.name + "GoToWorkingPlace",
                CoroutineStarter,
                action,
                Task.EmptyCondition,
                Task.EmptyCondition,
                finishCondition);
            return task;
        }
        
        protected SimpleTask CreateGoToArchives()
        {

            Task.Condition[] finishCondition =
            {
                () => HasToStop ||
                      HtnPlanner.AllMustChangeActivity ||
                      (IsAgentInPosition(_archivePosition.position))
            };

            SimpleTask.TaskAction action = GoToArchives;

            var task = new SimpleTask(
                this.name + "GoToArchives",
                CoroutineStarter,
                action,
                Task.EmptyCondition,
                Task.EmptyCondition,
                finishCondition);
            return task;
        }
        
        protected SimpleTask CreateAlignWithWorkingPlaceChair()
        {
            SimpleTask.TaskAction action = () =>
            {
                AlignWithVector(workingPlace.navMeshDestination.forward, workingPlace.navMeshDestination.position);
            };

            var task = new SimpleTask(
                this.name + "AlignWithWorkingPlaceChair",
                CoroutineStarter,
                action,
                Task.EmptyCondition,
                Task.EmptyCondition,
                Task.EmptyCondition);
            return task;
        }
        
        protected SimpleTask CreateAlignWithArchives()
        {
            SimpleTask.TaskAction action = () =>
            {
                AlignWithVector(_archivePosition.forward, _archivePosition.position);
            };

            var task = new SimpleTask(
                this.name + "AlignWithArchives",
                CoroutineStarter,
                action,
                Task.EmptyCondition,
                Task.EmptyCondition,
                Task.EmptyCondition);
            return task;
        }
        
        protected SimpleTask CreateStartSearchingArchives()
        {
            Task.Condition[] finishConditions = {() => !IsSearchingArchives};

            SimpleTask.TaskAction action = StartSearchingArchives;

            var task = new SimpleTask(
                this.name + "StartSearchingArchives",
                CoroutineStarter,
                action,
                Task.EmptyCondition,
                Task.EmptyCondition,
                finishConditions);
            return task;
        }

        protected SimpleTask CreateTurnOn()
        {
            Task.Condition[] preCondition =
            {
                () => IsSitting,
                () => workingPlace.IsChairFacingComputer()
            };
            
            SimpleTask.TaskAction action = TurnOnComputer;
            
            var task = new SimpleTask(
                this.name + "TurnOn",
                CoroutineStarter,
                action,
                preCondition,
                Task.EmptyCondition,
                Task.EmptyCondition);
            return task;
        }

        protected SimpleTask CreateTurnOff()
        {
            Task.Condition[] preCondition =
            {
                () => IsSitting,
                () => workingPlace.IsChairFacingComputer()
            };
            
            SimpleTask.TaskAction action = workingPlace.computer.TurnOff;
            
            var task = new SimpleTask(
                this.name + "TurnOff",
                CoroutineStarter,
                action,
                preCondition,
                Task.EmptyCondition,
                Task.EmptyCondition);
            return task;
        }
        
        protected SimpleTask CreateStartTyping()
        {
            Task.Condition[] preCondition =
            {
                () => IsSitting,
                () => workingPlace.IsChairFacingComputer()
            };

            SimpleTask.TaskAction action = StartTyping;

            var task = new SimpleTask(
                this.name + "StartTyping",
                CoroutineStarter,
                action,
                preCondition,
                Task.EmptyCondition,
                Task.EmptyCondition);
            return task;
        }
        
        protected SimpleTask CreateStartTypingPassword()
        {
            Task.Condition[] preCondition =
            {
                () => IsSitting,
            };

            SimpleTask.TaskAction action = StartTypingPassword;

            var task = new SimpleTask(
                this.name + "StartTypingPassword",
                CoroutineStarter,
                action,
                preCondition,
                Task.EmptyCondition,
                Task.EmptyCondition);
            return task;
        }
        
        protected SimpleTask CreateSubmitPassword()
        {
            Task.Condition[] preCondition =
            {
                () => IsSitting,
            };

            SimpleTask.TaskAction action = SubmitPassword;

            var task = new SimpleTask(
                this.name + "SubmitPassword",
                CoroutineStarter,
                action,
                preCondition,
                Task.EmptyCondition,
                Task.EmptyCondition);
            return task;
        }
        
        protected SimpleTask CreateTypingPassword()
        {
            Task.Condition[] preCondition =
            {
                () => IsSitting,
            };

            Task.Condition[] integrityRules = {() => _handler.NoFlagsRaised()};
            
            Task.Condition[] finishConditions = {() => _waitTimer <= 0};

            SimpleTask.TaskAction action = () =>
            {
                if (_waitTimer > 0) _waitTimer += _personality.passwordTypingTime;
                else _waitTimer = _personality.passwordTypingTime;
            };

            var task = new SimpleTask(
                this.name + "TypingPassword",
                CoroutineStarter,
                action,
                preCondition,
                integrityRules,
                finishConditions);
            return task;
        }
        
        protected SimpleTask CreateLogOut()
        {
            Task.Condition[] preCondition =
            {
                () => IsSitting,
            };

            SimpleTask.TaskAction action = LogOut;

            var task = new SimpleTask(
                this.name + "LogOut",
                CoroutineStarter,
                action,
                preCondition,
                Task.EmptyCondition,
                Task.EmptyCondition);
            return task;
        }
        
        protected SimpleTask CreateStopTyping()
        {
            Task.Condition[] preCondition =
            {
                () => IsSitting,
                () => IsTyping,
            };

            SimpleTask.TaskAction action = StopTyping;

            var task = new SimpleTask(
                this.name + "StopTyping",
                CoroutineStarter,
                action,
                preCondition,
                Task.EmptyCondition,
                Task.EmptyCondition);
            return task;
        }
        
        protected SimpleTask CreateTurnToComputer()
        {
            Task.Condition[] preCondition =
            {
                () => IsSitting,
            };

            SimpleTask.TaskAction action = () => workingPlace.TurnToComputer();
            Task.Condition[] finishCondition =
            {
                workingPlace.chair.IsRotationComplete,
            };

            var task = new SimpleTask(
                this.name + "TurnToComputer",
                CoroutineStarter,
                action,
                preCondition,
                Task.EmptyCondition,
                finishCondition);
            return task;
        }
        
        protected SimpleTask CreateTurnAwayFromComputer()
        {
            SimpleTask.TaskAction action = () => workingPlace.chair.ResetSeat();

            Task.Condition[] finishCondition =
            {
                workingPlace.chair.IsRotationComplete,
            };

            var task = new SimpleTask(
                this.name + "TurnAwayFromComputer",
                CoroutineStarter,
                action,
                Task.EmptyCondition,
                Task.EmptyCondition,
                finishCondition);
            return task;
        }
        
        protected ComplexTask CreateSearchArchives()
        {
            Task.Condition[] integrityRules =
            {
                () => !HasToStop,
                () => !HtnPlanner.AllMustChangeActivity,
            };

            ComplexTask.DecompositionMethod method = DecomposeSearchArchives;

            var task = new ComplexTask(
                this.name + "SearchArchives",
                CoroutineStarter,
                method,
                Task.EmptyCondition,
                integrityRules);
            return task;
        }
        
        protected ComplexTask CreateLogIn()
        {
            Task.Condition[] conditions = Task.EmptyCondition;
            Task.Condition[] integrityRules = Task.EmptyCondition;

            ComplexTask.DecompositionMethod method = DecomposeLogIn;

            var task = new ComplexTask(
                this.name + "TypePassword",
                CoroutineStarter,
                method,
                conditions,
                integrityRules);
            return task;
        }
        
        protected ComplexTask CreateWorkAtComputer()
        {
            Task.Condition[] integrityRules =
            {
                () => !HasToStop,
                () => !HtnPlanner.AllMustChangeActivity,
            };

            ComplexTask.DecompositionMethod method = DecomposeWorkAtComputer;

            var task = new ComplexTask(
                this.name + "WorkAtComputer",
                CoroutineStarter,
                method,
                Task.EmptyCondition,
                integrityRules);
            return task;
        }
        
        protected ComplexTask CreateStopActivities()
        {

            ComplexTask.DecompositionMethod method = DecomposeStopActivities;
            var task = new ComplexTask(
                this.name + "StopActivities",
                CoroutineStarter,
                method,
                Task.EmptyCondition);
            return task;
        }
        
        protected ComplexTask CreateWork()
        {
            Task.Condition[] integrityRules =
            {
                () => !HasToStop,
                () => !HtnPlanner.AllMustChangeActivity,
            };

            ComplexTask.DecompositionMethod method = DecomposeWork;

            var task = new ComplexTask(
                this.name + "Work",
                CoroutineStarter,
                method,
                Task.EmptyCondition,
                integrityRules);
            return task;
        }

        protected ComplexTask CreateTalkThenWork()
        {
            Task.Condition[] integrityRules =
            {
                () => !HasToStop,
                () => !HtnPlanner.AllMustChangeActivity,
            };

            ComplexTask.DecompositionMethod method = DecomposeWork;

            var task = new ComplexTask(
                this.name + "Work",
                CoroutineStarter,
                method,
                Task.EmptyCondition,
                integrityRules);
            return task;
        }
        
        protected ComplexTask CreateSendPlayerAwayThenWork()
        {
            Task.Condition[] integrityRules =
            {
                () => !HasToStop,
                () => !HtnPlanner.AllMustChangeActivity,
            };

            ComplexTask.DecompositionMethod method = DecomposeWork;

            var task = new ComplexTask(
                this.name + "Work",
                CoroutineStarter,
                method,
                Task.EmptyCondition,
                integrityRules);
            return task;
        }
        
        protected SimpleTask CreateEmptyTask()
        {
            SimpleTask.TaskAction action = () => { };

            var task = new SimpleTask(
                this.name + "Empty",
                CoroutineStarter,
                action,
                Task.EmptyCondition,
                Task.EmptyCondition,
                Task.EmptyCondition);
            return task;

        }

        #endregion
    }
}