using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class GenericMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer rd;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rd = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        animator.SetBool("Moving", rb.velocity.x != 0 || rb.velocity.y != 0);
        rd.flipX = rb.velocity.x == 0 ? rd.flipX : rb.velocity.x < 0;
    }
}
