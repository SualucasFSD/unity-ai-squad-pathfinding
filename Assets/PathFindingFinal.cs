using System.Collections.Generic;
using UnityEngine;

public class PathFindingFinal : MonoBehaviour
{
    public static PathFindingFinal Instance;
    private void Awake()
    {
        Instance = this;
    }
    public List<PathNode> Theta(PathNode begin, PathNode end, Transform obj)
    {
        List<PathNode> path = AStar(begin, end);
        int current = 0;
        while (current + 2 < path.Count)
        {
            if (ManagerFinal.Instance.LineOfSight(path[current].transform.position, path[current + 2].transform.position))
            {
                path.RemoveAt(current + 1);
            }
            else
            {
                current++;
            }
        }
        if (path.Count >= 2)
        {
            if (ManagerFinal.Instance.LineOfSight(obj.position, path[1].transform.position))
            {
                path.RemoveAt(0);
            }
        }
        return path;
    }
    public List<PathNode> AStar(PathNode begin, PathNode end)
    {
        OwnList<PathNode> closeNodes = new OwnList<PathNode>();
        closeNodes.Add(begin, 0);
        Dictionary<PathNode, PathNode> reached = new Dictionary<PathNode, PathNode>();
        Dictionary<PathNode, int> Cost = new Dictionary<PathNode, int>();
        Cost.Add(begin, 0);
        while (closeNodes.Count > 0)
        {
            PathNode current = closeNodes.ObtainAndRemove();
            if (current == end)
            {
                List<PathNode> path = new List<PathNode>();
                while (current != begin)
                {
                    path.Add(current);
                    current = reached[current];
                }
                path.Add(current);
                path.Reverse();
                return path;
            }
            foreach (PathNode node in current.Neighbords)
            {
                //if (!node.BLock) continue;
                int newCost = Cost[current] + node.Cost;
                float priority = newCost + Vector3.Distance(node.transform.position, end.transform.position);
                if (!Cost.ContainsKey(node))
                {
                    Cost.Add(node, newCost);
                    closeNodes.Add(node, priority);
                    reached.Add(node, current);
                }
                else if (Cost[node] > newCost)
                {
                    closeNodes.Add(node, priority);
                    reached[node] = current;
                    Cost[node] = newCost;
                }
            }
        }

        return new List<PathNode>();
    }
}
