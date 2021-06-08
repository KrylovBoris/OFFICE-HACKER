using System.Collections.Generic;

namespace KnowledgeSystem
{
    public class KnowledgeContainer
    {
        private readonly List<Transaction> _transactionTimeline;
        private readonly ActionGraph _transactionGraph;
        private readonly Environment _environment;

        public bool IsComplete => _transactionGraph.IsFinal;

        public IReadOnlyList<Transaction> Timeline => _transactionTimeline;

        public KnowledgeContainer(Environment env, params (string, bool)[] finishingStateFacts)
        {
            _environment = env;
            _transactionGraph = new ActionGraph(_environment, finishingStateFacts);
            _transactionTimeline = new List<Transaction>();
        }

        public void ReceiveTransaction(Transaction transaction)
        {
            _transactionTimeline.Add(transaction);
            _transactionGraph.AddNode(transaction);
        }

        private class ActionGraph
        {
            private class Node
            {
                protected readonly State representedState;
                public State NodeState => representedState;
                protected readonly Node previous;
                private readonly string _nodeAction;
                public string Action => _nodeAction;

                public Node(State nodeState, Node previousNode, string action)
                {
                    representedState = nodeState;
                    previous = previousNode;
                    _nodeAction = action;
                }
            }

            private class JointNode : Node
            {
                private readonly Node[] _otherPreviousNodes;
                
                public JointNode(State nodeState, List<Node> previousNodes) : base (nodeState, previousNodes[0], "SateJoint")
                {
                    previousNodes.RemoveAt(0);
                    _otherPreviousNodes = previousNodes.ToArray();
                }
            }
           
            private readonly Environment _environment;
            private Node _initialNode;
            private readonly State _finishingState;
            private readonly List<State> _timeline;
            private readonly List<Node> _nodeList;
            private Node _finalNode;

            public bool IsFinal => (_finalNode != null);
                
            public void AddNode(Transaction newNode)
            {
                var preCond = newNode.Preconditions;
                var effect = newNode.Effect;
                var newAction = newNode.ActionToken;

                Node chosenNode;
                var applicableStateIndex = 0;
                var precondIndex = 0;
                while (applicableStateIndex >= _timeline.Count || !_timeline[applicableStateIndex].StateApplicable(preCond[precondIndex]))
                {
                    precondIndex += (applicableStateIndex > 0) ? 1 : 0;
                    for (applicableStateIndex = 0; (applicableStateIndex < _timeline.Count) && !_timeline[applicableStateIndex].StateApplicable(preCond[precondIndex]); applicableStateIndex++){}
                }

                if (_nodeList[applicableStateIndex].NodeState.StateApplicable(preCond[precondIndex]))
                {
                    chosenNode = _nodeList[applicableStateIndex];
                }
                else
                {
                    var listToJoin = new List<Node>();
                    var stateAggregation = _environment.NullState();
                    for (var i = 1; i <= applicableStateIndex && !stateAggregation.StateApplicable(preCond[precondIndex]); i++)
                    {
                        if (!(_nodeList[i].NodeState & preCond[precondIndex]).IsNull())
                        {
                            stateAggregation = stateAggregation | _nodeList[i].NodeState;
                            listToJoin.Add(_nodeList[i]);
                        }
                    }
                    chosenNode = new JointNode(stateAggregation, listToJoin);
                    _nodeList.Add(chosenNode);
                    _timeline.Add(_timeline[_timeline.Count - 1] | chosenNode.NodeState);
                }

                var addedNode = new Node(chosenNode.NodeState | effect, chosenNode, newAction);
                _nodeList.Add(addedNode);
                _timeline.Add(_timeline[_timeline.Count - 1] | addedNode.NodeState);

                if (addedNode.NodeState.StateApplicable(_finishingState))
                {
                    _finalNode = addedNode;
                    return;
                }

                if (_timeline[_timeline.Count - 1].StateApplicable(_finishingState))
                {
                    var listToJoin = new List<Node>();
                    State stateAggregation = _environment.NullState();
                    for (int i = 1; i <= (_timeline.Count - 1) && !stateAggregation.StateApplicable(_finishingState); i++)
                    {
                        if (!(_nodeList[i].NodeState & _finishingState).IsNull())
                        {
                            stateAggregation = stateAggregation | _nodeList[i].NodeState;
                            listToJoin.Add(_nodeList[i]);
                        }
                    }
                    _finalNode = new JointNode(stateAggregation, listToJoin);
                    _nodeList.Add(_finalNode);
                }
            }

            public ActionGraph(Environment environment, params (string, bool)[] finishingStateFacts)
            {
                _environment = environment;
                _initialNode = new Node(environment.NullState(), null, string.Empty);
                _finishingState = new State(environment, environment.GetStateHolder(finishingStateFacts));
                _nodeList = new List<Node>{_initialNode};
                _timeline = new List<State>{environment.NullState()};
            }
        }
        
        
    }
}