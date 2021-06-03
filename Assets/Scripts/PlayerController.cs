using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Utils;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpelRuntime))]
[RequireComponent(typeof(StatsController))]
public class PlayerController : MonoBehaviour
{
    private StatsController stats;
    private Animator anim;
    private GenericMovement mov;
    private Rigidbody2D rb;
    private SpelRuntime sr;
    private GameManager gm;

    [Header("Skills")]
    public string sendSkill;

    private bool isKeyPressed = false;
    private Vector3 lastMousePos;
    private bool isCasting = false;

    private int gold = 0;

    /* Generic functions */

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpelRuntime>();
        stats = GetComponent<StatsController>();
        anim = GetComponent<Animator>();
        mov = GetComponent<GenericMovement>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        FillUIStuff();

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
        var fire = new
        {
            block = new
            {
                items = new[] {
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
                type = "Block"
            },
            type = "Document"
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
        var fireOrb = new
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

        //sendSkill = JsonConvert.SerializeObject(water);

        // cast fire
        sendSkill = "{    'block': {        'items': [            {                'which': 'statement',                'statement': {                    'object': 'fire',                    'holder': 'soul',                    'type': 'CreateStatement'                },                'type': 'BlockItem'            },            {                'which': 'statement',                'statement': {                    'object': {                        'name': 'fire',                        'type': 'NamedExpression'                    },                    'to': 'left hand',                    'type': 'MoveStatement'                },                'type': 'BlockItem'            },            {                'which': 'statement',                'statement': {                    'from': 'left hand',                    'type': 'ReleaseStatement'                },                'type': 'BlockItem'            }        ],        'type': 'Block'    },    'type': 'Document'}".Replace("'", "\"");
        // cast water
        sendSkill = "{    'block': {        'items': [            {                'which': 'statement',                'statement': {                    'object': 'water',                    'holder': 'soul',                    'type': 'CreateStatement'                },                'type': 'BlockItem'            },            {                'which': 'statement',                'statement': {                    'object': {                        'name': 'water',                        'type': 'NamedExpression'                    },                    'to': 'left hand',                    'type': 'MoveStatement'                },                'type': 'BlockItem'            },            {                'which': 'statement',                'statement': {                    'from': 'left hand',                    'type': 'ReleaseStatement'                },                'type': 'BlockItem'            }        ],        'type': 'Block'    },    'type': 'Document'}".Replace("'", "\"");
        // cast fire orb
        sendSkill = "{'block':{'items':[{'which':'statement','statement':{'object':'orb','holder':'left hand','type':'CreateStatement'},'type':'BlockItem'},{'which':'statement','statement':{'object':'fire','holder':'soul','type':'CreateStatement'},'type':'BlockItem'},{'which':'statement','statement':{'object':{'expr':{'name':'orb','type':'NamedExpression'},'value':{'name':'fire','type':'NamedExpression'},'type':'Modification'},'to':'left hand','type':'MoveStatement'},'type':'BlockItem'},{'which':'statement','statement':{'from':'left hand','type':'ReleaseStatement'},'type':'BlockItem'}],'type':'Block'},'type':'Document'}".Replace("'", "\"");
        // say
        // sendSkill = "{'block':{'items':[{'which':'statement','statement':{'message':'ceva','tone':'say','type':'PrintStatement'},'type':'BlockItem'},{'which':'statement','statement':{'message':'shop','tone':'say','type':'PrintStatement'},'type':'BlockItem'}],'type':'Block'},'type':'Document'}".Replace("'", "\"");
        // throw inventory
        // sendSkill = "{'block':{'items':[{'which':'statement','statement':{'object':{'name':'rock','type':'NamedExpression'},'from':'','to':'left hand','type':'MoveStatement'},'type':'BlockItem'},{'which':'statement','statement':{'from':'left hand','type':'ReleaseStatement'},'type':'BlockItem'}],'type':'Block'},'type':'Document'}".Replace("'", "\"");
        // cast fire 2
        // sendSkill = "{    'block': {        'items': [{'which':'statement','statement':{'message':'lord of daybreak, release thy flames','tone':'say','type':'PrintStatement'},'type':'BlockItem'},            {                'which': 'statement',                'statement': {                    'object': 'fire',                    'holder': 'soul',                    'type': 'CreateStatement'                },                'type': 'BlockItem'            },            {                'which': 'statement',                'statement': {                    'object': {                        'name': 'fire',                        'type': 'NamedExpression'                    },                    'to': 'left hand',                    'type': 'MoveStatement'                },                'type': 'BlockItem'            },            {                'which': 'statement',                'statement': {                    'from': 'left hand',                    'type': 'ReleaseStatement'                },                'type': 'BlockItem'            }        ],        'type': 'Block'    },    'type': 'Document'}".Replace("'", "\"");
        // cast ice
        sendSkill = "{    'block': {        'items': [            {                'which': 'statement',                'statement': {                    'object': 'ice',                    'holder': 'soul',                    'type': 'CreateStatement'                },                'type': 'BlockItem'            },            {                'which': 'statement',                'statement': {                    'object': {                        'name': 'ice',                        'type': 'NamedExpression'                    },                    'to': 'left hand',                    'type': 'MoveStatement'                },                'type': 'BlockItem'            },            {                'which': 'statement',                'statement': {                    'from': 'left hand',                    'type': 'ReleaseStatement'                },                'type': 'BlockItem'            }        ],        'type': 'Block'    },    'type': 'Document'}".Replace("'", "\"");
#endif

        stats.hpChanged.AddListener(RefreshHealthBar);
        stats.mpChanged.AddListener(RefreshMPBar);
        stats.onDeath.AddListener(Die);
        stats.onAddEffect.AddListener(AddEffect);
        stats.onRemoveEffect.AddListener(RemoveEffect);

        SendUpdateInventory();
    }

    void Update()
    {
        isCasting = sr.vmIsRunning || stats.castCounter != 0;
        anim.SetBool("Casting", isCasting);
        mov.dontUpdate = isCasting;

        if (!PauseControl.gameIsPaused) {
            HandleMovement();
            HandleActions();
        }
    }

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

        // Rotation
        var mousePos = UtilsClass.GetMousePosition2D();
        if (isCasting && lastMousePos != mousePos)
        {
            var direction = (mousePos - transform.position).normalized;
            var len = UtilsClass.HypotenuseLength(direction.x, direction.y);
            var factor = 1.0f / (len == 0 ? Mathf.Epsilon : len);
            direction = new Vector3(direction.x * factor, direction.y * factor, direction.z);

            bool sign = direction.x == 0 ? transform.eulerAngles.y == 180 : direction.x < 0;
            transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, sign ? 180 : 0, transform.eulerAngles.z));

            lastMousePos = mousePos;
        }
    }

    private void HandleActions()
    {
        anim.SetBool("Attacking", false);
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetBool("Attacking", true);
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (!isKeyPressed)
            {
                isKeyPressed = true;

                if (stats.castCounter == 0 && !sr.cancelling)
                {
#if !UNITY_EDITOR && UNITY_WEBGL
                    RequestAction();
#else
                    TriggerAction(sendSkill);
#endif
                }
                else
                {
                    stats.EndSpell();
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

    public void ChangeGold(int amount)
    {
        gold += amount;
        goldText.text = gold.ToString();
    }

    public int GetGold()
    {
        return gold;
    }

    private void Die()
    {
        gm.GameOver();
    }

    /* Collision */

    public List<string> inventory = new List<string>(new[] { "cast" });
    public List<string> passives = new List<string>();

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

    public void SendUpdateInventory()
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
    private Transform healthBar;
    private Transform manaBar;
    private Text goldText;
    private GameObject effectBar;

    private void FillUIStuff()
    {
        healthBar = GameObject.Find("LifeBar").transform;
        manaBar = GameObject.Find("ManaBar").transform;
        goldText = GameObject.Find("NumGold").GetComponent<Text>();
        effectBar = GameObject.Find("Effects");
    }

    [Header("Effects")]
    public GameObject[] prefabs;
    public GameObject ownPrefab;

    public void AddEffect(string effect)
    {
        foreach(GameObject go in prefabs)
        {
            if (go.name == effect)
            {
                Instantiate(go, effectBar.transform);
            }
        }
    }

    private void RemoveEffect(string effect)
    {
        foreach (Transform child in effectBar.transform)
        {
            if (child.name.StartsWith(effect))
            {
                Destroy(child.gameObject);
            }
        }
    }
}
