using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Pj : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 _dir=Vector3.zero;
    [SerializeField] private float _speed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        _dir.x=Input.GetAxisRaw("Horizontal");
        _dir.z = Input.GetAxisRaw("Vertical");
        if(_dir.sqrMagnitude > 0 )
        {
            _dir.Normalize();
            //rb.MovePosition(transform.position+ _dir*Time.fixedDeltaTime*_speed);
            rb.position += _dir * _speed * Time.fixedDeltaTime;
        }
    }
}
