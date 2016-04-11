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

    public float speed;
    public float attackSpeedMultiplier;
    public float speedDifferentiation;
    public float hitRange;

    float setSpeed;

    public bool attacking = false;
    public bool initiateAttack = false;
    public bool attacked;

    public bool goToPlayer;
    bool isDead;

    bool nextLoopTrigger;
    bool checkGoTo = true;

    float health;

    public bool isGrounded;

    LayerMask floorMask;
    LayerMask playerLayer;

    public Transform pointA;
    public Transform pointB;
    public Transform infiniteChase;

    float knockback;

    bool deathTriggered = false;
    int stuckFix = 0;


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

        thisAnimator.SetFloat("SpeedMultiplier", (setSpeed / 12) + 1);
    }

    void Update()
    {
        if (deathTriggered)
            return;

        if (nextLoopTrigger)
        {
            stuckFix = 0;
            checkGoTo = true;
            nextLoopTrigger = false;
        }


        isDead = damageModifier.isDead;
        heroPosition = new Vector2(tempMove.realheroPosition.transform.position.x, transform.position.y);
        knockback = damageModifier.knockback;

        //Grounded Check
        isGrounded = Physics2D.OverlapArea(pointA.position, pointB.position, floorMask);

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

            if(checkRay)
            {
                //Look if it aint broke dont fix it.
                if (goToPlayer)
                {
                    thisAnimator.SetTrigger("Attack");
                    goToPlayer = false;
                    stuckFix = 1;
                }
                else
                {
                    if(!goToPlayer && stuckFix == 0)
                        goToPlayer = true;
                }
            }
            else
            {
                if (!goToPlayer)
                {
                    thisAnimator.SetTrigger("Move");
                    goToPlayer = true;
                }

                if(goToPlayer && checkGoTo)
                {
                    thisAnimator.SetTrigger("Move");
                    goToPlayer = true;
                    checkGoTo = false;
                }
            }
        }

        if (damageModifier.isHit == true)
        {
            thisRigidbody.velocity = new Vector2(knockback, Mathf.Abs(knockback / 1.75f));
            thisAnimator.SetTrigger("Hit");
            isGrounded = false;
            isDead = false;
            damageModifier.isHit = false;

            nextLoopTrigger = true;
        }

        if (isDead)
        {
            if (isGrounded && deathTriggered == false)
            {
                thisAnimator.SetTrigger("Die");
                thisRigidbody.isKinematic = true;
                GetComponent<BoxCollider2D>().enabled = false;
                GetComponent<CircleCollider2D>().enabled = false;
                deathTriggered = true;
                GetComponentInParent<EnemyDeath>().enabled = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (goToPlayer && isGrounded && deathTriggered == false)
        {
            if (tempMove.isGrounded < 2)
                thisRigidbody.position = Vector2.MoveTowards(transform.position, heroPosition, setSpeed * Time.deltaTime);
            else
                thisRigidbody.position = Vector2.MoveTowards(transform.position, infiniteChase.position, Random.Range(setSpeed - 0.6f, setSpeed) * Time.deltaTime);
        }
    }

    public void InitiateAttack()
    {
        if (!isDead && Physics2D.Raycast(transform.position, Vector2.left * transform.localScale.x, hitRange + 0.265f, playerLayer))
            damageModifier.Attack();
    }

    void OnTriggerEnter(Collider heroCollider)
    {
        print("Collided");
        if(heroCollider.gameObject.GetComponent<TempMove>().isGrounded > 0)
        {
            damageModifier.Attack();
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