using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(StatsController))]
public class EnemyBehaviour : MonoBehaviour
{
    public PlayerController player;
    private Animator animator;
    private StatsController stats;

    private System.DateTime lastAttack;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        lastAttack = System.DateTime.Now;
        stats = GetComponent<StatsController>();

        stats.onDeath.AddListener(Die);
    }

    private void Update()
    {
        bool isClose = Vector3.Distance(transform.position, player.transform.position) < 1f;
        animator.SetBool("Attacking", isClose);
        if (isClose && (System.DateTime.Now - lastAttack).TotalSeconds > 1)
        {
            player.GetComponent<StatsController>().Damage(1);
            lastAttack = System.DateTime.Now;
        }
    }

    private void Die()
    {
        Destroy(transform.parent.gameObject);
    }
}
