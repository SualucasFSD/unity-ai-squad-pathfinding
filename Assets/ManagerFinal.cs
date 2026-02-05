using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerFinal : MonoBehaviour
{
    public static ManagerFinal Instance;
    public List<PathNode> pathNodes = new List<PathNode>();
    public Comandante[] Coman= new Comandante[2];
    [SerializeField] private LayerMask _blockLayer;
    public Transform[] Base=new Transform[2];
    public List<Transform> Agents= new List<Transform>();
    [Range(0,1)] public float RadiousAvoid;
    [Range(0, 1)] public float ArrivePriority;
    [Range(1,10)] public float RadiousSeparate;
    private void Awake()
    {
        Instance = this;
    }

    public bool FieldOfView(GameObject Caster, GameObject Target, float AreaOfVision, float Distance)
    {
        //if(backFrontAngle >= AreaOfVision&& Vector3.Distance(Target.transform.position,Caster.transform.position)<Distance)
        if (Vector3.Distance(Target.transform.position, Caster.transform.position) < Distance)
        {
            float backFrontAngle = Vector3.Dot(Caster.transform.forward, (Target.transform.position - Caster.transform.position).normalized);
            if (backFrontAngle > AreaOfVision) { return LineOfSight(Caster.transform.position, Target.transform.position); }
            else { return false; }
        }
        else
        {
            return false;
        }
    }
    public bool LineOfSight(Vector3 obj, Vector3 tg)
    {
        Vector3 Dir = tg - obj;
        return !Physics.Raycast(obj, Dir, Dir.magnitude, _blockLayer);
    }
    public PathNode GetCloseNode(Transform _Obj)
    {
        float Distance = Mathf.Infinity;
        PathNode CloseNode = null;
        Vector3 dir;
        foreach (PathNode node in pathNodes)
        {
            dir = node.transform.position - _Obj.transform.position;
            if (dir.magnitude < Distance)
            {
                if (LineOfSight(node.transform.position, _Obj.position))
                {
                    CloseNode = node;
                    Distance = dir.magnitude;
                }
            }
        }
        if (CloseNode == null)
        {
            Distance = Mathf.Infinity;
            foreach (PathNode node in pathNodes)
            {
                dir = node.transform.position - _Obj.transform.position;
                if (dir.magnitude < Distance)
                {
                    CloseNode = node;
                    Distance = dir.magnitude;
                }
            }
        }
        return CloseNode;
    }
    public PathNode GetPjCloseNode(int i)
    {
        return GetCloseNode(Coman[i].transform);
    }
}
