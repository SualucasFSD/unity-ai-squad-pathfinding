using UnityEngine;

public class IaEnemy : MonoBehaviour, IDamageable
{
    private FsmFinal _fsm;
    [SerializeField] private float _escapeVel = 15;
    [SerializeField] private float _fallowVel = 12;
    [SerializeField] private float _rotForce = 0.05f;
    [SerializeField] private PathNode[] _baseNode;
    [SerializeField][Range(0, 100)] private float _life = 100;
    [SerializeField] private LayerMask _layerToAvoid;
    [SerializeField] private GameObject _bullet;
    [SerializeField][Range(1,5)]private float _shootCadence=4;
    public float Life { get => _life; set => _life = value; }
    private enum BaseIndex
    {
        Green,
        Blue
    }
    [SerializeField] BaseIndex _baseIndex; 
    void Start()
    {
        _fsm = new FsmFinal();
        _fsm.AddState(FsmFinal.AgentStates.Fallow, new FallowState((int)_baseIndex,transform,_fallowVel,_fsm,_rotForce,this,_layerToAvoid));
        _fsm.AddState(FsmFinal.AgentStates.Healer, new HealingState(transform,(int)_baseIndex,_escapeVel,_rotForce,_fsm,this,_layerToAvoid));
        _fsm.AddState(FsmFinal.AgentStates.Escaping, new EscapingState(transform,_escapeVel,_fsm,_rotForce,this,_baseNode,(int)_baseIndex,_layerToAvoid));
        _fsm.AddState(FsmFinal.AgentStates.Attacking, new AttackState(this,_fsm,transform,(int)_baseIndex,_fallowVel,_rotForce,_layerToAvoid,_shootCadence));
        _fsm.ChangeState(FsmFinal.AgentStates.Fallow);
        EventManager.Suscribe(EventManager.KindOfEvent.OnDetect, OnDetect);
    }
    void Update()
    {
        _fsm.ArtificialUpdate();
    }
    private void OnDetect(object[] obj)
    {
        //_fsm.ChangeState(FsmFinal.AgentStates.Pursuit);

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position,transform.forward * 20);
    }
    public void Shoot(int i)
    {
        BulletMove p= Instantiate(_bullet, transform.position, transform.rotation).GetComponent<BulletMove>();
        p.LayerNoHit = i;
    }
    public void TakeDamage(float dmg)
    {
        Life -= dmg;
        if (_life <= 0)
        {
            ManagerFinal.Instance.Agents.Remove(transform);
            Destroy(gameObject);
        }
    }
}
