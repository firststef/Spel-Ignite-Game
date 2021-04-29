using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class GenericMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer rd;
    public bool dontUpdate = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rd = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        animator.SetBool("Moving", rb.velocity.x != 0 || rb.velocity.y != 0);
        bool sign = rb.velocity.x == 0 ? transform.eulerAngles.y == 180 : rb.velocity.x < 0;
        if (!dontUpdate)
        {
            transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, sign ? 180 : 0, transform.eulerAngles.z));
        }
    }
}
