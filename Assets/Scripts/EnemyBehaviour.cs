using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{
    NavMeshAgent ag;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        ag = GetComponent<NavMeshAgent>();
        ag.updateRotation = false;
        ag.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        ag.SetDestination(new Vector3(target.position.x, target.position.y, -1));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.StartsWith("Fire"))
        {
            Destroy(gameObject);
        }
    }
}
