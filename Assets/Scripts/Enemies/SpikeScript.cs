using UnityEngine;
using System.Collections;

public class SpikeScript : MonoBehaviour
{
    Animator thisAnimator;
    GameObject hero;
    TempMove tempMove;
    DamageModifier damageModifier;
    Rigidbody2D thisRigidbody;

    Vector2 heroPosition;
    RaycastHit2D checkRay;
    LayerMask floorMask;
    LayerMask playerLayer;

    public Transform[] points;

    public float speed;
    public float attackSpeedMultiplier;
    public float speedDifferentiation;
    public float hitRange;

    public bool attacking = false;
    public bool initiateAttack = false;
    public bool attacked;
    public bool goToPlayer;
    public bool isGrounded;
    public bool freezeFix;

    float health;
    float setSpeed;
    float knockback;

    bool isDead;
    bool deathTrigger;


    void Start()
    {
        ObjectFinder objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        hero = objectFinder.hero;
        floorMask = objectFinder.floorMask;
        playerLayer = objectFinder.playerLayer;

        tempMove = hero.GetComponent<TempMove>();
        thisAnimator = GetComponentInParent<Animator>();
        thisRigidbody = GetComponent<Rigidbody2D>();
        damageModifier = GetComponentInParent<DamageModifier>();

        setSpeed = Random.Range(speed - speedDifferentiation, speed + speedDifferentiation);

        thisAnimator.SetFloat("AttackSpeedMultiplier", attackSpeedMultiplier);
        thisAnimator.SetFloat("SpeedMultiplier", (setSpeed / 8) + 1);
    }

    void Update()
    {
        //Grounded Check
        isGrounded = Physics2D.OverlapArea(points[0].position, points[1].position, floorMask);

        if (deathTrigger)
        {
            if(isGrounded)
            {
                thisAnimator.SetTrigger("Die");
                this.enabled = false;
            }
            else
                return;
        }

        isDead = damageModifier.isDead;
        heroPosition = new Vector2(tempMove.realheroPosition.transform.position.x, transform.position.y);
        knockback = damageModifier.knockback;

        //Hero Check
        checkRay = Physics2D.Raycast(transform.position, Vector2.left * transform.localScale.x * 4, hitRange, playerLayer);

        if (!isGrounded && goToPlayer)
            goToPlayer = false;

        if (!attacking && isGrounded)
        {
            if(tempMove.isGrounded < 2)
            {
                if (transform.position.x >= tempMove.realheroPosition.transform.position.x)
                    transform.localScale = new Vector3(0.5f, transform.localScale.y, transform.localScale.z);
                else
                    transform.localScale = new Vector3(-0.5f, transform.localScale.y, transform.localScale.z);
            }

            //Start State Checks
            if(checkRay)
            {
                if (goToPlayer)
                {
                    thisAnimator.SetTrigger("Attack");
                    goToPlayer = false;
                }

                if(freezeFix)
                {
                    goToPlayer = true;
                    freezeFix = false;
                }
            }
            else
            {
                if (!goToPlayer)
                {
                    thisAnimator.SetTrigger("Move");
                    goToPlayer = true;
                }
            }
            //End State Checks
        }

        if (damageModifier.isHit == true)
        {
            if(knockback != 0f)
            {
                thisRigidbody.velocity = new Vector2(knockback, Mathf.Abs(knockback / 1.75f));
                thisAnimator.SetTrigger("Hit");
            }

            isGrounded = false;
            damageModifier.isHit = false;
        }

        if (isDead)
        {
            if (deathTrigger == false)
            {
                deathTrigger = true;
                gameObject.layer = LayerMask.NameToLayer("PassbyEntity");
                GetComponentInParent<EnemyDeath>().enabled = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (goToPlayer && isGrounded && deathTrigger == false)
        {
            if (tempMove.isGrounded < 2)
                thisRigidbody.position = Vector2.MoveTowards(transform.position, heroPosition, setSpeed * Time.deltaTime);
            else
                thisRigidbody.position = Vector2.MoveTowards(transform.position, points[2].position, Random.Range(setSpeed - 0.6f, setSpeed) * Time.deltaTime);
        }
    }

    public void InitiateAttack()
    {
        if (!isDead && Physics2D.Raycast(transform.position, Vector2.left * transform.localScale.x, hitRange + 0.265f, playerLayer))
            damageModifier.Attack();
    }

    public void FreezeFix()
    {
        freezeFix = true;
    }

    void OnTriggerEnter2D(Collider2D heroCollider)
    {
        if(heroCollider.gameObject.tag == "Player")
        {
            if (tempMove.isGrounded > 0 && tempMove.thisRigidbody.velocity.y <= 0f && tempMove.activeTimer <= 0f)
                damageModifier.PushOut();
        }
    }
}






//
//
//Older version. Replaced with raycast method.
//
//

/*
if (!attacking && isGrounded)
        {
            //FIX: Spikes layers are set to 0, parts overlap unorderly
            //**Completed**
            //**Res: Use Vector3 instead of Vector2 for transform.localScale editing.

            //FIX: Overlapping between different enemies
            //**Completed**
            //**Res: Check 'AutoLayer' Script.

            if (transform.position.x >= tempMove.realheroPosition.transform.position.x)
                transform.localScale = new Vector3(0.5f, transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(-0.5f, transform.localScale.y, transform.localScale.z);

            if (range > attackRange)
            {
                if (!goToPlayer)
                {
                    thisAnimator.SetTrigger("Move");
                    goToPlayer = true;
                }
            }
            else
            {
             //   if (!goToPlayer && !freezeCheck)
               // {
               //     goToPlayer = true;
              //      freezeCheck = true;
              //  }

                if (goToPlayer)
                {
                    var checkRay = Physics2D.Raycast(transform.position, Vector2.left * transform.localScale.x * 4, hitRange, playerLayer);

                    if(checkRay)
                    {
                        thisAnimator.SetTrigger("Attack");
                        goToPlayer = false;
                    }
                  
                  //  freezeCheck = true;
                }
            }
        }*/