using System.Collections.Generic;
using UnityEngine;

public class Comandante : MonoBehaviour, IDamageable
{
    public Vector3 _dir;
    private Vector3 _movementDir;
    [SerializeField] private float _pursuitVel = 12;
    [SerializeField] private float _rotForce = 0.05f;
    public GameObject point;
    [SerializeField] private LayerMask _layerToAvoid;
    private List<PathNode> _path = new List<PathNode>();
    [Range(0,500)]public float Life;
    public bool suicide = false;
    [SerializeField] private GameObject _bullet;
    private FsmFinal _fsm;
    private void Start()
    {
       _fsm=new FsmFinal();
        _fsm.AddState(FsmFinal.AgentStates.AttackCo,new ComanStateAttack());
        _fsm.AddState(FsmFinal.AgentStates.Go,new ComandStateGo(transform,_pursuitVel,_rotForce,_layerToAvoid,this));
        //_fsm.ChangeState(FsmFinal.AgentStates.AttackCo);
    }
    private void Update()
    {
        if (suicide) { TakeDamage(500); }
        if(ManagerFinal.Instance.LineOfSight(transform.position,point.transform.position))
        {
            AddForce(Arrive(point.transform) + ObstacleAvoid() + ObstacleAvoid());
            _movementDir.y = 0;
            _movementDir.x = _dir.x;
            _movementDir.z = _dir.z;
            transform.position += _movementDir * Time.deltaTime;
            if (_movementDir != Vector3.zero)
            {
                transform.forward = _movementDir;
            }
            return;
        }
       if (_path.Count > 0 && _path!=null)
        {
            if (Vector3.Distance(_path[0].transform.position, transform.position) > 1.5f && _path.Count > 0)
            {
                AddForce(Seek(_path[0].transform)+ObstacleAvoid());
            }
            else
            {
                _path.RemoveAt(0);
            }
            _movementDir.y = 0;
            _movementDir.x = _dir.x;
            _movementDir.z = _dir.z;
            transform.position += _movementDir * Time.deltaTime;
            if (_dir != Vector3.zero)
            {
                transform.forward = _movementDir;
            }
        }
    }
    private Vector3 ObstacleAvoid()
    {
        Vector3 position = transform.position;
        Vector3 direcction = transform.forward;
        float distance = _dir.magnitude;
        if (Physics.SphereCast(position, 2f, direcction, out RaycastHit hit, distance, _layerToAvoid))
        {
            Transform obstacle = hit.transform;
            Vector3 dirToObj = obstacle.position - position;
            float angleBtw = Vector3.SignedAngle(transform.forward, dirToObj, Vector3.up);
            Vector3 desired = angleBtw >= 0 ? -transform.right : transform.right;
            desired.Normalize();
            desired *= _pursuitVel;
            Vector3 steering = Vector3.ClampMagnitude(desired - _dir, _rotForce);
            return steering;
        }
        return Vector3.zero;
    }
    private Vector3 Arrive(Transform target)
    {
        Vector3 DesirePosition;
        if (Vector3.Distance(target.position, transform.position) > 7f)
        {
            return Seek(target);
        }
        else
        {
            DesirePosition = target.position - transform.position;
            DesirePosition.Normalize();
            DesirePosition *= _pursuitVel * 0.5f;
            Vector3 Steering;
            Steering = DesirePosition - _dir;
            Steering = Vector3.ClampMagnitude(Steering, _rotForce);
            return Steering;
        }
    }
    private Vector3 Seek(Transform Tg)
    {
        Vector3 DesirePosition = Tg.position - transform.position;
        DesirePosition.Normalize();
        DesirePosition *= _pursuitVel;
        Vector3 Steering;
        Steering = DesirePosition - _dir;
        Steering = Vector3.ClampMagnitude(Steering, _rotForce);
        return Steering;
    }
    public void AddForce(Vector3 target)
    {
        _dir = Vector3.ClampMagnitude(_dir + target, _pursuitVel);
    }
    public void GetPath()
    {
        //_fsm.ChangeState(FsmFinal.AgentStates.Go);
        _path = PathFindingFinal.Instance.Theta(ManagerFinal.Instance.GetCloseNode(transform), ManagerFinal.Instance.GetCloseNode(point.transform),transform);
    }

    public void TakeDamage(float dmg)
    {
        Life -= dmg;
        if (Life <= 0) { ManagerFinal.Instance.Agents.Remove(gameObject.transform); Destroy(gameObject); }
    }
}
