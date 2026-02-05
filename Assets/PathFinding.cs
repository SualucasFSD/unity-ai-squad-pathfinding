using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public static PathFinding Instance;
    
    //List<PathNode> path = new List<PathNode>();
    private void Awake()
    {
        Instance = this;
    }
    public List<PathNode> Theta(PathNode begin, PathNode end,Transform obj)
    {
        List<PathNode> path= AStar(begin, end);
        int current = 0;
        while (current+2 < path.Count)
        {
            if (Manager.Instance.LineOfSight(path[current].transform.position, path[current+2].transform.position))
            {
                path.RemoveAt(current+1);
            }
            else
            {
                current++; 
            }
        }
        if(path.Count>=2)
        {
            if (Manager.Instance.LineOfSight(obj.position, path[1].transform.position))
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
                float priority=newCost +Vector3.Distance(node.transform.position,end.transform.position);
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
    #region Metodos No Utilizados
    //Analiza caminos, el primero que encuentra lo sigue sin mas.
    public List<PathNode> Bfs(PathNode begin, PathNode end)
    {
        Queue<PathNode> closeNodes = new Queue<PathNode>();
        closeNodes.Enqueue(begin);
        Dictionary<PathNode, PathNode> reached = new Dictionary<PathNode, PathNode>();
        while (closeNodes.Count > 0)
        {
            PathNode current = closeNodes.Dequeue();
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
                if (!reached.ContainsKey(node)) //if (!reached.ContainsKey(node)&&!node.BLock)
                {
                    closeNodes.Enqueue(node);
                    reached.Add(node, current);
                }
            }
        }
        return new List<PathNode>();
    }
    //Analiza caminos y elige el mas barato en termino de costos, simplemente bordea por el camino que menos le cueste.
    public List<PathNode> Djikstra(PathNode begin, PathNode end)
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
                if (!Cost.ContainsKey(node))
                {
                    Cost.Add(node, newCost);
                    closeNodes.Add(node, newCost);
                    reached.Add(node, current);
                }
                else if (Cost[node] > newCost)
                {
                    closeNodes.Add(node, newCost);
                    reached[node] = current;
                    Cost[node] = newCost;
                }
            }
        }

        return new List<PathNode>();
    }
    //Similar a Dijkstra, la diferencia es que no chequea el nodo mas barato sino el nodo mas cercano al objetivo. (Este me parece el peor)
    public List<PathNode> GreedyBfs(PathNode begin, PathNode end)
    {
        OwnList<PathNode> closeNodes = new OwnList<PathNode>();
        closeNodes.Add(begin, 0);
        Dictionary<PathNode, PathNode> reached = new Dictionary<PathNode, PathNode>();


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
                if (!reached.ContainsKey(node))
                {
                    Vector3 dir = end.transform.position - node.transform.position;
                    float priority = dir.magnitude;
                    closeNodes.Add(node, priority);
                    reached.Add(node, current);
                }
            }
        }

        return new List<PathNode>();
    }
    #endregion
}
