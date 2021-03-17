using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class GenericMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer renderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        animator.SetBool("Moving", rb.velocity.x != 0 || rb.velocity.y != 0);
        renderer.flipX = rb.velocity.x == 0 ? renderer.flipX : rb.velocity.x < 0;
    }
}
