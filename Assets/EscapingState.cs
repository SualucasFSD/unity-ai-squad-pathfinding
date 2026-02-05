using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EscapingState : IState
{
    #region Variables
    private FsmFinal _fsm;
    private Vector3 _dir;
    private Transform _transform;
    private float _vel;
    private float _rotVel;
    private Vector3 _movementDir;
    private List<PathNode> _path = new List<PathNode>();
    private IaEnemy _ia;
    private PathNode[] _BaseNode;
    private int _index;
    private int _team;
    private PathNode _nodeToRun;
    private LayerMask _avoidLayer;
    public EscapingState(Transform transform, float speed, FsmFinal fsm, float rotForce, IaEnemy ia, PathNode[] BaseNode, int index, LayerMask avoidLayer)
    {
        _transform = transform;
        _vel = speed;
        _fsm = fsm;
        _rotVel = rotForce;
        _ia = ia;
        _BaseNode = BaseNode;
        _index = index;
        _avoidLayer = avoidLayer;
    }
    #endregion
    public void OnEnter()
    {
        _nodeToRun = null;
        if (_index == 0)
        {
            _team = 8;
        }
        else if( _index == 1)
        {
            _team = 9;
        }
        List<PathNode> PosibleNodes = new List<PathNode>();
        foreach (var node in _BaseNode)
        {
            foreach(Transform agent in ManagerFinal.Instance.Agents)
            {
                if(agent.gameObject.layer == _team) { continue; }
                if (ManagerFinal.Instance.LineOfSight(agent.transform.position,node.transform.position)){ continue; }
                else
                {
                    PosibleNodes.Add(node);
                }
            }
        }
        _nodeToRun = PosibleNodes.ElementAt(Random.Range(0, PosibleNodes.Count));
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
        if (_nodeToRun != null)
        {
            _path = PathFindingFinal.Instance.Theta(ManagerFinal.Instance.GetCloseNode(_transform), _nodeToRun, _transform);
        }
        if (Vector3.Distance(_transform.position, _nodeToRun.transform.position) <= 5)
        {
            AddForce(Separation(ManagerFinal.Instance.Agents, ManagerFinal.Instance.RadiousSeparate));
            _movementDir.y = 0;
            _movementDir.x = _dir.x;
            _movementDir.z = _dir.z;
            _transform.position += _movementDir * Time.deltaTime;
            if (_dir != Vector3.zero)
            {
                _transform.forward = _movementDir;
            }
            return;
        }
            if (_nodeToRun != null)
        {
            _path = PathFindingFinal.Instance.Theta(ManagerFinal.Instance.GetCloseNode(_transform), _nodeToRun, _transform);
        }
        if (_path != null && _path.Count > 0)
        {
            if (Vector3.Distance(_path[0].transform.position, _transform.position) > 1.2f)
            {
                AddForce(Arrive(_path[0].transform) * ManagerFinal.Instance.ArrivePriority + Separation(ManagerFinal.Instance.Agents, ManagerFinal.Instance.RadiousSeparate) * ManagerFinal.Instance.RadiousAvoid + ObstacleAvoid());
            }
            else { _path.RemoveAt(0); }
            _movementDir.y = 0;
            _movementDir.x = _dir.x;
            _movementDir.z = _dir.z;
            _transform.position += _movementDir * Time.deltaTime;
            if (_dir != Vector3.zero)
            {
                _transform.forward = _movementDir;
            }
        }
        if (ManagerFinal.Instance.Coman[_index] == null)
        { 
            return;
        }
        if (_path.Count<=0) { _fsm.ChangeState(FsmFinal.AgentStates.Attacking); }
    }
    private Vector3 Separation(List<Transform> p, float radius)
    {
        Vector3 dir;
        Vector3 desired = Vector3.zero;
        Vector3 steering;
        foreach (Transform t in p)
        {
            if (t.gameObject == _ia.gameObject)
            {
                continue;
            }
            dir = t.position - _transform.position;
            if (dir.magnitude > radius)
            {
                continue;
            }
            desired -= dir;
        }
        if (desired == Vector3.zero)
        {
            return desired;
        }
        desired.Normalize();
        desired *= _vel;
        steering = desired - _dir;
        steering = Vector3.ClampMagnitude(steering, _rotVel);
        return steering;
    }
    private Vector3 ObstacleAvoid()
    {
        Vector3 position = _transform.position;
        Vector3 direcction = _transform.forward;
        float distance = _dir.magnitude;
        if (Physics.SphereCast(position, 2f, direcction, out RaycastHit hit, distance, _avoidLayer))
        {
            Transform obstacle = hit.transform;
            Vector3 dirToObj = obstacle.position - position;
            float angleBtw = Vector3.SignedAngle(_transform.forward, dirToObj, Vector3.up);
            Vector3 desired = angleBtw >= 0 ? -_transform.right : _transform.right;
            desired.Normalize();
            desired *= _vel;
            Vector3 steering = Vector3.ClampMagnitude(desired - _dir, _rotVel);
            return steering;
        }
        return Vector3.zero;
    }
    public Vector3 Arrive(Transform target)
    {
        Vector3 DesirePosition;
        if (Vector3.Distance(target.position, _transform.position) > 10f)
        {
            return Seek(target);
        }
        else
        {
            DesirePosition = target.position - _transform.position;
            DesirePosition.Normalize();
            DesirePosition *= _vel * 0.5f;
            Vector3 Steering;
            Steering = DesirePosition - _dir;
            Steering = Vector3.ClampMagnitude(Steering, _rotVel);
            return Steering;
        }
    }
    public Vector3 Seek(Transform Tg)
    {
        Vector3 DesirePosition = Tg.position - _transform.position;
        DesirePosition.Normalize();
        DesirePosition *= _vel;
        Vector3 Steering;
        Steering = DesirePosition - _dir;
        Steering = Vector3.ClampMagnitude(Steering, _rotVel);
        return Steering;
    }
    private void AddForce(Vector3 target)
    {
        if (target.magnitude == 0)
        { _dir = Vector3.zero; return; }
        _dir = Vector3.ClampMagnitude(_dir + target, _vel);
    }
}
