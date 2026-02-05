using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ComandStateGo : IState
{
    private Transform _transform;
    private float _pursuitVel;
    private List<PathNode> _path = new List<PathNode>();
    private float _rotForce;
    private Vector3 _movementDir;
    private LayerMask _layerToAvoid;
    private Comandante co;
    public ComandStateGo(Transform transform,float pursuitVel,float rotForce,LayerMask Layer, Comandante com)
    {
        _transform = transform;
        _pursuitVel = pursuitVel;
        _rotForce = rotForce;
        _layerToAvoid = Layer;
        co = com;
    }

    public void OnEnter()
    {
        
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        if (ManagerFinal.Instance.LineOfSight(_transform.position, co.point.transform.position))
        {
            co.AddForce(Arrive(co.point.transform) + ObstacleAvoid() + ObstacleAvoid());
            _movementDir.y = 0;
            _movementDir.x = co._dir.x;
            _movementDir.z = co._dir.z;
            _transform.position += _movementDir * Time.deltaTime;
            if (_movementDir != Vector3.zero)
            {
                _transform.forward = _movementDir;
            }
            return;
        }
        if (_path.Count > 0 && _path != null)
        {
            if (Vector3.Distance(_path[0].transform.position, _transform.position) > 1.5f && _path.Count > 0)
            {
               co.AddForce(Seek(_path[0].transform) + ObstacleAvoid());
            }
            else
            {
                _path.RemoveAt(0);
            }
            _movementDir.y = 0;
            _movementDir.x = co._dir.x;
            _movementDir.z = co._dir.z;
            _transform.position += _movementDir * Time.deltaTime;
            if (co._dir != Vector3.zero)
            {
                _transform.forward = _movementDir;
            }
        }
    }
    private Vector3 ObstacleAvoid()
    {
        Vector3 position = _transform.position;
        Vector3 direcction = _transform.forward;
        float distance = co._dir.magnitude;
        if (Physics.SphereCast(position, 2f, direcction, out RaycastHit hit, distance, _layerToAvoid))
        {
            Transform obstacle = hit.transform;
            Vector3 dirToObj = obstacle.position - position;
            float angleBtw = Vector3.SignedAngle(_transform.forward, dirToObj, Vector3.up);
            Vector3 desired = angleBtw >= 0 ? -_transform.right : _transform.right;
            desired.Normalize();
            desired *= _pursuitVel;
            Vector3 steering = Vector3.ClampMagnitude(desired - co._dir, _rotForce);
            return steering;
        }
        return Vector3.zero;
    }
    private Vector3 Arrive(Transform target)
    {
        Vector3 DesirePosition;
        if (Vector3.Distance(target.position, _transform.position) > 7f)
        {
            return Seek(target);
        }
        else
        {
            DesirePosition = target.position - _transform.position;
            DesirePosition.Normalize();
            DesirePosition *= _pursuitVel * 0.5f;
            Vector3 Steering;
            Steering = DesirePosition - co._dir;
            Steering = Vector3.ClampMagnitude(Steering, _rotForce);
            return Steering;
        }
    }
    private Vector3 Seek(Transform Tg)
    {
        Vector3 DesirePosition = Tg.position - _transform.position;
        DesirePosition.Normalize();
        DesirePosition *= _pursuitVel;
        Vector3 Steering;
        Steering = DesirePosition - co._dir;
        Steering = Vector3.ClampMagnitude(Steering, _rotForce);
        return Steering;
    }
    public void GetPath()
    {
        _path = PathFindingFinal.Instance.Theta(ManagerFinal.Instance.GetCloseNode(_transform), ManagerFinal.Instance.GetCloseNode(co.point.transform), _transform);
    }
}
