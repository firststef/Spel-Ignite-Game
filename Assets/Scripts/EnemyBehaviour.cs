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
            player.DamagePlayer();
            lastAttack = System.DateTime.Now;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.StartsWith("Fire"))
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
