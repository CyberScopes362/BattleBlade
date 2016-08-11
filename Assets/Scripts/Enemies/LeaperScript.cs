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

    float knockback;
    public float speed;
    public float attackSpeedMultiplier;
    public float speedMultiplier;
    public float speedDifferentiation;

    public float weaponCooldown;
    public float laserDamage;
    public float laserDamageDiff;
    public float staminaRemoval;

    LayerMask playerLayer;

    public Transform[] points;
    public ParticleSystem[] fireParticles;
    public float[] checkRanges;
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
    public bool isHitAnim;

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

        playerLayer = objectFinder.playerLayer;

        //Random Additions to differentiate and stop enemy syncing
        thisAnimator.SetFloat("AttackSpeedMultiplier", attackSpeedMultiplier + Random.Range(-0.1f, 0.1f));
        thisAnimator.SetFloat("SpeedMultiplier", speedMultiplier + Random.Range(-0.1f, 0.1f));
        thisAnimator.SetFloat("HitSpeedMultiplier", 1f + Random.Range(-0.2f, 0.2f));

        weaponCooldown += Random.Range(-0.4f, 0.4f);

        setSpeed = Random.Range(speed - speedDifferentiation, speed + speedDifferentiation);
    }

    void Update()
    {
        isDead = damageModifier.isDead;
        knockback = damageModifier.knockback;
        shootTimer -= Time.deltaTime;

        heroPosition = new Vector2(tempMove.realheroPosition.transform.position.x - 0.2f, transform.position.y);

        //Raycasts
        //Short check
        shortRay = Physics2D.Raycast(transform.position, Vector2.left * transform.localScale.x, checkRanges[0], playerLayer);
        //Far check
        longRay = Physics2D.Raycast(transform.position, Vector2.left * transform.localScale.x, checkRanges[1], playerLayer);

        //If hero is within long range
        if(!attacking && !isHitAnim)
        {
            if (transform.position.x >= tempMove.realheroPosition.transform.position.x)
                transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);

            if (longRay)
            {
                //If hero is within short range
                if (shortRay)
                {
                    if (goToPlayer)
                    {
                        thisAnimator.SetTrigger("AttackShort");
                        goToPlayer = false;
                    }
                }
                else
                //If hero is within long range but beyond short range
                {
                    if(shootTimer <= 0f)
                    {
                        if (goToPlayer)
                        {
                            thisAnimator.SetTrigger("AttackLongDirect");
                            goToPlayer = false;
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


        if (damageModifier.isHit == true)
        {
            if (knockback != 0f)
            {
                thisRigidbody.velocity = new Vector2(knockback / 2f, 0f);
                thisAnimator.SetTrigger("Hit");
            }

            goToPlayer = false;
            damageModifier.isHit = false;
        }

        if (isDead)
        {
            if (deathTrigger == false)
            {
                deathTrigger = true;
                gameObject.layer = LayerMask.NameToLayer("PassbyEntity");
                GetComponentInParent<EnemyDeath>().enabled = true;

                foreach (ParticleSystem i in fireParticles)
                    i.Stop();

                thisAnimator.SetTrigger("Die");
                this.enabled = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (goToPlayer)
            thisRigidbody.position = Vector2.MoveTowards(transform.position, heroPosition, setSpeed * Time.deltaTime);
    }

    public void InitiateAttack()
    {
        //CHANGE (Done?)
        if (!isDead)
        {
            if (shortRay)
            {
                damageModifier.Attack();
            }
        }
    }

    public void FixFreeze()
    {
        thisAnimator.SetTrigger("Move");
        goToPlayer = true;
        checkWeaponRotation = false;
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
            shootTimer = weaponCooldown;
            weaponParticles.Play();

            weaponLineMat.SetColor("_TintColor", new Color(1f, 1f, 1f, 1f));
            lineAlpha = 1f;

            Ray2D shotRay = new Ray2D(weaponTip.transform.position, new Vector2(weaponTip.transform.up.x * transform.localScale.x, weaponTip.transform.up.y));
            RaycastHit2D shotRaycast = Physics2D.Raycast(weaponTip.transform.position, new Vector2(weaponTip.transform.up.x * transform.localScale.x, weaponTip.transform.up.y), 18f, playerLayer);

            weaponLine.SetPosition(0, weaponTip.transform.position);

           /* var heroLimit = Mathf.Sqrt((Mathf.Pow(hero.transform.position.x - weaponTip.transform.position.x, 2) -  Mathf.Pow(hero.transform.position.y - weaponTip.transform.position.y, 2)));

            if (float.IsNaN(heroLimit))
                heroLimit = 18f;

    */

            weaponLine.SetPosition(1, shotRay.GetPoint(18f));
             
            if (shotRaycast)
            {
                float randomSetDamage = Random.Range(laserDamage - laserDamageDiff, laserDamage + laserDamageDiff);
                damageModifier.RayAttack(randomSetDamage, staminaRemoval);
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