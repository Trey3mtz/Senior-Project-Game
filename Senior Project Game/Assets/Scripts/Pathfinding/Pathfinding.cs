//using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEditor.Build.Utilities;

using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int  MOVE_DIAGONAL_COST = 14;


    /*********************************************************************************************************************************************
    
        FindPath from A* was made into a Job to fit Unity's DOTS system. This should yield great performance over the typical gameobject/class structure.
        It begins with a simple starting and ending position. Following that is an Execute() function which is the main A* algorithm.
        The functions following are helper functions.
    
    */

    [BurstCompile]
    private struct FindPathJob : IJob 
    {
        public int2 startPos;
        public int2 endPos;

        public void Execute() 
        {
            int2 gridSize = new int2(100, 100);

            NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

            for (int x = 0; x < gridSize.x; x++) {
                for (int y = 0; y < gridSize.y; y++) {
                    PathNode pathNode = new PathNode();
                    pathNode.x = x;
                    pathNode.y = y;
                    pathNode.index = CalculateIndex(x, y, gridSize.x);

                    pathNode.gCost = int.MaxValue;
                    pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPos);
                    pathNode.CalculateFCost();

                    pathNode.isWalkable = true;
                    pathNode.cameFromNodeIndex = -1;

                    pathNodeArray[pathNode.index] = pathNode;
                }
            }

            // These are the 8 directions one could take from a given node or spot in the A* algorithm.
            NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
            neighbourOffsetArray[0] = new int2(-1, 0); // Left
            neighbourOffsetArray[1] = new int2(+1, 0); // Right
            neighbourOffsetArray[2] = new int2(0, +1); // Up
            neighbourOffsetArray[3] = new int2(0, -1); // Down
            neighbourOffsetArray[4] = new int2(-1, -1); // Left Down
            neighbourOffsetArray[5] = new int2(-1, +1); // Left Up
            neighbourOffsetArray[6] = new int2(+1, -1); // Right Down
            neighbourOffsetArray[7] = new int2(+1, +1); // Right Up

            int endNodeIndex = CalculateIndex(endPos.x, endPos.y, gridSize.x);

            PathNode startNode = pathNodeArray[CalculateIndex(startPos.x, startPos.y, gridSize.x)];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            pathNodeArray[startNode.index] = startNode;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> clsoedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNode.index);

            while(openList.Length > 0)
            {
                int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
                PathNode currentNode = pathNodeArray[currentNodeIndex];

                if(currentNodeIndex == endNodeIndex)
                {
                    // You have reached your destination
                    break;
                }

                // Remove current node from OpenList
                for(int i = 0; i < openList.Length; i++)
                {
                    if(openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                clsoedList.Add(currentNodeIndex);

                for(int i = 0; i < neighbourOffsetArray.Length; i++)
                {
                    int2 neighbourOffset = neighbourOffsetArray[i];
                    int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);
                
                    if(!IsPositionInsideGrid(neighbourPosition, gridSize))
                    {
                        // Neighbour not in a valid position
                        continue;
                    }

                    int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);
                    
                    if(clsoedList.Contains(neighbourNodeIndex))
                    {
                        // It already searched this node
                        continue;
                    }

                    PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                    if(!neighbourNode.isWalkable)
                    {
                        // This is not walkable
                        continue;
                    }

                    int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                    if(tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNodeIndex = currentNodeIndex;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.CalculateFCost();
                        pathNodeArray[neighbourNodeIndex] = neighbourNode;

                        if(!openList.Contains(neighbourNode.index))
                        {
                            openList.Add(neighbourNode.index);
                        }
                    }
                }

            }

            PathNode endNode = pathNodeArray[endNodeIndex];
            if(endNode.cameFromNodeIndex == -1){
                    // Did not find a path
                    //Debug.Log("Did not find a path");
            }
            else{
                    // Found a path!
                    NativeList<int2> path = CalculatePath(pathNodeArray, endNode);

                    //foreach(int2 pathPosition in path)
                    //    Debug.Log(pathPosition);
                        
                    path.Dispose();
            }

            
            // Dispose all Native Arrays created at the end of this Execute()
            pathNodeArray.Dispose();
            neighbourOffsetArray.Dispose();
            openList.Dispose();
            clsoedList.Dispose();
        }

    /***********************************************************************************************************
     Calculates a path given an array of PathNodes, and an ending PathNode. Returns in reverse.
    
    */
    private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
    {
        if(endNode.cameFromNodeIndex == -1)
        {   // Could not find a path
            return new NativeList<int2>(Allocator.Temp);
        }
        else{
            // Found a path, so walk backwards
            NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
            path.Add(new int2(endNode.x, endNode.y));

            PathNode currentNode = endNode;
            while(currentNode.cameFromNodeIndex != -1 )
            {
                PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                path.Add(new int2(cameFromNode.x, cameFromNode.y));
                currentNode = cameFromNode;
            }

            return path;
        }
    }

    private bool IsPositionInsideGrid(int2 gridPos, int2 gridSize)
    {
        return  gridPos.x >= 0 && 
                gridPos.y >= 0 &&
                gridPos.x < gridSize.x &&
                gridPos.y < gridSize.y;
    }

    private int CalculateIndex(int x, int y, int gridWidth)
    {
        return x + y * gridWidth;
    }

    
    /***********************************************************************************************************
    This function calculates the distance cost from A to B, and uses 
        the A* weight of 10 or 14 depending on if its a diagonal path or straight one; 

    */

    private int CalculateDistanceCost(int2 aPos, int2 bPos)
    {
        int xDistance = math.abs(aPos.x - bPos.x);
        int yDistance = math.abs(aPos.y - bPos.y);
        int remaining = math.abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }


    /**********************************************************************************************************
        This function simply gets the index of the lowest F Cost in an array of PathNodes.
        [ NOTE: F Cost refers to the variable F commonly used in the A* algorithm. It is the total cost of travel ]
    */
    private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
    {
        PathNode lowestCostPathNode = pathNodeArray[openList[0]];
        for(int i = 1; i < openList.Length; i++)
        {
            PathNode testPathNode = pathNodeArray[openList[i]];
            if(testPathNode.fCost < lowestCostPathNode.fCost)
                lowestCostPathNode = testPathNode;
        }

        return lowestCostPathNode.index;
    }

    /**********************************************************************************************************
    PathNode is meant to be used for a burst compile job. So it is built as a struct holding data.

    */
    private struct PathNode
    {   
    public int x;
    public int y;

    public int index;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;

    public int cameFromNodeIndex;

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }

    }
}
