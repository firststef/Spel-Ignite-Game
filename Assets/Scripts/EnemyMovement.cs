using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    NavMeshAgent ag;
    public Transform target;

    void Start()
    {
        ag = GetComponent<NavMeshAgent>();
        ag.updateRotation = false;
        ag.updateUpAxis = false;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        ag.SetDestination(new Vector3(target.position.x, target.position.y, -1));
    }
}
