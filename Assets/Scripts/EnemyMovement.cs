using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StatsController))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    NavMeshAgent ag;
    public Transform target;

    private Animator animator;
    private SpriteRenderer rd;
    private StatsController st;

    void Start()
    {
        ag = GetComponent<NavMeshAgent>();
        ag.updateRotation = false;
        ag.updateUpAxis = false;

        animator = GetComponentInChildren<Animator>();
        rd = GetComponentInChildren<SpriteRenderer>();
        st = GetComponent<StatsController>();
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        if (Vector3.Distance(transform.position, target.position) < 20f)
        {
            ag.SetDestination(new Vector3(target.position.x, target.position.y, -1));
            ag.speed = st.GetSpeed();
        }

        animator.SetBool("Moving", ag.velocity.x != 0 || ag.velocity.y != 0);
        rd.flipX = ag.velocity.x == 0 ? rd.flipX : ag.velocity.x < 0;
    }
}
