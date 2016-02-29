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

    public float speed;
    public float speedDifferentiation;

    float setSpeed;

    public float attackRange;

    public bool attacking = false;
    public bool initiateAttack = false;
    public bool attacked;

    public bool goToPlayer;
    bool isDead;
    public bool freezeCheck;

    float health;
    public float range;

    public bool isGrounded;

    LayerMask floorMask;

    public Transform pointA;
    public Transform pointB;

    float knockback;

    bool deathTriggered = false;


    void Start()
    {
        ObjectFinder objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        hero = objectFinder.hero;
        floorMask = objectFinder.floorMask;

        tempMove = hero.GetComponent<TempMove>();
        thisAnimator = GetComponentInParent<Animator>();
        thisRigidbody = GetComponent<Rigidbody2D>();
        damageModifier = GetComponentInParent<DamageModifier>();

        setSpeed = Random.Range(speed - speedDifferentiation, speed + speedDifferentiation);

        thisAnimator.SetFloat("SpeedMultiplier", (setSpeed / 12) + 1);
    }

    void Update()
    {
        if (deathTriggered)
            return;

        isDead = damageModifier.isDead;
        heroPosition = new Vector2(tempMove.realheroPosition.transform.position.x, transform.position.y);
        range = Vector2.Distance(transform.position, tempMove.realheroPosition.transform.position);
        knockback = damageModifier.knockback;

        //Grounded Check
        isGrounded = Physics2D.OverlapArea(pointA.position, pointB.position, floorMask);

        if (!isGrounded && goToPlayer)
            goToPlayer = false; 

        //FIX: Spike stays hit after attacking it when its attacking parameter is set to false.
        //**Completed**
        //**Res: Through freezeCheck variable.

        if (!attacking && isGrounded)
        {
            //FIX: Spikes layers are set to 0, parts overlap unorderly
            //**Completed**
            //**Res: Use Vector3 instead of Vector2 for transform.localScale editing.

            //FIX: Overlapping between different enemies
            //**Completed**
            //**Red: Check 'AutoLayer' Script.

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
                if (!goToPlayer && !freezeCheck)
                {
                    goToPlayer = true;
                    freezeCheck = true;
                }

                if (goToPlayer)
                {
                    thisAnimator.SetTrigger("Attack");
                    goToPlayer = false;
                    freezeCheck = true;
                }
            }
        }

        if (damageModifier.isHit == true)
        {
            thisAnimator.SetTrigger("Hit");
            thisRigidbody.velocity = new Vector2(knockback, Mathf.Abs(knockback / 2f));
            isGrounded = false;
            isDead = false;
            damageModifier.isHit = false;
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
            gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, heroPosition, setSpeed * Time.deltaTime);
    }

    public void InitiateAttack()
    {
        if (range < attackRange && !isDead)
            damageModifier.Attack();
    }
}
