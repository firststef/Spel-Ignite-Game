using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpelRuntime))]
[RequireComponent(typeof(StatsController))]
public class PlayerController : MonoBehaviour
{
    private StatsController stats;
    private Animator anim;

    [Header("Skills")]
    public string sendSkill;
    [HideInInspector]
    public float spellOffset = 0.8f;
    private SpelRuntime sr;

    private Rigidbody2D rb;
    private bool isKeyPressed = false;

    [HideInInspector]
    public int castCounter = 0;
    [HideInInspector]
    public UnityEvent endSpell = new UnityEvent();

    public Transform enemies;

    private int gold = 0;

    /* Actions */

    private void HandleMovement()
    {
        if ((Input.GetAxisRaw("Vertical") > 0.5 || Input.GetAxisRaw("Vertical") < -0.5) || (Input.GetAxisRaw("Horizontal") > 0.5 || Input.GetAxisRaw("Horizontal") < -0.5))
        {
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * stats.GetSpeed(), Input.GetAxisRaw("Vertical") * stats.GetSpeed());
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
        anim.SetBool("Attacking", false);
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetBool("Attacking", true);
            foreach (Transform child in enemies)
            {
                bool isClose = Vector3.Distance(transform.position, child.position) < 2f;
                var stats = child.GetChild(0).GetComponent<StatsController>();
                if (isClose && stats)
                {
                    stats.Damage(0.3f);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (!isKeyPressed)
            {
                isKeyPressed = true;

                if (castCounter == 0 && !sr.cancelling)
                {
#if !UNITY_EDITOR && UNITY_WEBGL
                    RequestAction();
#else
                    TriggerAction(sendSkill);
#endif
                }
                else
                {
                    endSpell.Invoke();
                }
            }
        }
        else if (isKeyPressed)
        {
            isKeyPressed = false;
        }
    }

    public void TriggerAction(string skillJson)
    {
        sr.Execute(skillJson);
    }

    /* Stats */

    void RefreshHealthBar()
    {
        int auxH = 0;
        float playerHealth = stats.GetHP();
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

    public void RefreshMPBar()
    {
        manaBar.GetComponent<Image>().fillAmount = stats.GetMP() / stats.maxMP;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        goldText.text = gold.ToString();
    }

    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /* Collision */

    public List<string> inventory = new List<string>(new[] { "cast" });

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

    private void SendUpdateInventory()
    {
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

        handle.Complete();
    }

    /* Generic functions */

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpelRuntime>();
        stats = GetComponent<StatsController>();
        anim = GetComponent<Animator>();

#if UNITY_EDITOR
        #region whileSkill
        var whileSkill = new
        {
            block = new
            {
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
                                    expr= new {
                                        expr= new {
                                            name= "fire",
                                            type= "NamedExpression"
                                        },
                                        value= new {
                                            name= "orb",
                                            type= "NamedExpression"
                                        },
                                        type= "Modification"
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
        #endregion

        #region fire
        var fire = new {
            block= new {
                items= new[] {
                        new {
                            which= "statement",
                            statement= new {
                                expr= new {
                                    name= "fire",
                                    type= "NamedExpression"
                                },
                                type= "Call"
                            },
                            type= "BlockItem"
                        }
                },
                type= "Block"
            },
            type= "Document"
        };
        #endregion

        #region water
        var water = new
        {
            block = new
            {
                items = new[] {
                        new {
                            which= "statement",
                            statement= new {
                                expr= new {
                                    name= "water",
                                    type= "NamedExpression"
                                },
                                type= "Call"
                            },
                            type= "BlockItem"
                        }
                },
                type = "Block"
            },
            type = "Document"
        };
        #endregion

        #region earth
        var earth = new
        {
            block = new
            {
                items = new[] {
                        new {
                            which= "statement",
                            statement= new {
                                expr= new {
                                    name= "earth",
                                    type= "NamedExpression"
                                },
                                type= "Call"
                            },
                            type= "BlockItem"
                        }
                },
                type = "Block"
            },
            type = "Document"
        };
        #endregion

        #region fireOrb
        var fireOrb = new {
            block = new {
                items = new[] {
                            new {
                            which= "statement",
                            statement= new  {
                                expr= new {
                                    expr= new {
                                        name= "fire",
                                        type= "NamedExpression"
                                    },
                                    value= new {
                                        name= "orb",
                                        type= "NamedExpression"
                                    },
                                    type= "Modification"
                                },
                                type= "Call"
                            },
                            type= "BlockItem"
                        }
                },
                type = "Block"
            },
            type = "Document"
        };
        #endregion

        #region firespeed
        var firespeed = new
        {
            block = new
            {
                items = new[] {
                            new {
                            which= "statement",
                            statement= new  {
                                expr= new {
                                    expr= new {
                                        name= "fire",
                                        type= "NamedExpression"
                                    },
                                    value= new {
                                        name= "speed",
                                        type= "NamedExpression"
                                    },
                                    type= "Modification"
                                },
                                type= "Call"
                            },
                            type= "BlockItem"
                        }
                },
                type = "Block"
            },
            type = "Document"
        };
        #endregion

        sendSkill = JsonConvert.SerializeObject(water);
#endif

        stats.hpChanged.AddListener(RefreshHealthBar);
        stats.mpChanged.AddListener(RefreshMPBar);
        stats.onDeath.AddListener(Die);
    }

    void Update()
    {
        HandleMovement();
        HandleActions();

        anim.SetBool("Casting", sr.vmIsRunning || castCounter != 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.StartsWith("Block"))
        {
            var item = collision.GetComponent<Collectible>();
            if (item && item.id != "")
            {
                inventory.Add(item.id);

                SendUpdateInventory();

                Destroy(collision.gameObject);
            }
        }
    }

    /* External */

    [DllImport("__Internal")]
    private static extern void RequestAction();

    [DllImport("__Internal")]
    private static extern void UpdateInventory(string str);

    /* UI stuff */

    [Header("UI")]
    public Transform healthBar;
    public Transform manaBar;
    public Text goldText;
}
