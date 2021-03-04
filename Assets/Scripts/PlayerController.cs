using System;
using UnityEngine;
using Utils;
using System.Runtime.InteropServices;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 2.0f;

    public Transform pfWater;
    public Transform pfFire;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        OnAction += PlayerAction;

#if !UNITY_EDITOR && UNITY_WEBGL
WebGLInput.captureAllKeyboardInput = false;
#endif
    }

    void Update()
    {
        HandleMovement();
        HandleActions();
    }

    private void HandleMovement()
    {
        if ((Input.GetAxisRaw("Vertical") > 0.5 || Input.GetAxisRaw("Vertical") < -0.5) || (Input.GetAxisRaw("Horizontal") > 0.5 || Input.GetAxisRaw("Horizontal") < -0.5))
        {
            //lastMove.y = Input.GetAxisRaw("Vertical");
            //lastMove.x = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * MoveSpeed, Input.GetAxisRaw("Vertical") * MoveSpeed);
        }

        if (Input.GetAxisRaw("Vertical") < 0.5 && Input.GetAxisRaw("Vertical") > -0.5)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        if (Input.GetAxisRaw("Horizontal") < 0.5 && Input.GetAxisRaw("Horizontal") > -0.5)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    public event EventHandler<OnActiontEventArgs> OnAction;
    public class OnActiontEventArgs : EventArgs
    {
        public Vector3 endPointPosition;
        public Vector3 shootPosition;
        public string skillName;
    }

    private void HandleActions()
    {
        if (Input.GetMouseButtonDown(0))
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            RequestAction();
#else
            TriggerAction("fire");
#endif
        }
    }

    public void TriggerAction(string skillName)
    {
        var mousePosition = UtilsClass.GetMousePosition2D();
        var direction = (mousePosition - transform.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        OnAction?.Invoke(this, new OnActiontEventArgs
        {
            endPointPosition = transform.position,
            shootPosition = mousePosition,
            skillName = skillName
        });
    }

    private void PlayerAction(object sender, OnActiontEventArgs e)
    {
        if (e.skillName == "water") {
            var waterTransform = Instantiate(pfWater, e.endPointPosition, Quaternion.identity);
            var direction = (e.shootPosition - e.endPointPosition).normalized;
            waterTransform.GetComponent<WaterBall>().Setup(direction);
        }
        else if (e.skillName == "fire")
        {
            var fireTransform = Instantiate(pfFire, e.endPointPosition, Quaternion.identity);
            var direction = (e.shootPosition - e.endPointPosition).normalized;
            fireTransform.GetComponent<FireBall>().Setup(direction);
        }
    }

    [DllImport("__Internal")]
    private static extern void RequestAction();
}
