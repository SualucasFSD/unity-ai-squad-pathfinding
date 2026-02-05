using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
   public static Manager Instance;
   public List<PathNode> pathNodes = new List<PathNode>();
   public Pj Player;
    [SerializeField] private LayerMask _blockLayer;
    private void Awake()
    {
        Instance = this;
    }

    public bool FieldOfView(GameObject Caster, GameObject Target,float AreaOfVision, float Distance)
    {
        //if(backFrontAngle >= AreaOfVision&& Vector3.Distance(Target.transform.position,Caster.transform.position)<Distance)
        if(Vector3.Distance(Target.transform.position, Caster.transform.position) < Distance)
        {
            float backFrontAngle = Vector3.Dot(Caster.transform.forward, (Target.transform.position - Caster.transform.position).normalized);
            if (backFrontAngle > AreaOfVision) { return LineOfSight(Caster.transform.position,Target.transform.position); }
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
    public PathNode PjCloseNode()
    {
       return GetCloseNode(Player.transform);
    }
    public PathNode GetCloseNode(Transform _Obj)
    {
        float Distance = Mathf.Infinity;
        PathNode CloseNode = null;
        Vector3 dir;
        foreach(PathNode node in pathNodes)
        {
            dir= node.transform.position-_Obj.transform.position;
            if (dir.magnitude < Distance)
            {
                if (LineOfSight(node.transform.position, _Obj.position))
                {
                    CloseNode = node;
                    Distance = dir.magnitude;
                }
            }
        }
        return CloseNode;
    }
    public PathNode GetPjCloseNode()
    {
        return GetCloseNode(Player.transform);
    }
}
