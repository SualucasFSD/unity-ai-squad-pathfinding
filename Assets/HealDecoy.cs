using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealDecoy : MonoBehaviour, IDamageable
{
    private float _time;
    [SerializeField] private float _life = 1000;
    public void TakeDamage(float dmg)
    {
        _life -= dmg;
        if(_life <= 0 )
        {
            ManagerFinal.Instance.Agents.Remove(transform);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ManagerFinal.Instance.Agents.Add(transform);
    }
    void Update()
    {
        _time += Time.deltaTime;
        if (_time >= 2)
        {
            foreach (Transform t in ManagerFinal.Instance.Agents)
            {
                if (t.gameObject.layer!=gameObject.layer) { continue; }
                if (t.gameObject == this) { continue; }

                if (Vector3.Distance(transform.position, t.position) < 15)
                {
                    if (t.GetComponent<IaEnemy>() != null)
                    {
                        t.GetComponent<IaEnemy>().Life += 20;
                    }
                    else if (t.GetComponent<Comandante>() != null)
                    {
                        t.GetComponent<Comandante>().Life += 50;
                    }
                }
            }
            _time = 0;
        }
    }
}
