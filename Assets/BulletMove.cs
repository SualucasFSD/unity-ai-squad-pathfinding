using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class BulletMove : MonoBehaviour
{
    public int LayerNoHit=8;
    private Rigidbody _rb;
    [SerializeField] private float _dmg;
    [SerializeField] private float _speed;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.constraints= RigidbodyConstraints.FreezeRotation;
    }
    private void OnTriggerEnter(Collider other)
    {
        IDamageable Damageable= other.GetComponent<IDamageable>();
        if(Damageable != null )
        {
            if(LayerNoHit==other.gameObject.layer)
            { return; }
            else
            {
                Damageable.TakeDamage(20);
                Destroy(gameObject);
            }
        }
        else { Destroy(gameObject); }
        
    }
    private void FixedUpdate()
    {
        _rb.position += transform.forward*_speed * Time.deltaTime;
    }
}
