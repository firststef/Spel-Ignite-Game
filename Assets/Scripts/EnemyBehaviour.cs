using UnityEngine;

[RequireComponent(typeof(StatsController))]
public class EnemyBehaviour : MonoBehaviour
{
    public PlayerController player;
    public Transform goldPrefab;
    private Animator animator;
    private StatsController stats;

    private void Awake()
    {
        animator = transform.GetComponentInChildren<Animator>();
        stats = GetComponent<StatsController>();

        stats.onDeath.AddListener(Die);
    }

    private void Update()
    {
        bool isClose = Vector3.Distance(transform.position, player.transform.position) < 1.5f;
        animator.SetBool("Attacking", isClose);
    }

    private void Die()
    {
        Instantiate(goldPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
