using System.Collections;
using System.Collections.Generic;
using Cyrcadian.UtilityAI;
using Unity.Assertions;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cyrcadian.BehaviorTrees
{
   public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node
    {
        protected NodeState state;

        public Node parent;
        protected List<Node> children = new List<Node>();

        protected Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        public Node()
        {
            parent = null;
        }

        public Node(List<Node> children)
        {
            foreach (Node child in children)
                _Attach(child);
        }

        private void _Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;

        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }

        public object GetData(string key)
        {
            object value = null;
            if (_dataContext.TryGetValue(key, out value))
                return value;

            Node node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                    return value;
                node = node.parent;
            }
            return null;
        }

        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                    return true;
                node = node.parent;
            }
            return false;
        }

        public bool ClearAllData()
        {
            if(_dataContext.Count != 0)
            {
                _dataContext.Clear();
                _dataContext.TrimExcess();
                Assert.AreEqual(0, _dataContext.Count);
                return true;
            }

            Node node = parent;
            while(node != null)
            {   
                bool cleared = node.ClearAllData();
                if(cleared)
                    return true;
                node = node.parent;
            }

            return false;
        }
    }

    public abstract class CreatureBehaviorTree : MonoBehaviour
    {

        protected Node _root = null;
        protected CreatureController _myCreature;

        protected void Start()
        {
            _myCreature = GetComponent<CreatureController>();
            _root = SetupTree(_myCreature);
            _root.SetData("MyCreature", _myCreature);
        }

        public virtual void Update()
        {
            if(_root != null)
            {  
                _root.Evaluate();
            }
        }

        protected abstract Node SetupTree(CreatureController creature);

        protected CreatureController GetMyCreature()
        {
            return _myCreature;
        }

        public void FinishedWithTree()
        {
            _root = null;
            Destroy(this);
        }
    }

    public abstract class Tree : MonoBehaviour
    {

        private Node _root = null;

        protected void Start()
        {
            _root = SetupTree();
        }

        private void Update()
        {
            if (_root != null)
                _root.Evaluate();
        }

        protected abstract Node SetupTree();

    }

    /// <summary>
    ///                         This is a node that executes all of its children nodes, in a sequence. Act similar to an [AND GATE]                
    /// </summary>
    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }

           
            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }

    }

    /// <summary>
    ///                         This is a node that decides to only select one Node to go to, from all its children. Acts similar to an [OR GATE]          
    /// </summary>
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        continue;
                }
            }

            
            state = NodeState.FAILURE;
            return state;
        }

    }
}
