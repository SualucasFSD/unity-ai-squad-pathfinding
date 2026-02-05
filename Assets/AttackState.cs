using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private IaEnemy _ia;
    private FsmFinal _fsm;
    private Transform _transform;
    private int _index;
    private int _team;
    private Transform _obj;
    private float _distance;
    private float _speed;
    private float _rotVel;
    private LayerMask _avoidLayer;
    private Vector3 _dir;
    private Vector3 _movementDir;
    private float _shootCadence;
    private float _time=0;
    public AttackState(IaEnemy ia,FsmFinal fsm,Transform transform, int index,float speed,float rotSpeed, LayerMask avoidLayer,float shootCadence)
    {
        _ia = ia;
        _fsm = fsm;
        _transform = transform;
        _index = index;
        _speed = speed;
        _rotVel = rotSpeed;
        _avoidLayer = avoidLayer;
        _shootCadence = shootCadence;
    }
    public void OnEnter()
    {
        if (_index == 0)
        {
            _team = 8;
        }
        else if (_index == 1)
        {
            _team = 9;
        }
        _obj=null;
        _distance = Mathf.Infinity;
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        if (ManagerFinal.Instance.Coman[_index]==null && ManagerFinal.Instance.Base[_index]==null)
        {
            Debug.Log("Escaping");
            _fsm.ChangeState(FsmFinal.AgentStates.Escaping);
            return;
        }

        if (ManagerFinal.Instance.Coman[_index] == null)
        {
            if(_ia.Life>=30)
            {
                foreach (Transform agent in ManagerFinal.Instance.Agents)
                {
                    if (agent.gameObject.layer == _team) { continue; }
                    if (Vector3.Distance(_transform.position, agent.position) < _distance)
                    {
                        if (ManagerFinal.Instance.FieldOfView(_transform.gameObject, agent.gameObject, 0.55f, 20))
                        {
                            _obj = agent;
                            _distance = Vector3.Distance(_transform.position, agent.position);
                        }
                    }
                }
                if(_obj!=null)
                {
                    _distance = Vector3.Distance(_transform.position, _obj.position);
                    if(_distance<20)
                    {
                        _time += Time.deltaTime;
                        _transform.forward = (_obj.position - _transform.position).normalized;
                        AddForce(Separation(ManagerFinal.Instance.Agents, 5f));
                        _movementDir.y = 0;
                        _movementDir.x = _dir.x;
                        _movementDir.z = _dir.z;
                        _transform.position += _movementDir * Time.deltaTime;
                        if (_time >= _shootCadence)
                        {
                            _time = 0;
                            _ia.Shoot(_team);
                        }
                    }
                }
            }
            else if(_ia.Life<30) 
            {
               _fsm.ChangeState(FsmFinal.AgentStates.Healer);
            }
          return;
        }
        else if (_ia.Life < 30)
        {
            _fsm.ChangeState(FsmFinal.AgentStates.Healer);
        }
        foreach (Transform agent in ManagerFinal.Instance.Agents)
        {
            if (agent.gameObject.layer == _team) { continue; }
            if (Vector3.Distance(_transform.position, agent.position) < _distance)
            {
                if (ManagerFinal.Instance.FieldOfView(_transform.gameObject, agent.gameObject, 0.55f, 20))
                {
                    _obj = agent;
                    _distance = Vector3.Distance(_transform.position, agent.position);
                }
            }
        }
        if (_obj != null)
        {
            _transform.forward = (_obj.position - _transform.position).normalized;

            _distance = Vector3.Distance(_transform.position, _obj.position);
            float distCaptain = 0;
            distCaptain = Vector3.Distance(_transform.position, ManagerFinal.Instance.Coman[_index].transform.position);
            if (_distance > 25 || distCaptain > 30)
            {
                _fsm.ChangeState(FsmFinal.AgentStates.Fallow);
            }
            _transform.forward = (_obj.position - _transform.position).normalized;
            _time += Time.deltaTime;
            AddForce(Separation(ManagerFinal.Instance.Agents, 5f));
            _movementDir.y = 0;
            _movementDir.x = _dir.x;
            _movementDir.z = _dir.z;
            _transform.position += _movementDir * Time.deltaTime;
            if (_time >= _shootCadence)
            {
                _time = 0;
                _ia.Shoot(_team);
            }
        }
        else { _fsm.ChangeState(FsmFinal.AgentStates.Fallow); }
    }
    private Vector3 Separation(List<Transform> p, float radius)
    {
        Vector3 dir=Vector3.zero;
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
        desired *= _speed;
        steering = desired - _dir;
        steering = Vector3.ClampMagnitude(steering, _rotVel);
        return steering;
    }
    private void AddForce(Vector3 target)
    {
        if (target.magnitude == 0)
        { _dir = Vector3.zero; return; }
        _dir = Vector3.ClampMagnitude(_dir + target, _speed);
    }
}
