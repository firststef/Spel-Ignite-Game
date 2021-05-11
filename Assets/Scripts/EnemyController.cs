using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StatsController))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    public PlayerController player;
    public Transform goldPrefab;
    private Animator animator;
    private StatsController stats;
    private SpriteRenderer rd;
    private NavMeshAgent ag;
    private Transform target;

    private void Awake()
    {
        animator = transform.GetComponentInChildren<Animator>();
        stats = GetComponent<StatsController>();

        stats.onDeath.AddListener(Die);

        ag = GetComponent<NavMeshAgent>();
        ag.updateRotation = false;
        ag.updateUpAxis = false;

        animator = GetComponentInChildren<Animator>();
        rd = GetComponentInChildren<SpriteRenderer>();
        stats = GetComponent<StatsController>();
        target = player.transform;
    }

    private void Update()
    {
        bool isClose = Vector3.Distance(transform.position, player.transform.position) < 1.5f;
        animator.SetBool("Attacking", isClose);

        // Movement
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        if (Vector3.Distance(transform.position, target.position) < 20f)
        {
            ag.SetDestination(new Vector3(target.position.x, target.position.y, -1));
            ag.speed = stats.GetSpeed();
        }

        animator.SetBool("Moving", ag.velocity.x != 0 || ag.velocity.y != 0);
        bool sign = ag.velocity.x == 0 ? transform.eulerAngles.y == 180 : ag.velocity.x < 0;
        transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, sign ? 180 : 0, transform.eulerAngles.z));
    }

    private void Die()
    {
        Instantiate(goldPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
