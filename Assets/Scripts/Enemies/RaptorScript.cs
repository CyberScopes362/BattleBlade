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
    Vector2 refVelocity;

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
    public Transform parentPos;

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

        //Differentiation betwen enemies; stop syncing
        parentPos.position = new Vector2(parentPos.position.x, parentPos.position.y + Random.Range(-0.35f, 0.35f));

        xDistances[0] += Random.Range(-0.25f, 0.25f);
        xDistances[1] += Random.Range(-0.25f, 0.25f);

        laserWaitTime += Random.Range(-0.4f, 0.4f);
        swoopWaitTime += Random.Range(-0.6f, 0.6f);

        heroApartDistant += Random.Range(-0.35f, 0.35f);
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
                parentPos.position = new Vector2(parentPos.transform.position.x, 5.44f);
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
        if (goToPlayer && !attacking && !deathTrigger)
            thisRigidbody.position = Vector2.SmoothDamp(thisRigidbody.position, new Vector2(heroPosition.x + (heroApartDistant * transform.localScale.x), thisRigidbody.position.y), ref refVelocity, 10f - setSpeed, Mathf.Infinity, Time.deltaTime);
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
