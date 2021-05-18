using Spells;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StatsController))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    public enum AttackType
    {
        Melee,
        Ranged
    };
    public AttackType type = AttackType.Melee;
    private float findRange = 20f;

    [Header("Melee")]
    public float hitRange = 1.5f;

    [Header("Ranged")]
    public GameObject projectile;
    public float cooldown = 5f;
    private float timeSinceLast = 0;
    private bool wasAttacking = false;
    public float projectileSpeed = 3f;
    public float projectileDamage = 1f;

    [Header("Setup")]
    private Transform player;
    public Transform goldPrefab;

    private Animator animator;
    private StatsController stats;
    private NavMeshAgent ag;
    private Transform target;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        animator = transform.GetComponentInChildren<Animator>();
        stats = GetComponent<StatsController>();

        stats.onDeath.AddListener(Die);

        ag = GetComponent<NavMeshAgent>();
        ag.updateRotation = false;
        ag.updateUpAxis = false;

        animator = GetComponentInChildren<Animator>();
        stats = GetComponent<StatsController>();
        target = player;
    }

    private void Update()
    {
        if (type == AttackType.Ranged && wasAttacking && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            var orb = new CastOrb("projectile", stats, projectile, player.position, projectileDamage);
            orb.moveSpeed = projectileSpeed;
            orb.cast();
        }
        wasAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");

        // Movement
        if (Vector3.Distance(transform.position, target.position) < findRange)
        {
            if (type != AttackType.Ranged || !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                ag.SetDestination(new Vector3(target.position.x, target.position.y, transform.position.z));
                ag.speed = stats.GetSpeed();
            }
            else
            {
                ag.SetDestination(transform.position);
                ag.speed = 0f;
            }
        }

        bool isClose = Vector3.Distance(transform.position, player.position) < (type == AttackType.Melee ? hitRange : findRange);
        if (timeSinceLast >= 0)
        {
            timeSinceLast -= Time.deltaTime;
        }
        if (isClose)
        {
            if (type == AttackType.Ranged)
            {
                if (timeSinceLast <= 0)
                {
                    // cast orb but not here, after attack animation ended
                    animator.SetBool("Attacking", true);
                    timeSinceLast = cooldown;
                }
                else
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    {
                        animator.SetBool("Attacking", false);
                    }
                }

            }
            else if (type == AttackType.Melee)
            {
                animator.SetBool("Attacking", isClose);
            }
        }
        else if (type == AttackType.Melee)
        {
            animator.SetBool("Attacking", false);
        }

        animator.SetBool("Moving", ag.velocity.x != 0 || ag.velocity.y != 0);
        bool sign = ag.velocity.x == 0 ? transform.eulerAngles.y == 180 || (player.position.x < transform.position.x) : ag.velocity.x < 0;
        transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, sign ? 180 : 0, transform.eulerAngles.z));
    }

    private void Die()
    {
        Instantiate(goldPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
