using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    NavMeshAgent ag;
    public Transform target;

    private Animator animator;
    private SpriteRenderer rd;

    void Start()
    {
        ag = GetComponent<NavMeshAgent>();
        ag.updateRotation = false;
        ag.updateUpAxis = false;

        var realObject = transform.GetChild(0).gameObject;
        animator = realObject.GetComponent<Animator>();
        rd = realObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        ag.SetDestination(new Vector3(target.position.x, target.position.y, -1));

        animator.SetBool("Moving", ag.velocity.x != 0 || ag.velocity.y != 0);
        rd.flipX = ag.velocity.x == 0 ? rd.flipX : ag.velocity.x < 0;
    }
}
