﻿using System;
using System.Collections;
using System.Collections.Generic;
using GlobalMechanics;
using HierarchicalTaskNetwork;
using JetBrains.Collections.Viewable;
using NPC;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Agent
{
    public partial class BaseAgent
    {
        #region Methods
	        public void GoToPC()
            {
                _navMeshDestination = workingPlace.navMeshDestination;
                _navMeshAgent.SetDestination(GetDestination());
            }
            
            public void GoToArchives()
            {
                _navMeshDestination = _activeArchiveZone.WaitingZone;
                _navMeshAgent.stoppingDistance = _activeArchiveZone.WaitingZoneRadius;
                _navMeshAgent.SetDestination(GetDestination());
            }
	        public void TurnOnPC() => workingPlace.computer.TurnOn();
            public void TurnOffPC() => workingPlace.computer.TurnOff();
            public void LogIn()
            {
                LogOut();
                StartTypingPassword();
                StartCoroutine(CoroutineUtils.DelayedAction(_personality.passwordTypingTime, 
                        SubmitPassword));
            }
        
            
            private void LogOut() => workingPlace.LogOut();
            
	        public void StartTyping()
            {
                _isTyping = true;
                _animationManager.StartTyping();;
            }
	        public void StopTyping()
            {
                _isTyping = false;
                _animationManager.StopTyping();
            }
	        public void AlignWithDestination()
            {
                var agentTransform = transform;
                agentTransform.position = _navMeshDestination.position;
                agentTransform.forward = _navMeshDestination.forward;
            }
	        public void SitDown()
            {
                _animationManager.SitDownOnChair();
                _isSitting = true;
                _navMeshAgent.enabled = false;
                transform.parent = workingPlace.chair.transform;
                workingPlace.CharacterSeated(_personality);
            }
	        public void TurnToComputer() => workingPlace.TurnToComputer();
            public void TurnAwayFromPC()
            {
                workingPlace.chair.ResetSeat();
            }
            private void StartTypingPassword()
            {
                workingPlace.StartTypingPassword(_personality.passwordTypingTime);
            }
            private void SubmitPassword()
            {
                workingPlace.LogIn(_personality.LogInId, _personality.KnownPassword, out var flag);
                if (!flag) _handler.RaiseFlag(InterruptionFlag.WrongPassword);
                workingPlace.StopTypingPassword();
            }
            public void WaitSomeTime()
            {
                HasToWaitTimer = true;
                StartCoroutine(CoroutineUtils.DelayedAction(
                    UnityEngine.Random.Range(minWaitTime, maxWaitTime),
                    () => { HasToWaitTimer = false;}));
            }
	        public void CheckMail()
            {
                workingPlace.computer.CheckAllMail(_personality);
            }
	        public void StandUp()
            {
                _isSitting = false;
                _navMeshAgent.enabled = true;
                transform.parent = null;
                _animationManager.StandUpFromChair();
            }
	        public void RequestArchiveToken()
            {
                _activeArchiveZone = department.GetRandomArchiveNavMeshDestination();
                _activeToken = _activeArchiveZone.RequestToken();
                _activeToken.Accept();
                if (_activeToken is RequestToken requestToken)
                {
                    requestToken.RequestedTokenSignal.AdviseOnce(GameManager.gm.ProjectLifetime, token =>
                    {
                        _activeToken = token;
                        token.Accept();
                        _navMeshDestination = ((ArchiveSearchToken) _activeToken).SearchingSpot;
                    });
                }
                else
                {
                    _navMeshDestination = ((ArchiveSearchToken) _activeToken).SearchingSpot;
                }
            }

            public void SearchArchives()
            {
                _navMeshAgent.enabled = false;
                _animationManager.StartSearchingFillingCabinet(((ArchiveSearchToken)_activeToken).Method);
            }
	        public void Emote()
            {
                _animationManager.EmoteAny();
            }
	        public void InitTalk()
            {
                _animationManager.LookAt(_currentConversation.GetInterlocutor(this).Head);
            }
	        public void WaitPlayerResponse()
            {
                throw new NotImplementedException();
            }
	        public void RequestFaxToken()
            {
                _activeFaxZone = department.GetRandomFaxMachineDestination();
                _activeToken = _activeFaxZone.RequestToken();
                _activeToken.Accept();
                if (_activeToken is RequestToken requestToken)
                {
                    requestToken.RequestedTokenSignal.AdviseOnce(GameManager.gm.ProjectLifetime, token =>
                    {
                        _activeToken = token;
                        token.Accept();
                        _navMeshDestination = ((FaxMachineToken) _activeToken).SearchingSpot;
                    });
                }
                else
                {
                    _navMeshDestination = ((FaxMachineToken) _activeToken).SearchingSpot;
                }
            }
	        public void GoToPrinter()
            {
                _navMeshDestination = _activeFaxZone.WaitingZone;
                _navMeshAgent.stoppingDistance = _activeFaxZone.WaitingZoneRadius;
                _navMeshAgent.SetDestination(GetDestination());
            }
            
	        public void AdjustPosition()
            {
                if (_activeToken is ArchiveSearchToken archiveToken)
                {
                    _navMeshDestination = archiveToken.SearchingSpot;
                }
                if (_activeToken is FaxMachineToken faxToken)
                {
                    _navMeshDestination = faxToken.SearchingSpot;
                }
                _navMeshAgent.stoppingDistance = standardStoppingDistance;
                _navMeshAgent.SetDestination(GetDestination());
                StartCoroutine(CoroutineUtils.ConditionedAction(IsNearDestination, AlignWithDestination));
            }
	        public void UseFax()
            {
                _navMeshAgent.enabled = false;
                _animationManager.StartSendingFax();
            }
	        public void Abort()
            {
                throw new NotImplementedException();
            }
	        public void Reject()
            {
                throw new NotImplementedException();
            }
	        public void Approve()
            {
                throw new NotImplementedException();
            }
	        public void WaitNPCResponse()
            {
                _currentConversation.RequestLine();
            }
	        public void TalkToNPC()
            {
                _animationManager.SayLine();
            }
	        public void WaitForInterlocutor() {}
	        public void WalkToKitchen()
            {
                throw new NotImplementedException();
            }
	        public void WalkToFriend()
            {
                throw new NotImplementedException();
            }
	        public void WalkToSofa()
            {
                throw new NotImplementedException();
            }
	        public void WalkToBreakRoom()
            {
                throw new NotImplementedException();
            }
	        public void WalkToUSB()
            {
                throw new NotImplementedException();
            }
	        public void PickUpUSB()
            {
                throw new NotImplementedException();
            }
	        public void InstallUsbInComputer()
            {
                throw new NotImplementedException();
            }

            public bool IsSitting() => _isSitting;
            public bool IsTyping() => _isTyping;
            public bool IsSearchingArchives() => _activeToken is ArchiveSearchToken && _activeToken.Status == TokenStatus.InProgress;
            public bool IsDoingSomething() => _isTyping || _isTalking || IsSearchingArchives();
            
	        public bool IsFacingComputer()
            {
                return workingPlace.IsChairFacingComputer();
            }
	        public bool IsNearDestination()
            {
                return IsAgentInPosition(_navMeshAgent.destination);
            }
	        public bool IsWorking()
            {
                var res = true;
                throw new NotImplementedException();
                return res;
            }
	        public bool WillWorkOnPC()
            {
                var res = Random.Range(0, 100) < 75;
                //TODO: Personality-based choice
                return res;
            }
	        public bool CanAnimate() => !_animationManager.IsAnimatingAction;

            public bool StopFlag()
            {
                if (_hasToStop)
                {
                    _hasToStop = false;
                    return true;
                }

                return false;
            }
	        public bool IsComputerOn() => workingPlace.computer.IsOn;

            public bool IsLoggedIn() => workingPlace.computer.IsLoggedIn(out var id) && id == _personality.LogInId;
            public bool ChairRotationComplete()
            {
                return workingPlace.chair.IsRotationComplete();
            }
	        public bool HasToWaitTimer { get; private set; }

            public bool HasRequestedToken() =>
                !(_activeToken is RequestToken) && _activeToken.Status == TokenStatus.InProgress;
            
            public bool WillChatNPC()
            {
                var res = true;
                throw new NotImplementedException();
                return res;
            }
	        public bool WillEmote()
            {
                var res = true;
                throw new NotImplementedException();
                return res;
            }
	        public bool WillSearchArchives()
            {
                var res = Random.Range(0, 100) < 50;
                //TODO: Personality-based choice
                return res;
            }
            
            public bool IsTalking() => _isTalking;
            
            public bool HasPlayerResponse()
            {
                var res = true;
                throw new NotImplementedException();
                return res;
            }
	        public bool WillTalk()
            {
                var res = true;
                throw new NotImplementedException();
                return res;
            }
	        public bool WillApprove()
            {
                var res = true;
                throw new NotImplementedException();
                return res;
            }
	        public bool HasToStopChat()
            {
                var res = HasRequestedToken();
                return res;
            }
	        public bool InterlocutorFound()
            {
                var res = true;
                throw new NotImplementedException();
                return res;
            }
	        public bool CanChat()
            {
                var res = true;
                throw new NotImplementedException();
                return res;
            }
	        public bool WillGoToBreakRoom()
            {
                var res = true;
                throw new NotImplementedException();
                return res;
            }
	        public bool WillSpeakWithFriend()
            {
                var res = true;
                throw new NotImplementedException();
                return res;
            }
	        public bool WillGoToKitchen()
            {
                var res = true;
                throw new NotImplementedException();
                return res;
            }

            public bool CanSit()
            {
                var res = true;
                throw new NotImplementedException();
                return res;
            }
	        public bool HasUSB()
            {
                var res = true;
                //TODO USB pickup check;
                return res;
            }
	        public bool WillPlugUsb()
            {
                return new RandomEvent(_personality.UsbAttackProbability).HasEventHappened();
            }
            
            private bool HasNpcResponse()
            {
                return _currentConversation.HasRequestedLine(this);
            }
            
        #endregion
        
        #region SimpleTasks
        [HtnRootTask("Empty")]
        protected SimpleTask CreateEmptyTask()
        {
            SimpleTask.TaskAction action = () => { };

            var task = new SimpleTask(
                this.name + "Empty",
                action,
                HtnTask.EmptyCondition,
                HtnTask.EmptyCondition,
                HtnTask.EmptyCondition);
            return task;

        }

        public SimpleTask CreateGoToPC()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => IsNearDestination()
            };

            SimpleTask.TaskAction action = GoToPC;

            var task = new SimpleTask(
                name + "GoToPC",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateGoToArchiveArea()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => IsNearDestination()
            };

            SimpleTask.TaskAction action = GoToArchives;

            var task = new SimpleTask(
                name + "GoToArchiveArea",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateAlignWithDestination()
        {
            HtnTask.Condition[] preConditions =
            {
                IsNearDestination
            };

            var integrityRules = HtnTask.EmptyCondition;

            var finishConditions = HtnTask.EmptyCondition;

            SimpleTask.TaskAction action = AlignWithDestination;

            var task = new SimpleTask(
                name + "AlignWithDestination",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateSitDown()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => CanAnimate()
            };

            SimpleTask.TaskAction action = SitDown;

            var task = new SimpleTask(
                name + "SitDown",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateTurnOnPC()
        {
            HtnTask.Condition[] preConditions =
            {
                () => IsSitting(),
                () => IsFacingComputer(),
            };

            var integrityRules = HtnTask.EmptyCondition;

            var finishConditions = HtnTask.EmptyCondition;

            SimpleTask.TaskAction action = TurnOnPC;

            var task = new SimpleTask(
                name + "TurnOnPC",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateLogIn()
        {
            HtnTask.Condition[] preConditions =
            {
                () => IsComputerOn()
            };

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => IsLoggedIn()
            };

            SimpleTask.TaskAction action = LogIn;

            var task = new SimpleTask(
                name + "LogIn",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateTurnToComputer()
        {
            HtnTask.Condition[] preConditions =
            {
                () => IsSitting()
            };

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => ChairRotationComplete()
            };

            SimpleTask.TaskAction action = TurnToComputer;

            var task = new SimpleTask(
                name + "TurnToComputer",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateTurnFromComputer()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => ChairRotationComplete()
            };

            SimpleTask.TaskAction action = TurnAwayFromPC;

            var task = new SimpleTask(
                name + "TurnFromComputer",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateTurnOffPC()
        {
            HtnTask.Condition[] preConditions = 
            {
                () => IsSitting(),
                () => IsFacingComputer()
            };

            var integrityRules = HtnTask.EmptyCondition;

            var finishConditions = HtnTask.EmptyCondition;

            SimpleTask.TaskAction action = TurnOffPC;

            var task = new SimpleTask(
                name + "TurnOffPC",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateStartTyping()
        {
            HtnTask.Condition[] preConditions =
            {
                () => IsSitting(),
                () => IsFacingComputer()
            };

            var integrityRules = HtnTask.EmptyCondition;

            var finishConditions = HtnTask.EmptyCondition;

            SimpleTask.TaskAction action = StartTyping;

            var task = new SimpleTask(
                name + "StartTyping",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateStopTyping()
        {
            HtnTask.Condition[] preConditions =
            {
                () => IsSitting(),
                () => IsTyping()
            };

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => CanAnimate()
            };

            SimpleTask.TaskAction action = StopTyping;

            var task = new SimpleTask(
                name + "StopTyping",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateLogOut()
        {
            HtnTask.Condition[] preConditions =
            {
                () => IsLoggedIn()
            };

            var integrityRules = HtnTask.EmptyCondition;

            var finishConditions = HtnTask.EmptyCondition;

            SimpleTask.TaskAction action = LogOut;

            var task = new SimpleTask(
                name + "LogOut",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateWaitSomeTime()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => !HasToWaitTimer
            };

            SimpleTask.TaskAction action = WaitSomeTime;

            var task = new SimpleTask(
                name + "WaitSomeTime",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateCheckMail()
        {
            HtnTask.Condition[] preConditions =
            {
                () => IsLoggedIn(),
                () => IsComputerOn()
            };

            var integrityRules = HtnTask.EmptyCondition;

            var finishConditions = HtnTask.EmptyCondition;

            SimpleTask.TaskAction action = CheckMail;

            var task = new SimpleTask(
                name + "CheckMail",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateStandUp()
        {
            HtnTask.Condition[] preConditions =
            {
                () => IsSitting()
            };

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => CanAnimate()
            };

            SimpleTask.TaskAction action = StandUp;

            var task = new SimpleTask(
                name + "StandUp",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateRequestArchiveToken()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            var finishConditions = HtnTask.EmptyCondition;

            SimpleTask.TaskAction action = RequestArchiveToken;

            var task = new SimpleTask(
                name + "RequestArchiveToken",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateSearchArchives()
        {
            HtnTask.Condition[] preConditions =
            {
                IsNearDestination
            };

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                CanAnimate
            };

            SimpleTask.TaskAction action = SearchArchives;

            var task = new SimpleTask(
                name + "SearchArchives",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateInitiateDialogueRoutine()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            var finishConditions = HtnTask.EmptyCondition;

            SimpleTask.TaskAction action = InitTalk;

            var task = new SimpleTask(
                name + "InitiateDialogueRoutine",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateWaitPlayerLine()
        {
            HtnTask.Condition[] preConditions =
            {
                IsTalking
            };

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                HasPlayerResponse
            };

            SimpleTask.TaskAction action = WaitPlayerResponse;

            var task = new SimpleTask(
                name + "WaitPlayerLine",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateEmote()
        {
            HtnTask.Condition[] preConditions =
            {
                () => CanAnimate()
            };

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => CanAnimate()
            };

            SimpleTask.TaskAction action = Emote;

            var task = new SimpleTask(
                name + "Emote",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateRequestFaxToken()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            var finishConditions = HtnTask.EmptyCondition;

            SimpleTask.TaskAction action = RequestFaxToken;

            var task = new SimpleTask(
                name + "RequestFaxToken",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateGoToPrinterArea()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => IsNearDestination()
            };

            SimpleTask.TaskAction action = GoToPrinter;

            var task = new SimpleTask(
                name + "GoToPrinterArea",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateAdjustPosition()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                IsNearDestination
            };

            SimpleTask.TaskAction action = AdjustPosition;

            var task = new SimpleTask(
                name + "AdjustPosition",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateUseFax()
        {
            HtnTask.Condition[] preConditions =
            {
                () => IsNearDestination()
            };

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => CanAnimate()
            };

            SimpleTask.TaskAction action = UseFax;

            var task = new SimpleTask(
                name + "UseFax",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateStopConversation()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            var finishConditions = HtnTask.EmptyCondition;

            SimpleTask.TaskAction action = Abort;

            var task = new SimpleTask(
                name + "StopConversation",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreatePositiveResponse()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            var finishConditions = HtnTask.EmptyCondition;

            SimpleTask.TaskAction action = Approve;

            var task = new SimpleTask(
                name + "PositiveResponse",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateNegativeResponse()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            var finishConditions = HtnTask.EmptyCondition;

            SimpleTask.TaskAction action = Reject;

            var task = new SimpleTask(
                name + "NegativeResponse",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateWaitNPCLine()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => HasNpcResponse()
            };

            SimpleTask.TaskAction action = WaitNPCResponse;

            var task = new SimpleTask(
                name + "WaitNPCLine",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateNPCLine()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            var finishConditions = HtnTask.EmptyCondition;

            SimpleTask.TaskAction action = TalkToNPC;

            var task = new SimpleTask(
                name + "NPCLine",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateWaitForInterlocutor()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => InterlocutorFound() || HasToStopChat()
            };

            SimpleTask.TaskAction action = WaitForInterlocutor;

            var task = new SimpleTask(
                name + "WaitForInterlocutor",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateWalkToKitchen()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => IsNearDestination()
            };

            SimpleTask.TaskAction action = WalkToKitchen;

            var task = new SimpleTask(
                name + "WalkToKitchen",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateWalkToFriend()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => IsNearDestination()
            };

            SimpleTask.TaskAction action = WalkToFriend;

            var task = new SimpleTask(
                name + "WalkToFriend",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateWalkToBreakRoom()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => IsNearDestination()
            };

            SimpleTask.TaskAction action = WalkToBreakRoom;

            var task = new SimpleTask(
                name + "WalkToBreakRoom",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateWalkToSofa()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => IsNearDestination()
            };

            SimpleTask.TaskAction action = WalkToSofa;

            var task = new SimpleTask(
                name + "WalkToSofa",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreateWalkToUSB()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => IsNearDestination()
            };

            SimpleTask.TaskAction action = WalkToUSB;

            var task = new SimpleTask(
                name + "WalkToUSB",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreatePickUpUSB()
        {
            var preConditions = HtnTask.EmptyCondition;

            var integrityRules = HtnTask.EmptyCondition;

            HtnTask.Condition[] finishConditions =
            {
                () => HasUSB()
            };

            SimpleTask.TaskAction action = PickUpUSB;

            var task = new SimpleTask(
                name + "PickUpUSB",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }

        public SimpleTask CreatePlugInUsb()
        {
            HtnTask.Condition[] preConditions =
            {
                () => HasUSB()
            };

            var integrityRules = HtnTask.EmptyCondition;

            var finishConditions = HtnTask.EmptyCondition;

            SimpleTask.TaskAction action = InstallUsbInComputer;

            var task = new SimpleTask(
                name + "PlugInUsb",
                action,
                preConditions,
                integrityRules,
                finishConditions);
            return task;
        }
        
        #endregion
        
        #region ComplexTasks
        
        [HtnRootTask("Work")]
        public ComplexTask CreateWork()
        {
            HtnTask.Condition[] preConditions = HtnTask.EmptyCondition;
        
            HtnTask.Condition[] integrityRules = HtnTask.EmptyCondition;
            
            ComplexTask.DecompositionMethod method = DecomposeWork;
          
            var task = new ComplexTask (
                this.name + "Work",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeWork()
        {
            var tasks = new List<HtnTask>();
            if(WillWorkOnPC()) {
                tasks.Add(CreateWorkOnPC());
            }
            else {
                if(WillSearchArchives()) {
                    tasks.Add(CreateRequestArchiveToken());
                    tasks.Add(CreateSearchArchivesRoutine());
                }
                else {
                    tasks.Add(CreateRequestFaxToken());
                    tasks.Add(CreateFaxRoutine());
                }
            }
            tasks.Add(CreateFinishAllActivities());
            return tasks.ToArray();
        }

	    public ComplexTask CreateWorkOnPC()
        {
            HtnTask.Condition[] preConditions = HtnTask.EmptyCondition;
        
            HtnTask.Condition[] integrityRules = 
            {
                () => !StopFlag(),
            };
              
            ComplexTask.DecompositionMethod method = DecomposeWorkOnPC;
          
            var task = new ComplexTask (
                this.name + "WorkOnPC",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeWorkOnPC()
        {
            var tasks = new List<HtnTask>();
            tasks.Add(CreateGoToPC());
            if(IsSitting()) {
            }
            else {
                tasks.Add(CreateSit());
            }
            if(IsComputerOn()) {
            }
            else {
                tasks.Add(CreateTurnOnPC());
            }
            tasks.Add(CreateWorkAtPC());
            return tasks.ToArray();
        }

	    public ComplexTask CreateSearchArchivesRoutine()
        {
            HtnTask.Condition[] preConditions = HtnTask.EmptyCondition;
        
            HtnTask.Condition[] integrityRules = 
            {
                () => !StopFlag(),
            };
              
            ComplexTask.DecompositionMethod method = DecomposeSearchArchivesRoutine;
          
            var task = new ComplexTask (
                this.name + "SearchArchivesRoutine",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeSearchArchivesRoutine()
        {
            var tasks = new List<HtnTask>();
            tasks.Add(CreateGoToArchiveArea());
            if(HasRequestedToken()) {
            }
            else {
                tasks.Add(CreateWaitAtLocation());
            }
            tasks.Add(CreateAdjustPosition());
            tasks.Add(CreateSearchArchives());
            return tasks.ToArray();
        }

	    public ComplexTask CreateFinishAllActivities()
        {
            HtnTask.Condition[] preConditions = HtnTask.EmptyCondition;
        
            HtnTask.Condition[] integrityRules = HtnTask.EmptyCondition;
            
            ComplexTask.DecompositionMethod method = DecomposeFinishAllActivities;
          
            var task = new ComplexTask (
                this.name + "FinishAllActivities",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeFinishAllActivities()
        {
            var tasks = new List<HtnTask>();
            if(IsLoggedIn()) {
                tasks.Add(CreateLogOut());
            }
            if(IsTyping()) {
                tasks.Add(CreateStopTyping());
            }
            if(IsComputerOn()) {
                tasks.Add(CreateTurnOffPC());
            }
            if(IsSitting()) {
                tasks.Add(CreateStand());
            }
            return tasks.ToArray();
        }

	    public ComplexTask CreateSit()
        {
            HtnTask.Condition[] preConditions = 
            { 
                () => IsNearDestination(),  
            };
        
            HtnTask.Condition[] integrityRules = HtnTask.EmptyCondition;
            
            ComplexTask.DecompositionMethod method = DecomposeSit;
          
            var task = new ComplexTask (
                this.name + "Sit",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeSit()
        {
            var tasks = new List<HtnTask>();
            tasks.Add(CreateAlignWithDestination());
            tasks.Add(CreateSitDown());
            tasks.Add(CreateTurnToComputer());
            return tasks.ToArray();
        }

        public ComplexTask CreateWorkAtPC()
        {
            HtnTask.Condition[] preConditions = 
            { 
                () => IsSitting(), 
                () => IsFacingComputer(), 
                () => IsComputerOn(),  
            };
    
            HtnTask.Condition[] integrityRules = 
            {
                () => !StopFlag(),
            };
          
            ComplexTask.DecompositionMethod method = DecomposeWorkAtPC;
      
            var task = new ComplexTask (
                this.name + "WorkAtPC",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeWorkAtPC()
        {
            var tasks = new List<HtnTask>();
            tasks.Add(CreateStartTyping());
            if(IsLoggedIn()) {
            }
            else {
                tasks.Add(CreateLogIn());
            }
            if(WillPlugUsb() & HasUSB()) {
                tasks.Add(CreatePlugInUsb());
            }
            tasks.Add(CreateCheckMail());
            tasks.Add(CreateWaitSomeTime());
            tasks.Add(CreateCheckMail());
            return tasks.ToArray();
        }

	    public ComplexTask CreateStand()
        {
            HtnTask.Condition[] preConditions = HtnTask.EmptyCondition;
        
            HtnTask.Condition[] integrityRules = HtnTask.EmptyCondition;
            
            ComplexTask.DecompositionMethod method = DecomposeStand;
          
            var task = new ComplexTask (
                this.name + "Stand",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeStand()
        {
            var tasks = new List<HtnTask>();
            if(IsFacingComputer()) {
                tasks.Add(CreateTurnFromComputer());
            }
            tasks.Add(CreateStandUp());
            return tasks.ToArray();
        }

	    public ComplexTask CreateWaitAtLocation()
        {
            HtnTask.Condition[] preConditions = HtnTask.EmptyCondition;
        
            HtnTask.Condition[] integrityRules = HtnTask.EmptyCondition;
            
            ComplexTask.DecompositionMethod method = DecomposeWaitAtLocation;
          
            var task = new ComplexTask (
                this.name + "WaitAtLocation",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeWaitAtLocation()
        {
            var tasks = new List<HtnTask>();
            if(HasRequestedToken()) {
            }
            else {
                if(WillChatNPC()) {
                    tasks.Add(CreateInitiateTalkNPC());
                }
                else {
                    if(WillEmote()) {
                        tasks.Add(CreateEmote());
                    }
                    else {
                        tasks.Add(CreateWaitSomeTime());
                    }
                }
                tasks.Add(CreateWaitAtLocation());
            }
            return tasks.ToArray();
        }

	    public ComplexTask CreateFaxRoutine()
        {
            HtnTask.Condition[] preConditions = HtnTask.EmptyCondition;
        
            HtnTask.Condition[] integrityRules = 
            {
                () => !StopFlag(),
            };
              
            ComplexTask.DecompositionMethod method = DecomposeFaxRoutine;
          
            var task = new ComplexTask (
                this.name + "FaxRoutine",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeFaxRoutine()
        {
            var tasks = new List<HtnTask>();
            tasks.Add(CreateGoToPrinterArea());
            if(HasRequestedToken()) {
            }
            else {
                tasks.Add(CreateWaitAtLocation());
            }
            tasks.Add(CreateAdjustPosition());
            tasks.Add(CreateUseFax());
            return tasks.ToArray();
        }

	    public ComplexTask CreateInitiateTalkNPC()
        {
            HtnTask.Condition[] preConditions = 
            { 
                () => CanChat(),  
            };
        
            HtnTask.Condition[] integrityRules = 
            {
                () => !StopFlag(),
            };
              
            ComplexTask.DecompositionMethod method = DecomposeInitiateTalkNPC;
          
            var task = new ComplexTask (
                this.name + "InitiateTalkNPC",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeInitiateTalkNPC()
        {
            var tasks = new List<HtnTask>();
            if(!HasToStopChat()) {
                if(InterlocutorFound()) {
                    tasks.Add(CreateInitiateDialogueRoutine());
                    tasks.Add(CreateChattingNPC());
                }
                else {
                    tasks.Add(CreateWaitForInterlocutor());
                    tasks.Add(CreateInitiateTalkNPC());
                }
            }
            return tasks.ToArray();
        }

        [HtnRootTask("TalkPlayer")]
	    public ComplexTask CreateTalkPlayer()
        {
            HtnTask.Condition[] preConditions = 
            { 
                () => CanChat(),  
            };
        
            HtnTask.Condition[] integrityRules = HtnTask.EmptyCondition;
            
            ComplexTask.DecompositionMethod method = DecomposeTalkPlayer;
          
            var task = new ComplexTask (
                this.name + "TalkPlayer",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeTalkPlayer()
        {
            var tasks = new List<HtnTask>();
            tasks.Add(CreateInitiateDialogueRoutine());
            if(WillTalk()) {
                tasks.Add(CreateStopConversation());
            }
            else {
                tasks.Add(CreateListenAndReply());
            }
            return tasks.ToArray();
        }

	    public ComplexTask CreateListenAndReply()
        {
            HtnTask.Condition[] preConditions = HtnTask.EmptyCondition;
        
            HtnTask.Condition[] integrityRules = HtnTask.EmptyCondition;
            
            ComplexTask.DecompositionMethod method = DecomposeListenAndReply;
          
            var task = new ComplexTask (
                this.name + "ListenAndReply",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeListenAndReply()
        {
            var tasks = new List<HtnTask>();
            tasks.Add(CreateWaitPlayerLine());
            if(WillApprove()) {
                tasks.Add(CreatePositiveResponse());
            }
            else {
                if(WillTalk()) {
                    tasks.Add(CreateNegativeResponse());
                    tasks.Add(CreateListenAndReply());
                }
                else {
                    tasks.Add(CreateStopConversation());
                }
            }
            return tasks.ToArray();
        }

	    public ComplexTask CreateChattingNPC()
        {
            HtnTask.Condition[] preConditions = HtnTask.EmptyCondition;
        
            HtnTask.Condition[] integrityRules = HtnTask.EmptyCondition;
            
            ComplexTask.DecompositionMethod method = DecomposeChattingNPC;
          
            var task = new ComplexTask (
                this.name + "ChattingNPC",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeChattingNPC()
        {
            var tasks = new List<HtnTask>();
            tasks.Add(CreateWaitNPCLine());
            tasks.Add(CreateEmote());
            tasks.Add(CreateNPCLine());
            if(HasToStopChat()) {
            }
            else {
                tasks.Add(CreateChattingNPC());
            }
            return tasks.ToArray();
        }
        
        [HtnRootTask("Break")]
	    public ComplexTask CreateBreak()
        {
            HtnTask.Condition[] preConditions = HtnTask.EmptyCondition;
        
            HtnTask.Condition[] integrityRules = 
            {
                () => !StopFlag(),
            };
              
            ComplexTask.DecompositionMethod method = DecomposeBreak;
          
            var task = new ComplexTask (
                this.name + "Break",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeBreak()
        {
            var tasks = new List<HtnTask>();
            if(WillGoToBreakRoom()) {
                tasks.Add(CreateGoToBreakRoom());
            }
            else {
                if(WillSpeakWithFriend()) {
                    tasks.Add(CreateWalkToFriend());
                    tasks.Add(CreateInitiateTalkNPC());
                }
                else {
                    if(WillGoToKitchen()) {
                        tasks.Add(CreateWalkToKitchen());
                        tasks.Add(CreateEmote());
                    }
                    tasks.Add(CreateFinishAllActivities());
                }
            }
            return tasks.ToArray();
        }

	    public ComplexTask CreateGoToBreakRoom()
        {
            HtnTask.Condition[] preConditions = HtnTask.EmptyCondition;
        
            HtnTask.Condition[] integrityRules = HtnTask.EmptyCondition;
            
            ComplexTask.DecompositionMethod method = DecomposeGoToBreakRoom;
          
            var task = new ComplexTask (
                this.name + "GoToBreakRoom",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeGoToBreakRoom()
        {
            var tasks = new List<HtnTask>();
            tasks.Add(CreateWalkToBreakRoom());
            tasks.Add(CreateFindPlace());
            tasks.Add(CreateInitiateTalkNPC());
            return tasks.ToArray();
        }

	    public ComplexTask CreateFindPlace()
        {
            HtnTask.Condition[] preConditions = 
            { 
                () => IsNearDestination(),  
            };
        
            HtnTask.Condition[] integrityRules = HtnTask.EmptyCondition;
            
            ComplexTask.DecompositionMethod method = DecomposeFindPlace;
          
            var task = new ComplexTask (
                this.name + "FindPlace",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposeFindPlace()
        {
            var tasks = new List<HtnTask>();
            if(CanSit()) {
                tasks.Add(CreateWalkToSofa());
                tasks.Add(CreateSit());
            }
            return tasks.ToArray();
        }
        
        [HtnRootTask("PickUpUSBDrive")]
	    public ComplexTask CreatePickUpUSBDrive()
        {
            HtnTask.Condition[] preConditions = HtnTask.EmptyCondition;
        
            HtnTask.Condition[] integrityRules = HtnTask.EmptyCondition;
            
            ComplexTask.DecompositionMethod method = DecomposePickUpUSBDrive;
          
            var task = new ComplexTask (
                this.name + "PickUpUSBDrive",
                method,
                preConditions,
                integrityRules);
            return task;
        }
        public HtnTask[] DecomposePickUpUSBDrive()
        {
            var tasks = new List<HtnTask>();
            tasks.Add(CreateWalkToUSB());
            tasks.Add(CreatePickUpUSB());
            return tasks.ToArray();
        }

        [HtnRootTask("ScarePlayerAway")]
        public ComplexTask CreateScarePlayerAway()
        {
            HtnTask.Condition[] preConditions = HtnTask.EmptyCondition;
            HtnTask.Condition[] integrityRules = HtnTask.EmptyCondition;
            
            ComplexTask.DecompositionMethod method = DecomposeScarePlayerAway;
              
            var task = new ComplexTask (
                this.name + "ScarePlayerAway",
                method, 
                preConditions, 
                integrityRules); 
            return task;
        }
        public HtnTask[] DecomposeScarePlayerAway()
        {
            var tasks = new List<HtnTask>();
            if(IsSitting()) { 
                tasks.Add(CreateStand());
            }
            tasks.Add(CreateEmote());
            return tasks.ToArray();
        }

        #endregion
    }
}
