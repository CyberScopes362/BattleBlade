using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RaptorScript : MonoBehaviour
{
    Animator thisAnimator;
    Collider2D thisCollider;
    public Collider2D swoopCol;
    Rigidbody2D thisRigidbody;
    GameObject hero;
    TempMove tempMove;
    DamageModifier damageModifier;

    Vector2 heroPosition;

    public GameObject laserProjectile;

    public float heroDistance;
    public float heroApartDistant;

    public bool attacking;
    public bool goToPlayer;
    public bool isHitAnim;
    bool isDead;

    float laserTimer;
    float swoopTimer;
    public float laserWaitTime;
    public float swoopWaitTime;

    public float[] xDistances;

    public float speed;
    public float speedDifferentiation;
    float setSpeed;

    public Transform boostParticlesTrans;
    public Transform hornGlowLaserTrans;

    bool deathTrigger;


    void Start()
    {
        ObjectFinder objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        hero = objectFinder.hero;
        tempMove = hero.GetComponent<TempMove>();
        damageModifier = GetComponentInParent<DamageModifier>();
        thisAnimator = GetComponent<Animator>();
        thisRigidbody = GetComponent<Rigidbody2D>();
        thisCollider = GetComponent<Collider2D>();

        setSpeed = Random.Range(speed - speedDifferentiation, speed + speedDifferentiation);

    }

    void Update()
    {
        isDead = damageModifier.isDead;
        heroPosition = new Vector2(tempMove.realheroPosition.transform.position.x, transform.position.y);
        heroDistance = Mathf.Abs(heroPosition.x - transform.position.x);

        laserTimer += Time.deltaTime;
        swoopTimer -= Time.deltaTime;

        if (!attacking && !isHitAnim)
        {
            if (!thisCollider.enabled)
                thisCollider.enabled = true;

            if (transform.position.x >= tempMove.realheroPosition.transform.position.x)
            {
                transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
                //Must flip boost particles aswell for some reason
                boostParticlesTrans.localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
                boostParticlesTrans.localScale = new Vector3(-1f, 1f, 1f);
            }

            //
            // Start State Checks
            //

            if (xDistances[0] >= heroDistance)
            {
                if (!goToPlayer && swoopTimer > 0f)
                {
                    thisAnimator.SetTrigger("Move");
                    goToPlayer = true;
                    return;
                }

                if (swoopTimer <= 0f)
                {
                    thisAnimator.SetTrigger("AttackSwoop");
                    goToPlayer = false;
                    swoopTimer = swoopWaitTime;
                    attacking = true;
                    thisCollider.enabled = false;
                    damageModifier.showHealthTimer = 0f;
                    return;
                }
            }
            else
            {
                if (xDistances[1] >= heroDistance)
                {
                    if (laserTimer >= laserWaitTime)
                    {
                        thisAnimator.SetTrigger("AttackLaser");
                        goToPlayer = false;
                        laserTimer = 0f;
                        attacking = true;
                        return;
                    }

                    if (!goToPlayer)
                    {
                        thisAnimator.SetTrigger("Move");
                        goToPlayer = true;
                        return;
                    }
                }
                else
                {
                    if(!goToPlayer)
                    {
                        thisAnimator.SetTrigger("Move");
                        goToPlayer = true;
                        return;
                    }
                }
            }

            //
            // End State Checks
            //
        }

        if (damageModifier.isHit == true)
        {
            //No knockback cause causes drag glitches
            if (attacking)
                attacking = false;

            thisAnimator.SetTrigger("Hit");

            damageModifier.isHit = false;
        }

        if (isDead)
        {
            if (deathTrigger == false)
            {
                deathTrigger = true;
                gameObject.layer = LayerMask.NameToLayer("PassbyEntity");
                GetComponentInParent<EnemyDeath>().enabled = true;
                boostParticlesTrans.GetComponent<ParticleSystem>().Stop();

                thisAnimator.SetTrigger("Die");
                this.enabled = false;
            }
        }
    }

    void FixedUpdate()
    {
        if(goToPlayer && !attacking && !deathTrigger)
            thisRigidbody.position = Vector2.MoveTowards(transform.position, new Vector2(heroPosition.x + (heroApartDistant * transform.localScale.x), transform.position.y), setSpeed * Time.deltaTime);
    }
  
    public void InitiateLaserProjectile()
    {
        Instantiate(laserProjectile, hornGlowLaserTrans.position, hornGlowLaserTrans.rotation);
    }

    public void SwoopAttack()
    {
        damageModifier.Attack();
    }

    public void DisableAttacking()
    {
        attacking = false;
    }

    public void EnableSwoopCol()
    {
        swoopCol.enabled = true;
    }

    public void DisableSwoopCol()
    {
        swoopCol.enabled = false;
    }
}
