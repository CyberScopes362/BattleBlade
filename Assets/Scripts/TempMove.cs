using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Xft;
using System.Collections;

//Super script for main hero interactions

public class TempMove : MonoBehaviour 
{
    //Gameobjects
    public GameObject weapon;
    public GameObject heroHitParticles;
    public GameObject critHitObject;
    public GameObject slamAttackParticles;
    GameObject absoluteShield;
    GameObject floor;

    //Get Components
    [HideInInspector]
    public Animator thisAnimator;

    [HideInInspector]
    public Rigidbody2D thisRigidbody;

    EdgeCollider2D weaponCollider;
    public XWeaponTrail trail;
    XWeaponTrail heroTrail;

    //General Speeds and number values
    public float defaultMovementSpeed = 4f;
    public float jumpHeight = 5f;
    public float trailOvertime;
    float movementSpeed;
    float blockingMovementSpeed;
    float setMovementSpeed;
    float moveMultiplier;
    float blockingMultiplier;
    float chargeDamageMultiplier;

    public float attackRange;

    int randomAttack;
    int flipRatio = 1;

    public int isGrounded;

    float growingSpeed = 0.1f;
    float airFloatCounter = 0.4f;
    float airBoostCounter = 0.6f;

    public float airBoostSpeed;
    public float jumpAttackCheckDistance;

    public float airGravScale;
    float defaultGravScale;

    //Initiators between Update and FixedUpdate
    bool jumpInitiated = false;

    //Checks
    public bool canAttack = true;
    public bool dash = false;
    bool block = false;
    bool isFlipped = false;
    bool lerpToGround = false;
    bool airBoost = false;
    bool inAirHeightCheck;
    bool midNoTimeAttack;
    bool heroTrailInitiate;

    //Timers and Set Times
    [HideInInspector]
    public float activeTimer;

    float jumpTimer;
    public float lightAttackTime = 0.5f;
    public float heavyAttackTime = 1f;
    public float slashAttackTime;
    public float jumpAttackTime;
    public float airAttackTime;
    public float jumpInterval = 0.3f;

    //Dash Speeds
    float currentDashSpeed;
    public float lightAttackDashSpeed = 1.4f;
    public float heavyAttackDashSpeed = 0.9f;
    public float slashDashSpeed = 3f;
    public float airBoostDistance;

    //Vectors
    public Transform pointA;
    public Transform pointB;
    public Transform internalPointA;
    public Transform internalPointB;
    public Transform realheroPosition;

    //Colliders
    public Transform[] externalColliders;

    //Masks
    LayerMask floorMask;
    public LayerMask attackableMask;

    //-----
    //For health and attack
    //-----
    public float maxHealth;
    public float currentHealth;
    public float maxStamina;
    public float currentStamina;
    public float[] staminaList;
    public float staminaAdditions;
    [HideInInspector]
    public bool canStaminaBlock;

    float currentDamage;
    float currentKnockback;
    float knockbackChance;
    float currentCriticalChance;

    public float blockReducer;

    //These vars are set by weapon
    public float knockbackRatio;
    public float lightAttackStrength;
    public float heavyAttackStrength;
    public float slamAttackStrength;
    public float criticalChance;

    //This is calculated so not set by weapon
    public float slashAttackStrength;

    //These vars are set by items/armor pieces/whatever
    //Multipliers:
    //Movement Speed
    public float movS;
    //Attack Speed
    public float atkS;
    //Blocking Speed
    public float blkS;
    //Blocking Boost
    public float blkbstS;
    //Total Health Additions
    public float hthS;


    void Start()
    {
        ObjectFinder objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        transform.position = objectFinder.spawnPoint.transform.position;
        floor = objectFinder.floor;
        floorMask = objectFinder.floorMask;
        absoluteShield = objectFinder.absoluteShield;
        heroTrail = objectFinder.heroTrailObj.GetComponent<XWeaponTrail>();

        thisAnimator = GetComponent<Animator>();
        thisRigidbody = GetComponent<Rigidbody2D>();
        weaponCollider = weapon.GetComponent<EdgeCollider2D>();

        SetHeroStats();

        blockingMovementSpeed = defaultMovementSpeed / 2.5f;
        activeTimer = 0f;

        //Initiate trails
        heroTrail.Activate();
        trail.Activate();

        defaultGravScale = thisRigidbody.gravityScale;

        currentHealth = maxHealth;
        currentStamina = maxStamina;
        thisAnimator.SetInteger("AttackType", 0);
    }

    void Update()
    {
        //Timer Updates
        activeTimer -= 1f * Time.deltaTime;
        jumpTimer += 1f * Time.deltaTime;

        //Stamina Growth
        if (currentStamina < 100f)
            currentStamina += (10f + staminaAdditions) * Time.deltaTime;
        else
            currentStamina = 100f;

        //Stamina system for blocking:
        if(!block)
        {
            if (currentStamina > 20f)
                canStaminaBlock = true;
            else
                canStaminaBlock = false;
        }
        else
            canStaminaBlock = true;

        //-------------
        //Attacking
        //-------------

        //Weapon Trail system
        if (activeTimer <= trailOvertime)
        {
            trail.StopSmoothly(0.06f);
            activeTimer = 0f;

            //Initiate Realign of trails
            if (!heroTrailInitiate)
            {
                trail.Deactivate();
                heroTrail.Deactivate();
                heroTrailInitiate = true;
            }
        }

        if (activeTimer <= 0f)
        {
            canAttack = true;

            if(block == false)
            {
                if(isGrounded == 0)
                {
                    //Light Attack
#if UNITY_STANDALONE || UNITY_EDITOR
                    if (Input.GetButtonDown("Fire1"))
#else
                    if (CrossPlatformInputManager.GetButtonDown("Fire1"))
#endif
                        Attack(0);

                    //Heavy Attack
#if UNITY_STANDALONE || UNITY_EDITOR
                    if (Input.GetButtonDown("Fire2"))
#else
                    if (CrossPlatformInputManager.GetButtonDown("Fire2"))
#endif
                        Attack(1);

                    //Slash Attack
#if UNITY_STANDALONE || UNITY_EDITOR
                    if (Input.GetButtonDown("Fire4") )
#else
                    if (CrossPlatformInputManager.GetButtonDown("Fire4"))
#endif
                        //Slash Attack = Stamina List Value 1
                        if(currentStamina >= staminaList[1])
                            Attack(4);
                }
                else
                {
                    if (isGrounded == 2 )
                    {
                        //Jump Slam Attack
#if UNITY_STANDALONE || UNITY_EDITOR
                        if (Input.GetButtonDown("Fire2"))
#else
                        if (CrossPlatformInputManager.GetButtonDown("Fire2"))
#endif
                            //Jump Slam Attack = Stamina List Value 0
                            if (currentStamina >= staminaList[0])
                                Attack(2);

                        //Air Attack
#if UNITY_STANDALONE || UNITY_EDITOR
                        if (Input.GetButtonDown("Fire1"))
#else
                        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
#endif
                            //Air Attack = Stamina List Value 2
                            if (currentStamina >= staminaList[2])
                                Attack(3);
                    }
                }
            } 
        }
        else
            canAttack = false;

        //Blocking
#if UNITY_STANDALONE || UNITY_EDITOR
        if (Input.GetButton("Fire3") && isGrounded == 0 && currentStamina > 0f && canStaminaBlock)
#else
        if (CrossPlatformInputManager.GetButton("Fire3") && isGrounded == 0)
#endif
        {
            thisAnimator.SetBool("Block", true);
            movementSpeed = blockingMovementSpeed;
            currentStamina -= 30f * Time.deltaTime;

            if (!block)
                block = true;

            if (thisAnimator.GetFloat("MoveMultiplier") != blockingMultiplier)
                thisAnimator.SetFloat("MoveMultiplier", blockingMultiplier);
        }
        else
        {
            thisAnimator.SetBool("Block", false);
            movementSpeed = setMovementSpeed;

            if (weaponCollider.isTrigger != true)
                weaponCollider.isTrigger = true;

            if (block)
                block = false;

            if (thisAnimator.GetFloat("MoveMultiplier") != moveMultiplier)
                thisAnimator.SetFloat("MoveMultiplier", moveMultiplier);
        }

        //Jump Initiation
#if UNITY_STANDALONE || UNITY_EDITOR
        if (Input.GetButtonDown("Jump"))
#else
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
#endif
            jumpInitiated = true;


        //Grounded Check
        if (Physics2D.OverlapArea(pointA.position, pointB.position, floorMask))
        {
            isGrounded = 0;
            thisAnimator.SetInteger("Jump", isGrounded);
            midNoTimeAttack = false;
        }   
    }

    void FixedUpdate()
    {
        //Raycast Check
        //Checks if distance between hero and floor is too low
        inAirHeightCheck = Physics2D.Raycast(transform.position, Vector2.down, jumpAttackCheckDistance, floorMask);

        Vector3 moveForward = new Vector3(movementSpeed * flipRatio, 0f, 0f);
        Vector3 dashForward = new Vector3(currentDashSpeed * flipRatio, 0f, 0f);

        //Movement
#if UNITY_STANDALONE || UNITY_EDITOR
        if (canAttack == true && dash == false && (Input.GetAxis("Horizontal") > 0f || Input.GetAxis("Horizontal") < 0f))
#else
        if (canAttack == true && dash == false && (CrossPlatformInputManager.GetAxisRaw("Horizontal") > 0f || CrossPlatformInputManager.GetAxisRaw("Horizontal") < 0f))
#endif
        {
            thisAnimator.SetBool("Walk", true);
            transform.position += moveForward * Time.deltaTime;

#if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetAxis("Horizontal") < 0f && isFlipped == false)
#else
            if (CrossPlatformInputManager.GetAxisRaw("Horizontal") < 0f && isFlipped == false)
#endif
            {
                transform.localScale = new Vector3(-1, 1, 1);
                flipRatio = -1;
                isFlipped = true;
            }

#if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetAxis("Horizontal") > 0f && isFlipped == true)
#else
            if (CrossPlatformInputManager.GetAxisRaw("Horizontal") > 0f && isFlipped == true)
#endif
            {
                transform.localScale = new Vector3(1, 1, 1);
                flipRatio = 1;
                isFlipped = false;
            }
        }
        else
            thisAnimator.SetBool("Walk", false);

        //Jumping
        if(jumpInitiated)
        {
            if(jumpTimer > jumpInterval && isGrounded < 2 && dash == false && canAttack == true)
            {
                isGrounded += 1;
                jumpTimer = 0f;

                thisRigidbody.velocity = new Vector2(thisRigidbody.velocity.x, jumpHeight);
                thisAnimator.SetInteger("Jump", isGrounded);

                jumpInitiated = false;
            }
        }

        if(dash)
            transform.position += dashForward * currentDashSpeed * Time.deltaTime;

        //For Jump Slam Attack
        if(lerpToGround)
        {
            if(isGrounded == 0)
            {
                thisAnimator.SetTrigger("JumpAttackSlamExecute");
                //Use pointB as point to create slam particles
                Instantiate(slamAttackParticles, pointB.transform.position, transform.rotation);

                lerpToGround = false;
                growingSpeed = 0.1f;
                thisRigidbody.isKinematic = false;

                airFloatCounter = 0.4f;
            }
            else
            {
                transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, floor.transform.position.y), growingSpeed * Time.deltaTime);
                thisRigidbody.isKinematic = true;

                airFloatCounter -= 1f * Time.deltaTime;

                if(airFloatCounter <= 0f)
                {
                    if(growingSpeed == 0.1f)
                        growingSpeed = 3f;

                    growingSpeed += 3.4f * Time.deltaTime;

                    if (growingSpeed >= 3f)
                        thisRigidbody.isKinematic = false;
                }
            }
        }

        //For Air Attack
        if(airBoost)
        {
            if (airBoostCounter > 0f)
            {
                airBoostCounter -= 1f * Time.deltaTime;
                transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x + (flipRatio * airBoostDistance), transform.position.y), airBoostSpeed * Time.deltaTime);
                thisRigidbody.gravityScale = airGravScale;
            }
            else
            {
                airBoostCounter = 0.6f;
                thisRigidbody.gravityScale = defaultGravScale;
                airBoost = false;
            }
        }

        //Reset jump
        jumpInitiated = false;
    }


    //
    //
    //Custom Functions:
    //
    //


    void Attack(int attackType)
    {
        //attackType: 
        //0 = Light, 
        //1 = Heavy, 
        //2 = Jump Attack, 
        //3 = Air Attack

        if (midNoTimeAttack)
            return;

        randomAttack = Random.Range(1, 4);
        thisAnimator.SetInteger("SetRandomAttack", randomAttack);

        if (activeTimer > 0f)
            thisAnimator.SetTrigger("AttackConsecutive");

        switch(attackType)
        {
            //Light
            case 0:
                currentDashSpeed = lightAttackDashSpeed;
                thisAnimator.SetTrigger("LightAttack");
                activeTimer = lightAttackTime;
                currentDamage = lightAttackStrength;
                knockbackChance = 0.65f;
                break;
            
            //Heavy
            case 1:
                currentDashSpeed = heavyAttackDashSpeed;
                thisAnimator.SetTrigger("HeavyAttack");
                activeTimer = heavyAttackTime;
                currentDamage = heavyAttackStrength;
                knockbackChance = 0.175f;
                break;

            //Jump Slam Attack
            case 2:
                if(!inAirHeightCheck)
                {
                    currentDashSpeed = 0f;
                    thisAnimator.SetTrigger("JumpAttackSlam");
                    activeTimer = jumpAttackTime;
                    currentDamage = slamAttackStrength;
                    lerpToGround = true;
                    knockbackChance = 0.075f;
                    currentStamina -= staminaList[0];
                }
                break;

            //Air Attack
            case 3:
                if (!inAirHeightCheck)
                {
                    currentDashSpeed = 0f;
                    thisAnimator.SetTrigger("AirAttack");
                    activeTimer = airAttackTime;
                    var airAttackStrength = lightAttackStrength / 1.4f;
                    currentDamage = airAttackStrength;
                    midNoTimeAttack = true;
                    airBoost = true;
                    knockbackChance = 0.3f;
                    currentStamina -= staminaList[2];
                }
                break;

            //Slash Attack
            case 4:
                currentDashSpeed = slashDashSpeed;
                thisAnimator.SetTrigger("SlashAttack");
                activeTimer = slashAttackTime;
                currentDamage = (lightAttackStrength + heavyAttackStrength) / 2f;
                knockbackChance = 0.75f;
                currentStamina -= staminaList[1];
                break;
        }
    }

    public void DealDamage(int selectCollider)
    {
        //
        //## Damage modifiers go here
        // eg currentDamage = currentDamage * boost/ability/whatever
        //

        
        currentKnockback = knockbackRatio * (currentDamage / 10f);

        var hit = Physics2D.BoxCastAll(externalColliders[selectCollider].transform.position, new Vector2(externalColliders[selectCollider].GetComponent<BoxCollider2D>().bounds.size.x, externalColliders[selectCollider].GetComponent<BoxCollider2D>().bounds.size.y), 0f, Vector2.zero, attackRange, attackableMask);

        for (var i = 0; i < hit.Length; i++)
            hit[i].transform.gameObject.GetComponentInParent<DamageModifier>().Hit(currentDamage, currentKnockback * flipRatio, critHitObject, criticalChance, knockbackChance);
    }

    public void TakeDamage(float takeDamage, float takeKnockback, int takeKnockbackDirection)
    {
        if (block)
        {
            if((takeKnockbackDirection * transform.localScale.x) > 0)
            {
                takeDamage /= blockReducer;
                Instantiate(absoluteShield, transform.position, transform.rotation);
            }
            else
            {
                //Dont want to play hit animation while jumping.
                if (isGrounded == 0)
                    thisAnimator.SetTrigger("TakeHit");

                Instantiate(heroHitParticles, transform.position, transform.rotation);
                thisRigidbody.velocity = new Vector2((-takeKnockback * takeKnockbackDirection) / 1.25f, thisRigidbody.velocity.y);
            }
        }
        else
        {
            //Dont want to play hit animation while jumping.
            if (isGrounded == 0)
                thisAnimator.SetTrigger("TakeHit");

            Instantiate(heroHitParticles, transform.position, transform.rotation);
            thisRigidbody.velocity = new Vector2((-takeKnockback * takeKnockbackDirection) / 1.25f, thisRigidbody.velocity.y);
        }

        currentHealth -= takeDamage;
    }

    //Set Perks
    //Keep at 1dp for complete synchronization and simpler settings
    //
    // 
    // SUBJECT TO CHANGE - THESE STATS SHOULD BE SET BY ARMOR ITSELF 
    // 
    //
    //

    void SetHeroStats()
    {
        //Attack Anim Speed (No edit required)
        float tempAtkSAnim;

        //Movement Speed 
        movementSpeed = defaultMovementSpeed * movS;
        setMovementSpeed = movementSpeed;
        moveMultiplier = movS;
        thisAnimator.SetFloat("MoveMultiplier", movS);

        //Attack Speed
        lightAttackTime = lightAttackTime / atkS;
        heavyAttackTime = heavyAttackTime / atkS;
        slashAttackTime = slashAttackTime / atkS;
        lightAttackDashSpeed = lightAttackDashSpeed * atkS;
        heavyAttackDashSpeed = heavyAttackDashSpeed * atkS;
        slashDashSpeed = slashDashSpeed * (atkS / 2f);

        //Calculation for animation speed
        tempAtkSAnim = atkS;
        tempAtkSAnim = tempAtkSAnim - (atkS / 7f);
        thisAnimator.SetFloat("SpeedMultiplier", tempAtkSAnim);

        //Blocking Speed
        blockingMovementSpeed *= blkS;
        blockingMultiplier = blkS / 2f;

        //Blocking Boost
        blockReducer *= blkbstS;

        //Total Health Multiplier
        maxHealth *= hthS;
    }

    public void SetBlockCollider()
    {
        if (weaponCollider.isTrigger)
            weaponCollider.isTrigger = false;
    }

    public void ActivateTrail()
    {
        trail.Activate();
    }

    public void ActivateHeroTrail()
    {
        heroTrail.Activate();
    }

    public void StopTrail()
    {
        trail.Deactivate();
    }

    public void StopHeroTrail()
    {
        heroTrail.StopSmoothly(0.1f);
    }


}
