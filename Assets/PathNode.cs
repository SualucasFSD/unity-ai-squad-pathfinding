using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    [SerializeField] private List<PathNode> _neighbords = new List<PathNode>();
    [SerializeField] private int _cost=1;
    public int Cost { get { return _cost; } }
    public List<PathNode> Neighbords {  get { return _neighbords; } }
   /* private void Awake()
    {
        Manager.Instance.pathNodes.Add(this);
    }*/
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach( PathNode node in _neighbords ) { Gizmos.DrawLine(node.transform.position,transform.position); } 
    }
}
