using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyBehaviour : MonoBehaviour
{
    public PlayerController player;
    private Animator animator;

    private System.DateTime lastAttack;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        lastAttack = System.DateTime.Now;
    }

    private void Update()
    {
        bool isClose = Vector3.Distance(transform.position, player.transform.position) < 2f;
        animator.SetBool("Attacking", isClose);
        if (isClose && (System.DateTime.Now - lastAttack).TotalSeconds > 1)
        {
            player.DamagePlayer(1);
            lastAttack = System.DateTime.Now;
        }
    }

    public void TakeDamage(string spell, int amount=0)
    {
        if (spell.StartsWith("Fire"))
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
