﻿using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace redd096
{
    /// <summary>
    /// Used to create dynamic NotWalkable nodes on the grid
    /// </summary>
    [AddComponentMenu("redd096/Path Finding A Star/Obstacle A Star 2D")]
    public class ObstacleAStar2D : MonoBehaviour
    {
        enum ETypeCollider { circle, box }

        [Header("Collider Obstacle")]
        [SerializeField] Vector2 offset = Vector2.zero;
        [SerializeField] ETypeCollider typeCollider = ETypeCollider.box;
        [EnableIf("typeCollider", ETypeCollider.box)] [SerializeField] Vector2 sizeCollider = Vector2.one;
        [EnableIf("typeCollider", ETypeCollider.circle)] [SerializeField] float radiusCollider = 1;

        [Header("DEBUG")]
        [SerializeField] bool drawDebug = false;

        //vars
        bool isActive = true;
        GridAStar2D grid;
        List<Node2D> nodesPosition = new List<Node2D>();    //nodes with this obstacle

        //nodes to calculate
        Node2D centerNode;
        Node2D leftNode;
        Node2D rightNode;
        Node2D upNode;
        Node2D downNode;
        Node2D nodeToCheck;

        void OnDrawGizmos()
        {
            if (drawDebug)
            {
                Gizmos.color = Color.cyan;

                //draw box
                if (typeCollider == ETypeCollider.box)
                {
                    Gizmos.DrawWireCube((Vector2)transform.position + offset, sizeCollider);
                }
                //draw circle
                else
                {
                    Gizmos.DrawWireSphere((Vector2)transform.position + offset, radiusCollider);
                }

                Gizmos.color = Color.white;
            }
        }

        #region public API

        /// <summary>
        /// Calculate new position on the grid and update nodes
        /// </summary>
        /// <param name="grid"></param>
        public void UpdatePositionOnGrid(GridAStar2D grid)
        {
            if (grid == null)
                return;

            //set vars
            this.grid = grid;

            //update nodes
            RemoveFromPreviousNodes();
            if (isActive) SetNewNodes();
        }

        public void RemoveColliders()
        {
            isActive = false;
        }

        /// <summary>
        /// Remove from current nodes
        /// </summary>
        public void RemoveFromPreviousNodes()
        {
            //remove this from previous nodes
            foreach (Node2D node in nodesPosition)
            {
                if (node != null && node.obstaclesOnThisNode.Contains(this))
                    node.obstaclesOnThisNode.Remove(this);
            }

            //clear list
            nodesPosition.Clear();
        }

        /// <summary>
        /// Calculate new position on the grid and add to new nodes (is better use UpdatePositionOnGrid to set grid and remove from previous nodes)
        /// </summary>
        public void SetNewNodes()
        {
            if (grid == null)
                return;

            //set nodes using box or circle
            if (typeCollider == ETypeCollider.box)
                SetNodesUsingBox();
            else
                SetNodesUsingCircle();
        }

        #endregion

        #region private API

        void SetNodesUsingBox()
        {
            //calculate nodes
            //use an offset to check if node is inside also if collider not reach center of the node (add grid.NodeRadius in the half size)
            centerNode = grid.GetNodeFromWorldPosition((Vector2)transform.position + offset);
            grid.GetNodesExtremesOfABox(centerNode, (Vector2)transform.position + offset, (sizeCollider * 0.5f) + (Vector2.one * grid.NodeRadius), out leftNode, out rightNode, out downNode, out upNode);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = downNode.gridPosition.y; y <= upNode.gridPosition.y; y++)
                {
                    nodeToCheck = grid.GetNodeByCoordinates(x, y);

                    //set it
                    if (nodeToCheck.obstaclesOnThisNode.Contains(this) == false)
                        nodeToCheck.obstaclesOnThisNode.Add(this);

                    //and add to the list
                    if (nodesPosition.Contains(nodeToCheck) == false)
                        nodesPosition.Add(nodeToCheck);
                }
            }
        }

        void SetNodesUsingCircle()
        {
            //calculate nodes
            //use an offset to check if node is inside also if collider not reach center of the node (add grid.NodeRadius in the half size)
            centerNode = grid.GetNodeFromWorldPosition((Vector2)transform.position + offset);
            grid.GetNodesExtremesOfABox(centerNode, (Vector2)transform.position + offset, (Vector2.one * radiusCollider) + (Vector2.one * grid.NodeRadius), out leftNode, out rightNode, out downNode, out upNode);

            //check every node
            for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
            {
                for (int y = downNode.gridPosition.y; y <= upNode.gridPosition.y; y++)
                {
                    nodeToCheck = grid.GetNodeByCoordinates(x, y);

                    //if inside radius (+ node radius offset)
                    if (Vector2.Distance(centerNode.worldPosition, nodeToCheck.worldPosition) <= radiusCollider + grid.NodeRadius)
                    {
                        //set it
                        if (nodeToCheck.obstaclesOnThisNode.Contains(this) == false)
                            nodeToCheck.obstaclesOnThisNode.Add(this);

                        //and add to the list
                        if (nodesPosition.Contains(nodeToCheck) == false)
                            nodesPosition.Add(nodeToCheck);
                    }
                }
            }
        }

        //void SetNodesUsingColliders()
        //{
        //    //foreach collider
        //    foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
        //    {
        //        if (col == null)
        //            continue;
        //
        //        //calculate nodes
        //        //use an offset to check if node is inside also if collider not reach center of the node (add grid.NodeRadius in the half size)
        //        centerNode = grid.GetNodeFromWorldPosition(col.bounds.center);
        //        grid.GetNodesExtremesOfABox(centerNode, col.bounds.center, (Vector2)col.bounds.extents + (Vector2.one * grid.NodeRadius), out leftNode, out rightNode, out downNode, out upNode);
        //
        //        //check every node
        //        Node2D nodeToCheck;
        //        for (int x = leftNode.gridPosition.x; x <= rightNode.gridPosition.x; x++)
        //        {
        //            for (int y = downNode.gridPosition.y; y <= upNode.gridPosition.y; y++)
        //            {
        //                nodeToCheck = grid.GetNodeByCoordinates(x, y);
        //
        //                //if node is inside collider (+ node radius offset)
        //                if (Vector2.Distance(col.ClosestPoint(nodeToCheck.worldPosition), nodeToCheck.worldPosition) < Mathf.Epsilon + grid.NodeRadius)
        //                {
        //                    //set it
        //                    if (nodeToCheck.obstaclesOnThisNode.Contains(this) == false)
        //                        nodeToCheck.obstaclesOnThisNode.Add(this);
        //
        //                    //and add to the list
        //                    if (nodesPosition.Contains(nodeToCheck) == false)
        //                        nodesPosition.Add(nodeToCheck);
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion
    }
}