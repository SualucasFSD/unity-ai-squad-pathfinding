using System.Collections.Generic;
using UnityEngine;

public class HealingState : IState
{
    #region Variables
    private List<PathNode> _path = new List<PathNode>();
    private Transform _transform;
    private int _index;
    private float _vel;
    private float _rotVel;
    private Vector3 _dir=Vector3.zero;
    private Vector3 _movementDir;
    private FsmFinal _fsm;
    private IaEnemy _ia;
    private LayerMask _avoidLayer;
    public HealingState(Transform t,int i,float vel,float rotVel,FsmFinal fsm, IaEnemy ia, LayerMask avoidLayer)
    {
        _transform = t;
        _index = i;
        _vel =vel;
        _rotVel =rotVel;
        _fsm = fsm;
        _ia =ia;
        _avoidLayer =avoidLayer;
    }
    #endregion
    public void OnEnter()
    {
    }

    public void OnExit()
    {
      
    }

    public void OnUpdate()
    {
        if (ManagerFinal.Instance.Base[_index] == null)
        {
            _path = null;
            _fsm.ChangeState(FsmFinal.AgentStates.Escaping);
            return;
        }
        _path = PathFindingFinal.Instance.Theta(ManagerFinal.Instance.GetCloseNode(_transform), ManagerFinal.Instance.GetCloseNode(ManagerFinal.Instance.Base[_index]), _transform);
        if (Vector3.Distance(_transform.position, ManagerFinal.Instance.Base[_index].transform.position)<=5)
        {
            if (ManagerFinal.Instance.Coman[_index] == null)
            {
                _ia.Life = 31;
                _fsm.ChangeState(FsmFinal.AgentStates.Attacking);
            }

            AddForce(Separation(ManagerFinal.Instance.Agents, ManagerFinal.Instance.RadiousSeparate));
            _movementDir.y = 0;
            _movementDir.x = _dir.x;
            _movementDir.z = _dir.z;
            _transform.position += _movementDir * Time.deltaTime;
            if (_dir != Vector3.zero)
            {
                _transform.forward = _movementDir;
            }
            if (_ia.Life >= 100)
            {
                _fsm.ChangeState(FsmFinal.AgentStates.Fallow);
            }
            return;
        }    
       if(_path != null&& _path.Count>0)
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
    private Vector3 Separation(List<Transform> p, float radius)
    {
        Vector3 dir;
        Vector3 desired = Vector3.zero;
        Vector3 steering;
        // foreach (IaEnemy prey in p)
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
