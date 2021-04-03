using System;
using UnityEngine;
using Utils;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spells;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpelRuntime))]
public class PlayerController : MonoBehaviour
{
    /* Stats */

    [Header("Character")]
    public float MoveSpeed = 2.0f;

    [Header("Stats")]
    public int playerHealth = 10;
    public const int maxMana = 40;
    public int playerMana = maxMana;

    public void DamagePlayer(int damage)
    {
        playerHealth -= damage;
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

    public void ConsumeMana(int mana)
    {
        playerMana -= mana;
        Debug.Log(playerMana / maxMana);
        manaBar.GetComponent<Image>().fillAmount = playerMana / maxMana;
    }

    [Header("Debug")]
    public string sendSkill;
    public float skillOffset = 0.8f;

    private Rigidbody2D rb;
    private bool isKeyPressed = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpelRuntime>();

#if UNITY_EDITOR
        var skill = new {
            block = new {
                items = new[] {
                    new {
                        which = "statement",
                        statement = new {
                            expr= new {
                                name= "playerMana",
                                type= "NamedExpression"
                            },
                            stmts= new[] {
                                new {
                                    expr=new {
                                        name= "fire",
                                        type= "NamedExpression"
                                    },
                                    type= "Call"
                                }
                            },
                            type = "WhileStatement"
                        },
                        type = "BlockItem"
                    }
                },
                type = "Block"
            },
            type = "Document"
        };
        sendSkill = JsonConvert.SerializeObject(skill);
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
                sr.TriggerAction(sendSkill);
#endif
            }
        }
        else if (isKeyPressed)
        {
            isKeyPressed = false;
        }
    }

    public void CastSpell(ICastSpell spell)
    {
        if (spell.isPlayerWorthy(this))
        {
            spell.cast();
            spell.applyConsequences(this);
        }
    }

    /* Collision */

    public List<string> inventory = new List<string>();

    [Serializable]
    struct DataUpdate
    {
        public List<string> inventory;
    }

    struct UpdateJob : IJob
    {
        [DeallocateOnJobCompletion]
        public NativeArray<char> inventory;

        public void Execute()
        {
            var buffer = new char[inventory.Length];
            inventory.CopyTo(buffer);
            var read = new string(buffer);
            //Debug.Log(read);
            //Debug.Log(JsonUtility.FromJson<List<string>>(read)[0]);
#if !UNITY_EDITOR && UNITY_WEBGL
            UpdateInventory(read);
#endif
        }
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

                var write = JsonConvert.SerializeObject(data).ToCharArray();
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

    /* External */

    [DllImport("__Internal")]
    private static extern void RequestAction();

    private SpelRuntime sr;

    [DllImport("__Internal")]
    private static extern void UpdateInventory(string str);

    [Header("UI")]
    public Transform healthBar;
    public Transform manaBar;
}
