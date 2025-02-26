using System.Collections.Generic;
using UnityEngine;

namespace Systems.BehaviourTree
{
    public abstract class BehaviorNode
    {
        public enum NodeState { Running, Success, Failure }
        protected NodeState _state = NodeState.Running;
        public NodeState State => _state;
        public abstract NodeState Execute();
    }

    public class TimerNode : BehaviorNode
    {
        private float _interval;
        private BehaviorNode _childNode;
        private float _elapsedTime = 0f;

        public TimerNode(float interval, BehaviorNode childNode)
        {
            this._interval = interval;
            this._childNode = childNode;
        }

        public override NodeState Execute()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _interval)
            {
                _elapsedTime = 0f;
                return _childNode.Execute();
            }

            return NodeState.Running;
        }
    }

    public class DecayAggroNode : BehaviorNode
    {
        private BossController _boss;

        public DecayAggroNode(BossController boss)
        {
            this._boss = boss;
        }

        public override NodeState Execute()
        {
            _boss.CalculateDecay();
            return NodeState.Success;
        }
    }

    public class SelectBestTargetNode : BehaviorNode
    {
        private BossController _boss;

        public SelectBestTargetNode(BossController boss)
        {
            this._boss = boss;
        }

        public override NodeState Execute()
        {
            _boss.SelectBestTarget();
            return _boss.CurrentTarget != null ? NodeState.Success : NodeState.Failure;
        }
    }

    public class AttackTargetNode : BehaviorNode
    {
        private BossController _boss;

        public AttackTargetNode(BossController boss)
        {
            this._boss = boss;
        }

        public override NodeState Execute()
        {
            _boss.AttackTarget();
            return NodeState.Success;
        }
    }

    public class BehaviorSequence : BehaviorNode
    {
        private List<BehaviorNode> _nodes = new();
        private int _currentNodeIndex = 0;

        public void AddNode(BehaviorNode node)
        {
            _nodes.Add(node);
        }

        public override NodeState Execute()
        {
            if (_currentNodeIndex >= _nodes.Count)
            {
                _currentNodeIndex = 0;
                return NodeState.Success;
            }

            var currentNode = _nodes[_currentNodeIndex];
            var result = currentNode.Execute();

            if (result == NodeState.Running)
            {
                return NodeState.Running;
            }

            if (result == NodeState.Success)
            {
                _currentNodeIndex++;
                return Execute();
            }

            return NodeState.Failure;
        }
    }

}