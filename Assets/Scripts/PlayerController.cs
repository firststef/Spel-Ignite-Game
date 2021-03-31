using System;
using UnityEngine;
using Utils;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Options")]
    [Header("Character")]
    public float MoveSpeed = 2.0f;

    [Header("Skills")]
    public string sendSkill = "earth";
    public float skillOffset = 0.8f;

    [Header("Objects")]
    public Transform pfWater;
    public Transform pfFire;
    public Transform pfEarth;

    private Rigidbody2D rb;
    private bool isKeyPressed = false;

    void Awake()
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

    /* Actions */

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
        if (Input.GetMouseButtonDown(1))
        {
            if (!isKeyPressed)
            {
                isKeyPressed = true;

#if !UNITY_EDITOR && UNITY_WEBGL
                RequestAction();
#else
                TriggerAction(sendSkill);
#endif
            }
        }
        else if (isKeyPressed)
        {
            isKeyPressed = false;
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
        var direction = (e.shootPosition - e.endPointPosition).normalized;
        var len = UtilsClass.HypotenuseLength(direction.x, direction.y);
        var factor = 1.0f / (len == 0? Mathf.Epsilon: len);
        direction = new Vector3(direction.x * factor, direction.y * factor, direction.z);

        var newRoot = new Vector3(e.endPointPosition.x + direction.x * skillOffset, e.endPointPosition.y + direction.y * skillOffset, e.endPointPosition.z);
        Transform inst = null;
        if (e.skillName == "water") {
            inst = pfWater;
        }
        else if (e.skillName == "fire")
        {
            inst = pfFire;
        }
        else if (e.skillName == "earth")
        {
            inst = pfEarth;
        }
        else
        {
            // not found
            return;
        }
        Instantiate(inst, newRoot, Quaternion.identity).GetComponent<OrbSkill>().Setup(direction);
    }

    [DllImport("__Internal")]
    private static extern void RequestAction();

    /* Collision */

    [DllImport("__Internal")]
    private static extern void UpdateInventory(string str);

    public List<string> inventory = new List<string>();

    struct UpdateJob : IJob
    {
        [DeallocateOnJobCompletion]
        public NativeArray<char> inventory;

        public void Execute()
        {
            var buffer = new char[inventory.Length];
            inventory.CopyTo(buffer);
            var read = new string(buffer);
            Debug.Log(read);
            //Debug.Log(JsonUtility.FromJson<List<string>>(read)[0]);
#if !UNITY_EDITOR && UNITY_WEBGL
            UpdateInventory(read);
#endif
        }
    }


    [Serializable]
    struct DataUpdate
    {
        public List<string> inventory;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.StartsWith("Block"))
        {
            var item = collision.GetComponent<Collectible>();
            if (item && item.id != "")
            {
                inventory.Add(item.id);

                DataUpdate data;
                data.inventory = inventory;

                var write = JsonUtility.ToJson(data).ToCharArray();
                var position = new NativeArray<char>(write.Length, Allocator.Persistent);
                position.CopyFrom(write);

                var job = new UpdateJob()
                {
                    inventory = position
                };
                var handle = job.Schedule();

                Destroy(collision.gameObject);
                handle.Complete();
            }
        }
    }

    [Header("Stats")]
    public int playerHealth = 10;
    public Transform healthBar;

    public void DamagePlayer()
    {
        playerHealth -= 1;
        RefreshHealthBar();
    }

    void RefreshHealthBar()
    {
        int auxH = 0;
        foreach (Transform child in healthBar)
        {
            child.GetComponent<Image>().fillAmount = 0;
            if (auxH < playerHealth)
            {
                child.GetComponent<Image>().fillAmount += 0.5f;
                auxH++;
            }
            if (auxH < playerHealth)
            {
                child.GetComponent<Image>().fillAmount += 0.5f;
                auxH++;
            }
        }
    }
}
