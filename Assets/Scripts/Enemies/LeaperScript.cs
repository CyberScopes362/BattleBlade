using UnityEngine;
using System.Collections;

public class LeaperScript : MonoBehaviour
{
    Animator thisAnimator;
    GameObject hero;
    TempMove tempMove;
    DamageModifier damageModifier;
    Rigidbody2D thisRigidbody;

    Vector2 heroPosition;
    public GameObject weapon;
    public GameObject weaponTip;

    bool isGrounded;
    float knockback;

    public float speed;
    public float speedDifferentiation;

    LayerMask floorMask;
    LayerMask playerLayer;

    public Transform[] points;
    public ParticleSystem[] fireParticles;
    public float[] checkRanges;
    RaycastHit2D closeRay;
    RaycastHit2D shortRay;
    RaycastHit2D longRay;

    ParticleSystem weaponParticles;
    Quaternion tempRotation;
    LineRenderer weaponLine;
    Material weaponLineMat;
    float lineAlpha;

    public bool goToPlayer;
    bool checkWeaponRotation;
    bool setRotation;
    bool isDead;
    bool deathTrigger;

    public bool attacking = false;

    float setSpeed;

    float shootTimer;


    void Start()
    {
        ObjectFinder objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        hero = objectFinder.hero;

        tempMove = hero.GetComponent<TempMove>();
        thisAnimator = GetComponent<Animator>();
        thisRigidbody = GetComponent<Rigidbody2D>();
        damageModifier = GetComponentInParent<DamageModifier>();

        weaponLine = weaponTip.GetComponent<LineRenderer>();
        weaponLine.sortingLayerName = "Effects";
        weaponLineMat = weaponLine.material;
        weaponParticles = weaponTip.GetComponent<ParticleSystem>();

        floorMask = objectFinder.floorMask;
        playerLayer = objectFinder.playerLayer;

        setSpeed = Random.Range(speed - speedDifferentiation, speed + speedDifferentiation);
    }

    void Update()
    {
        isDead = damageModifier.isDead;

        shootTimer -= Time.deltaTime;

        //Grounded Check (Currently Useless)
        isGrounded = Physics2D.OverlapArea(points[0].position, points[1].position, floorMask);

        heroPosition = new Vector2(tempMove.realheroPosition.transform.position.x, transform.position.y);

        //Raycasts
        //Very close check
        closeRay = Physics2D.Raycast(transform.position, Vector2.left * transform.localScale.x, checkRanges[0], playerLayer);
        //Short check
        shortRay = Physics2D.Raycast(transform.position, Vector2.left * transform.localScale.x, checkRanges[1], playerLayer);
        //Far check
        longRay = Physics2D.Raycast(transform.position, Vector2.left * transform.localScale.x, checkRanges[2], playerLayer);

        if(!attacking)
        {
            if (transform.position.x >= tempMove.realheroPosition.transform.position.x)
                transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
        }

        //If hero is within long range
        if(!attacking)
        {
            if (closeRay && goToPlayer == false)
                goToPlayer = true;

            if (longRay)
            {
                //If hero is within short range
                if (shortRay)
                {
                    if (goToPlayer)
                    {
                        goToPlayer = false;
                        thisAnimator.SetTrigger("AttackShort");
                    }
                }
                else
                //If hero is within long range but beyond short range
                {
                    if(shootTimer <= 0f)
                    {
                        if (goToPlayer)
                        {
                            goToPlayer = false;
                            thisAnimator.SetTrigger("AttackLongDirect");
                        }
                    }
                    else
                    {
                        if (!goToPlayer)
                        {
                            goToPlayer = true;
                            thisAnimator.SetTrigger("Move");
                        }
                    }
                }
            }
            else
            //If hero is beyond long range
            {
                if (!goToPlayer)
                {
                    goToPlayer = true;
                    thisAnimator.SetTrigger("Move");
                }
            }
        }



        /*
        if attacktimer is appropriate
            if hero is within short range and direction/rotations are fine
                do short range attack
            else
                if hero is beyond short range and within long range and direction/rotations are fine
                    do long range attack
                    attacktimer is set high as cooldown
    
        during cooldown, walk back slowly if enemy is close, otherwise walk forward slowly
        */





        if (damageModifier.isHit == true)
        {
            if (knockback != 0f)
            {
                thisRigidbody.velocity = new Vector2(knockback, 0f);
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
                thisAnimator.SetTrigger("Die");
                this.enabled = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (goToPlayer)
        {
            thisRigidbody.position = Vector2.MoveTowards(transform.position, heroPosition, setSpeed * Time.deltaTime);
        }
    }

    public void InitiateAttack()
    {
        if (!isDead)
        {
            if(longRay)
            {
                if (shortRay)
                {
                    damageModifier.Attack();
                }
            }
        }
    }

    public void DirectShot(int prepared)
    {
        //Checking if it is intiated or being shot
        if (prepared == 0)
        {
            checkWeaponRotation = true;
            setRotation = true;
        }

        if(prepared == 1)
        {
            weaponParticles.Play();

            weaponLineMat.SetColor("_TintColor", new Color(1f, 1f, 1f, 1f));
            lineAlpha = 1f;

            Ray2D shotRay = new Ray2D(weaponTip.transform.position, new Vector2(weaponTip.transform.up.x * transform.localScale.x, weaponTip.transform.up.y));
            RaycastHit2D shotRaycast = Physics2D.Raycast(weaponTip.transform.position, new Vector2(weaponTip.transform.up.x * transform.localScale.x, weaponTip.transform.up.y), 12f, playerLayer);

            weaponLine.SetPosition(0, weaponTip.transform.position);
            weaponLine.SetPosition(1, shotRay.GetPoint(12f));

            if (shotRaycast)
            {
            //TODO: Change this to a proper attack command
                damageModifier.Attack();
            }
        }

        if (prepared == 2)
            checkWeaponRotation = false;
    }

    void LateUpdate()
    {
        if (checkWeaponRotation)
        {
            if (setRotation)
            {
                weapon.transform.rotation = new Quaternion(weapon.transform.rotation.x, weapon.transform.rotation.y, weapon.transform.rotation.z - (transform.localScale.x * Mathf.Atan((weapon.transform.position.y - hero.transform.position.y) / (weapon.transform.position.x - hero.transform.position.x))) + 0.244346f, weapon.transform.rotation.w);
                tempRotation = weapon.transform.rotation;
            }
            else
                weapon.transform.rotation = tempRotation;

            if (setRotation)
                setRotation = false;
        }

        if(weaponLineMat.GetColor("_TintColor").a > 0)
        {
            lineAlpha -= 1.5f * Time.deltaTime;
            weaponLineMat.SetColor("_TintColor", new Color(1f, 1f, 1f, lineAlpha));
        }
    }
}