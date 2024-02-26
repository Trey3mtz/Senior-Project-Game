using System.Collections;
using System.Collections.Generic;
using Cyrcadian.UtilityAI;
using Unity.Assertions;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cyrcadian.BehaviorTrees
{
    /// <summary>
    ///             Here is the logical break down of the Behavior Tree structure.
    ///             
    ///                 The shape of this is a  basic tree data structure, one with 1 root NODE, and N children NODE. Where N represents the states of logic we want in a tree.
    ///                 Each child also has N children NODES, where N is just some amount. 
    ///                 
    /// 
    ///                 + Besides the base verion of NODE, there are 2 types of NODEs:    "Sequence" and "Selector"
    ///                 +                                           NODES have states:    RUNNING, SUCCESS, and FAILURE.
    ///                 
    ///                 Sequence NODEs:     These return immediately if met with FAILURE. Otherwise they won't return until they go through all children NODES
    ///                 Selector NODEs:     These return immediately if met with SUCCESS or RUNNING. Otherwise they won't return until they go through all children NODES
    ///                 
    ///                 They return to whomever their parent is, and they return with whatever state made them return. Their parent can be selector or sequence.
    ///                 Only LEAVES or Children NODEs without any children of their own should be "Base" class NODEs. Else they should be Selector or Sequence.
    ///                 
    ///                 CAUTION: Do NOT SetData() outside of the _root NODE for the tree, like in any children.
    ///                          This is so all children can access the shared information, and it also so we don't leave any lingering dictionaries in memory.
    /// </summary>

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

        // Short Cut that SHOULD ALWAYS BE used over Set Data. 
        // You should AVOID usING SetData() as strings and objects are the roof of flexibility in C# dictionaries, therefore a tree's root should hold the dictionary with key-pair values.
        public void SetRootData(string key, object value)
        {
            Node myRoot = GetTreeRoot(this);
            myRoot._dataContext[key] = value;
        }

        protected Node GetTreeRoot(Node node)
        {
            Node Mega_Turbo_Parent = node.parent;
            while(Mega_Turbo_Parent != null)
            {
                if(Mega_Turbo_Parent.parent == null)
                    break;
                Mega_Turbo_Parent = Mega_Turbo_Parent.parent;
            }

            return Mega_Turbo_Parent;
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

        public void StopBehavior()
        {
             CreatureBehaviorTree thisTree = GetData("MyTreeRoot") as CreatureBehaviorTree;
            if(thisTree.StopBehavior())
                ClearTreeData();
        }

        public void SelfDestructCreatureTree()
        {
            CreatureBehaviorTree thisTree = GetData("MyTreeRoot") as CreatureBehaviorTree;
            if(ClearTreeData())
                thisTree.DeleteTree();
        }

        public void SelfDestructBehaviorTree()
        {
            BehaviorTree thisTree = GetData("MyTreeRoot") as BehaviorTree;
            if(ClearTreeData())
                thisTree.FinishedWithTree();
        }

        public bool ClearTreeData()
        {
            Node myRoot = GetTreeRoot(this);
            ClearChildrenData(myRoot);

            Assert.IsTrue(myRoot._dataContext.Count == 0);
            

            // Can only return true since it will do it's job.
            return true;
        }

        public void ClearChildrenData(Node node)
        {

            foreach(Node child in node.children)
            {
                // Check if my children have children.
                if(child.children.Count != 0)
                    ClearChildrenData(child);        
                // If they have no child no worries.
            }
        
            // Clear myself after all children are cleared, or I have none.
            node._dataContext.Clear();
        }

#region Only For Destroying Creatures who are taking up memory.
        // Only the father survives...
        public void FreeChildrenFromTheirFleshPrisons()
        {
            Node theFather = GetTreeRoot(this);
            foreach(Node littleOne in theFather.children)
            {
                NotJustTheMen(littleOne);   
            }
            // All kids are gone.
        }

        // Utterly destroys all children of this given node.
        public bool NotJustTheMen(Node thisNode)
        {
            // Omae Wa Mou Shindeiru.
            if(thisNode == null)
                return NANI;

            // "Inspect" every last one of thisNode's child.
            foreach(Node child in thisNode.children)
            {
                // If this child has children, no it doesn't.
                NotJustTheMen(child);    
            }

            thisNode = null;
            return true;
        }

        private bool NANI = true;
#endregion
    }

/// <summary>
///                 Designing Trees:    When designing a tree, Always have the "Action" portion be a leaf, and always have the nodes inbetween
///                                     The Root and leaves be "Checks". If the "Action" is under a Selector and SUCCESSEDS, call "StopBehavior" from the Tree.
///                                     If the "Action" was under sequence dont call it yet. Under a Sequence you call "StopBehavior"
///                                     if the "Action" under Sequence FAILS or if it was the last "Action" in the Sequence, then call "StopBehavior".
///                                     
///                                     
/// </summary>

    public abstract class CreatureBehaviorTree : MonoBehaviour
    {
        private bool StopUpdate;
        protected Node _root = null;
        protected CreatureController _myCreature;

        protected void Start()
        {
            _myCreature = GetComponent<CreatureController>();
            StopUpdate = false;

            _root = SetupTree(_myCreature);

            _root.SetData("MyCreature", _myCreature);
            _root.SetData("MyTreeRoot", this);
        }

        private void Update()
        {
            if(StopUpdate)
                return;

            if(_root != null)
            {  
                _root.Evaluate();
            }
        }

        protected abstract Node SetupTree(CreatureController creature);


        // USE THIS TO STOP A BEHAVIOR when traversing nodes!!!! 
        // Will notify creature to update their next best action.
        public virtual bool StopBehavior()
        {
            StopUpdate = true;
            _root.ClearChildrenData(_root);
            _myCreature.UponCompletedAction();
            return !StopUpdate;
        }

        // Use this in tandum with StopBehavior so you don't need to recreate/destroy monobehaviors every time someone does a Behavior Tree.
        public bool StartBehavior()
        {
            StopUpdate = false;
            _root.SetData("MyCreature", _myCreature);
            _root.SetData("MyTreeRoot", this);
            return StopUpdate;
        }

        protected CreatureController GetMyCreature()
        {
            return _myCreature;
        }

        public void DeleteTree()
        {
            if(!_myCreature.isDying)
                Debug.Log(" IF YOU ARE NOT DESTROYING A CREATURE, YOU SHOULD NOT BE SEEING THIS : you should've used StopBehavior");
            _root.FreeChildrenFromTheirFleshPrisons();
            _root = null;
            Destroy(this);
        }
    }

    public abstract class BehaviorTree : MonoBehaviour
    {

        private Node _root = null;

        protected void Start()
        {
            _root = SetupTree();
            _root.SetData("MyTreeRoot", this);
        }

        private void Update()
        {
            if (_root != null)
                _root.Evaluate();
        }

        protected abstract Node SetupTree();

        public void FinishedWithTree()
        {
            _root = null;
            Destroy(this);
        }
    }

    /// <summary>
    ///                         This is a node that executes all of its children nodes, in a sequence. Act similar to an [AND GATE]  
    ///                         " Only return if met with FAILURE in a child node. Until then go through all my children. "
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
    ///                         " If all my child nodes fail, only then do I return FAILURE. If SUCCESS or RUNNING, return immediately.
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
