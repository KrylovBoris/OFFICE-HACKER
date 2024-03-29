﻿// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/Hierarchical-Task-Network-planner-for-Unity/blob/master/LICENSE

using System;
using System.Collections.Generic;
using GlobalMechanics;
using JetBrains.Lifetimes;
using NPC;
using UnityEngine;
using BaseAgent = NPC.BaseAgent;

namespace HierarchicalTaskNetwork
{
    public class HTN_planner : MonoBehaviour
    {
        private LifetimeDefinition _lifetime;
    
        public BaseAgent[] agents;
        public GameObject player;

        private Timetable _timetable;
        private string _designatedTask;
        private bool _agentsSpawned = false;
        private Dictionary<uint, BaseAgent> _agentIdDictionary = new Dictionary<uint, BaseAgent>();
        private Dictionary<uint, HtnTask> _idToTask;

        private Dictionary<(string task, InterruptionFlag flag), string> _taskAndFlagToHandlingTask;
    
    
        private bool _globalFlagChangeActivity = false;
        private string _currentActivity;
        public bool AllMustChangeActivity => _globalFlagChangeActivity;
        public enum AgentType
        {
            Base,
        }
    
    
        // Start is called before the first frame update
        private void OnEnable()
        {
            _lifetime = Lifetime.Eternal.CreateNested();
        }

        private void OnDisable()
        {
            _lifetime.Terminate();
        }


        void Start()
        {
            _timetable = GetComponent<Timetable>();
            _designatedTask = _timetable.GetCurrentEvent();
            //navSystem = GetComponent<NavigationSystem>();
            _agentIdDictionary = new Dictionary<uint, BaseAgent>();
            _idToTask = new Dictionary<uint, HtnTask>();
             
            _taskAndFlagToHandlingTask = new Dictionary<(string task, InterruptionFlag flag), string>();
            _taskAndFlagToHandlingTask.Add(("Work", InterruptionFlag.Talk), "TalkThenWork");
            _taskAndFlagToHandlingTask.Add(("Work", InterruptionFlag.PlayerPeeking), "SendPlayerAwayThenWork");
            //_taskAndFlagToHandlingTask.Add(("Work", InterruptionFlag.WrongPassword), "TalkThenWork");
            _taskAndFlagToHandlingTask.Add(("Work", InterruptionFlag.PlayerTookWorkplace), "SendPlayerAwayThenWork");
            _taskAndFlagToHandlingTask.Add(("Work", InterruptionFlag.PlayerTurnedOffTheComputer), "SendPlayerAwayThenWork");
            _taskAndFlagToHandlingTask.Add(("Lunch", InterruptionFlag.Talk), "TalkThenLunch");
            _taskAndFlagToHandlingTask.Add(("Lunch", InterruptionFlag.PlayerTookWorkplace), "SendPlayerAwayThenLunch");
            _taskAndFlagToHandlingTask.Add(("Break", InterruptionFlag.Talk), "TalkThenBreak");
            _taskAndFlagToHandlingTask.Add(("Break", InterruptionFlag.PlayerPeeking), "SendPlayerAwayThenBreak");
            _taskAndFlagToHandlingTask.Add(("Break", InterruptionFlag.PlayerTookWorkplace), "SendPlayerAwayThenBreak");
        
            foreach (var a in agents)
            {
                if (a.isActiveAndEnabled)
                {
            
                    a.RegistryHtnPlanner(this);
                    _agentsSpawned = true;
                }
            }
            SetAgents();
        }

        public void SpawnAgents()
        {
            try
            {
                foreach (var a in agents)
                {
                    if(!a.isActiveAndEnabled)
                        a.gameObject.SetActive(true);
                }
                SetAgents();
                _agentsSpawned = true;
            }
            catch (IndexOutOfRangeException exception)
            {
                Debug.LogError("Agents are not set up.");
                throw;
            }

        }
    
        private void SetAgents()
        {
            for(int i = 0; i < agents.Length; i++)
            {
                _agentIdDictionary.Add(agents[i].ID, agents[i]);
                AssignTask(agents[i], agents[i].GetTaskByName(_designatedTask));
            }
        }
        // Update is called once per frame
        void Update()
        {
            if (_timetable.GetCurrentEvent() != _designatedTask)
            {
                _globalFlagChangeActivity = true;
                _designatedTask = _timetable.GetCurrentEvent();
                for(int i = 0; i < agents.Length; i++)
                {
                    AssignTask(agents[i], agents[i].GetTaskByName(_designatedTask));
                }

                _globalFlagChangeActivity = false;
            }

            foreach (var t in _idToTask.Values)
            {
                if (t.Status != HtnTask.TaskStatus.InProgress )
                    t.StartExecution(_lifetime.Lifetime);
                //Debug.Log(t.Name + t.Status);
            }

            foreach (var a in agents)
            {
                if (!a.Handler.NoFlagsRaised(out var flag))
                {
                    AssignTask(a, a.GetTaskByName(_taskAndFlagToHandlingTask[(_designatedTask, flag)]));
                    a.Handler.ResetFlags();
                }
            }
        }

        public void AssignTask(BaseAgent a, HtnTask htnTask)
        {
            if (_idToTask.ContainsKey(a.ID))
            {
                _idToTask[a.ID] = htnTask;
            }
            else
            {
                _idToTask.Add(a.ID, htnTask);
            }
        }
    
        //Suspicious Behaviour found
        /*
    public void Notify(GameObject targetGO)
    {
        if (player != null)
        {
            if (player.Equals(targetGO))
            {
                foreach (Agent agent in agents)
                {
                    if (agent != null) agent.Alert(targetGO);
                }
            }
        }
    }
    */
    

    
        public void AgentDied(uint agent)
        {
            _agentIdDictionary.Remove(agent);
            _idToTask.Remove(agent);
            Debug.Log("Agent" + agent + " has died and removed.");
        }
       

    }
}
