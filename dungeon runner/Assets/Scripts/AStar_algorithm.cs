using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar_algorithm : MonoBehaviour
{
    // Attach to a transform that you
    // want to set a destination for

    public GameObject Boss;
    public GameObject Player;
    public List<GameObject> nodes;
    public List<GameObject> OpenList;
    public List<GameObject> OpenList1;


    private float t_TraverseTime = 0;
    private bool HasReached0thNode = false;
    private bool HasSavedValue = false;
    private bool HasFinshedPath = false;
    Vector3 bossPos;
    private bool newPath = true;
    private int pathIndex = 0;
    private GameObject previous_nearNodePlayer;

    GameObject findNearNode(GameObject entity)
    {
        GameObject nearestNode;
        float shortestDist = Mathf.Infinity;
        int shortestNode = 0;

        for (int i = 0; i < nodes.Count; i++)
        {
            //print(nodes[i].transform.position);

            float dist = Vector3.Distance(entity.transform.position, nodes[i].transform.position);

            if (dist < shortestDist)
            {
                shortestNode = i;
                shortestDist = dist;
            }
        }
        nearestNode = nodes[shortestNode].gameObject;

        return nearestNode;
    }

    void fillOpenList(GameObject startingNode)
    {
        OpenList.Add(startingNode);
        startingNode.GetComponent<Node>().searched = true;

        // either find another attached node to go through the loop
        // Or if the last node in openlist == nearNode_player break recusion
        if (startingNode != findNearNode(Player))
        {
            float shortestDist = Mathf.Infinity;
            int shortestNode = 0;

            for (int i = 0; i < startingNode.GetComponent<Node>().AttachedNodes.Count; i++)
            {

                if (startingNode.GetComponent<Node>().AttachedNodes[i].GetComponent<Node>().searched != true)
                {
                    float dist = Vector3.Distance(Player.transform.position, startingNode.GetComponent<Node>().AttachedNodes[i].transform.position);

                    if (dist <= shortestDist)
                    {
                        shortestDist = dist;
                        shortestNode = i;
                    }
                }
            }
            if (OpenList.Count <= nodes.Count)
                fillOpenList(startingNode.GetComponent<Node>().AttachedNodes[shortestNode].gameObject);
        }
    }

    float distanceAcrossNodes(List<GameObject> startList)
    {
        float dist = 0;
        for (int r = 0; r < startList.Count - 1; r++)
        {
            dist += Vector3.Distance(startList[r].transform.position, startList[r + 1].transform.position);
        }

        return dist;
    }

    public GameObject nearNode_boss;
    public Transform nearNode_player;

    void pathAlgorithm()
    {

        nearNode_boss = findNearNode(Boss);
        nearNode_player = findNearNode(Player).transform;

        //shortestPath = null;
        if (OpenList1.Count > 0)
            OpenList1.Clear();

        for (int i = 0; i < nearNode_boss.GetComponent<Node>().AttachedNodes.Count; i++)
        {
            OpenList.Clear();
            // Node Set-up P1 - Set all nodes false
            for (int j = 0; j < nodes.Count; j++)
            {
                //for all nodes. Turn searched false
                if ((nodes[j] != nearNode_boss))
                {
                    nodes[j].GetComponent<Node>().searched = false;
                }
            }
            // Node Set-up P2 - Set nearNode and all its attached nodes true
            for (int k = 0; k < nearNode_boss.GetComponent<Node>().AttachedNodes.Count; k++)
            {
                nearNode_boss.GetComponent<Node>().AttachedNodes[k].gameObject.GetComponent<Node>().searched = true;
            }
            nearNode_boss.GetComponent<Node>().searched = true;

            //-- Now all nodes are properly set up, lets fill openlist --//
            if (OpenList.Count <= nodes.Count)
                fillOpenList(nearNode_boss.GetComponent<Node>().AttachedNodes[i].gameObject);

            //-- Lets calculate the dist of the nodes --//
            //-- We now have a list (openlist) filled with nodes
            float dist = 0, dist1 = 0;

            for (int f = 0; f < OpenList.Count - 1; f++)
            {
                dist += Vector3.Distance(OpenList[f].transform.position, OpenList[f + 1].transform.position);
            }

            // Compares path in Openlist dist to path in openlist1 and fills openlist1 with the shortest
            if (OpenList1.Count != 0)
            {
                // Secondary list is full
                for (int f = 0; f < OpenList1.Count - 1; f++)
                {
                    dist1 += Vector3.Distance(OpenList1[f].transform.position, OpenList1[f + 1].transform.position);
                }
                if (dist <= dist1)
                {
                    OpenList1.Clear();
                    OpenList1.AddRange(OpenList);
                }
            }
            else
            {
                // Meaning The secondary openlist in empty
                OpenList1.Clear();
                OpenList1.AddRange(OpenList);
            }
        }
    }

    void FixedUpdate()
    {
        //if (Boss = true)
        {


            //OpenList.Clear();
            //OpenList1.Clear();
            //
            //for (int j = 0; j < nodes.Count; j++)
            //{
            //    //for all nodes. Turn searched false
            //    if ((nodes[j] != nearNode_boss))
            //    {
            //        nodes[j].GetComponent<Node>().searched = false;
            //    }
            //}
            //newPath = true;

            // if nothing in shortest path list (openlist1) or newPath == true; Then create path;
            if ((OpenList1.Count == 0) || (newPath))
            {
                pathAlgorithm();
                newPath = false;

                // We already have a path and dont want a new one
                // Lets Send the enemy in dir of corrent node
                if (Vector2.Distance(Boss.transform.position, OpenList1[pathIndex].transform.position) < 0.2f)
                {
                    if (pathIndex < OpenList1.Count - 1)
                        pathIndex++;
                }
                // Movement
                Boss.transform.position = (Vector2.MoveTowards(Boss.transform.position, OpenList1[pathIndex].transform.position, 0.1f));

                // Logic for when we need a new path
                // Basically: if the node near player isnt the same as the last node in the list
                if (findNearNode(Player) != (OpenList1[OpenList1.Count - 1]))
                {
                    pathIndex = 0;
                    newPath = true;
                }
            }
            else
            {
                // We already have a path and dont want a new one
                // Lets Send the enemy in dir of corrent node
                if (Vector2.Distance(Boss.transform.position, OpenList1[pathIndex].transform.position) < 0.5f)
                {
                    if (pathIndex < OpenList1.Count - 1)
                        pathIndex++;
                }
                // Movement
                Boss.transform.position = (Vector2.MoveTowards(Boss.transform.position, OpenList1[pathIndex].transform.position, 0.2f));

                // Logic for when we need a new path
                // Basically: if the node near player isnt the same as the last node in the list
                if (findNearNode(Player) != (OpenList1[OpenList1.Count - 1]))
                {
                    pathIndex = 0;
                    newPath = true;
                }
            }

            //------------- TRAVERSAL Visuals (debug) -------------//
            for (int u = 0; u < OpenList1.Count - 1; u++)
            {
                Debug.DrawLine(OpenList1[u].transform.position, OpenList1[u + 1].transform.position);
            }
        }
    }
}