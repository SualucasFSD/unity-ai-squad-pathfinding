using System.Collections.Generic;
using UnityEngine;

public class FallowState : IState
{
    #region Variables.
    private FsmFinal _fsm;
    private int _baseIndex;
    private Vector3 _dir;
    private Transform _transform;
    private float _speed;
    private float _rotForce;
    private Vector3 _movementDir;
    private List<PathNode> _path = new List<PathNode>();
    private IaEnemy _ia;
    private LayerMask _avoidLayer;
    private int _team;
    public FallowState(int baseIndex,Transform transform, float speed,FsmFinal fsm, float rotForce,IaEnemy ia,LayerMask avoidLayer)
    {
        _baseIndex = baseIndex;
        _transform = transform;
        _speed = speed;
        _fsm = fsm;
        _rotForce = rotForce;
        _ia = ia;
        _avoidLayer = avoidLayer;
    }
    #endregion
    public void OnEnter()
    {
        if (_baseIndex == 0)
        {
            _team = 8;
        }
        else if (_baseIndex == 1)
        {
            _team = 9;
        }
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        #region Cambios de estado
        if (ManagerFinal.Instance.Coman[_baseIndex] == null)
        {
            _path=null;
            _fsm.ChangeState(FsmFinal.AgentStates.Healer);
            return;
        }
        if (_ia.Life <= 30 && ManagerFinal.Instance.Base[_baseIndex] != null)
        {
            _fsm.ChangeState(FsmFinal.AgentStates.Healer);
        }
        else if (_ia.Life <= 30 && ManagerFinal.Instance.Base[_baseIndex] == null)
        {
            _fsm.ChangeState(FsmFinal.AgentStates.Escaping);
        }
        foreach (Transform agent in ManagerFinal.Instance.Agents)
        {
            if (agent.gameObject.layer == _team) { continue; }
            if (ManagerFinal.Instance.FieldOfView(_transform.gameObject, agent.gameObject, 0.55f, 20)&& Vector3.Distance(_transform.position, ManagerFinal.Instance.Coman[_baseIndex].transform.position) < 30)
            {
              _fsm.ChangeState(FsmFinal.AgentStates.Attacking);
            }
        }
        #endregion

        if (Vector3.Distance(_transform.position, ManagerFinal.Instance.Coman[_baseIndex].transform.position) <= 8f)
        {
            AddForce(Separation(ManagerFinal.Instance.Agents, ManagerFinal.Instance.RadiousSeparate) +ObstacleAvoid());
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
        //Si esta a la vista del comandante va directo hacia el.
        if (ManagerFinal.Instance.LineOfSight(_transform.position, ManagerFinal.Instance.Coman[_baseIndex].transform.position))
        {
            AddForce(Arrive(ManagerFinal.Instance.Coman[_baseIndex].transform) * ManagerFinal.Instance.ArrivePriority + Separation(ManagerFinal.Instance.Agents, ManagerFinal.Instance.RadiousSeparate) * ManagerFinal.Instance.RadiousAvoid + ObstacleAvoid());
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
        //Si no tiene a la vista al comandante utiliza pathfinding
        if (Vector3.Distance(_transform.position, ManagerFinal.Instance.Coman[_baseIndex].transform.position)>4f)
        {
            _path = PathFindingFinal.Instance.Theta(ManagerFinal.Instance.GetCloseNode(_transform), ManagerFinal.Instance.GetCloseNode(ManagerFinal.Instance.Coman[_baseIndex].transform), _transform);
        }
        if (_path != null && _path.Count > 0)
        {
            if (Vector3.Distance(_path[0].transform.position, _transform.position) > 5f)
            {
                AddForce(Arrive(_path[0].transform)*ManagerFinal.Instance.ArrivePriority +Separation(ManagerFinal.Instance.Agents, ManagerFinal.Instance.RadiousSeparate) *ManagerFinal.Instance.RadiousAvoid + ObstacleAvoid());
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
    }
    private Vector3 ObstacleAvoid()
    {
        Vector3 position=_transform.position;
        Vector3 direcction = _transform.forward;
        float distance=_dir.magnitude;
        if(Physics.SphereCast(position,2f,direcction,out RaycastHit hit,distance,_avoidLayer))
        {
            Transform obstacle=hit.transform;
            Vector3 dirToObj=obstacle.position-position;
            float angleBtw = Vector3.SignedAngle(_transform.forward, dirToObj, Vector3.up);
            Vector3 desired= angleBtw>=0?-_transform.right : _transform.right;
            desired.Normalize();
            desired *= _speed;
            Vector3 steering= Vector3.ClampMagnitude(desired-_dir,_rotForce);
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
            DesirePosition *= _speed * 0.5f;
            Vector3 Steering;
            Steering = DesirePosition - _dir;
            Steering = Vector3.ClampMagnitude(Steering, _rotForce);
            return Steering;
        }
    }
    public Vector3 Seek(Transform Tg)
    {
        Vector3 DesirePosition = Tg.position - _transform.position;
        DesirePosition.Normalize();
        DesirePosition *= _speed;
        Vector3 Steering;
        Steering = DesirePosition - _dir;
        Steering = Vector3.ClampMagnitude(Steering, _rotForce);
        return Steering;
    }
    private Vector3 Separation(List<Transform> p, float radius)
    {
        Vector3 dir;
        Vector3 desired = Vector3.zero;
        Vector3 steering;
        foreach(Transform t in p)
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
        desired *= _speed;
        steering = desired - _dir;
        steering = Vector3.ClampMagnitude(steering, _rotForce);
        return steering;
    }
    private void AddForce(Vector3 target)
    {
        if(target.magnitude == 0)
        { _dir = Vector3.zero; return; }
        _dir = Vector3.ClampMagnitude(_dir + target, _speed);
    }
}
