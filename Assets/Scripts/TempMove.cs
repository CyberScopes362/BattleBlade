using UnityEngine;
using Xft;
using System.Collections;

//Super script for main hero interactions

public class TempMove : MonoBehaviour 
{
    //Gameobjects
    public GameObject weapon;
    public GameObject heroHitParticles;
    public GameObject critHitObject;
    GameObject floor;

    //Get Components
    Animator thisAnimator;
    Rigidbody2D thisRigidbody;
    BoxCollider2D weaponCollider;
    public XWeaponTrail trail;

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

    //Initiators between Update and FixedUpdate
    bool jumpInitiated = false;

    //Checks
    public bool canAttack = true;
    public bool dash = false;
    bool block = false;
    bool isFlipped = false;
    bool lerpToGround = false;

    //Timers and Set Times
    float activeTimer;
    float jumpTimer;
    public float lightAttackTime = 0.5f;
    public float heavyAttackTime = 1f;
    public float jumpAttackTime;
    public float jumpInterval = 0.3f;

    //Dash Speeds
    float currentDashSpeed;
    public float lightAttackDashSpeed = 1.4f;
    public float heavyAttackDashSpeed = 0.9f;

    //Vectors
    public Transform pointA;
    public Transform pointB;
    public Transform boxOrigination;
    public Transform realheroPosition;

    //Class Related
    enum HeroClass {Fire, Ice, Wind, Energy, Neon, Cyber, Nuclear}

    LayerMask floorMask;
    public LayerMask attackableMask;

    //-----
    //For health and attack
    //-----
    public float maxHealth;
    public float currentHealth;

    int currentAttackType;
    float currentDamage;
    float currentKnockback;
    float currentCriticalChance;

    //These vars should be set by weapon
    //Knockback divider amount
    public float knockbackRatio;
    //Strength of attacks
    public float lightAttackStrength;
    public float heavyAttackStrength;
    public float slamAttackStrength;
    public float criticalChance;


    void Start()
    {
        ObjectFinder objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        transform.position = objectFinder.spawnPoint.transform.position;
        floor = objectFinder.floor;
        floorMask = objectFinder.floorMask;

        thisAnimator = GetComponent<Animator>();
        thisRigidbody = GetComponent<Rigidbody2D>();
        weaponCollider = weapon.GetComponent<BoxCollider2D>();

        blockingMovementSpeed = defaultMovementSpeed / 3f;
        activeTimer = 0f;

        currentHealth = maxHealth;
        SetAnimationType(0);
        SetHeroClass(HeroClass.Fire);
    }

    void Update()
    {
        //Timer Updates
        activeTimer -= 1f * Time.deltaTime;
        jumpTimer += 1f * Time.deltaTime;

        //-------------
        //Attacking
        //-------------

        if (activeTimer <= trailOvertime)
            trail.Deactivate();

        if (activeTimer <= 0f)
        {
            canAttack = true;

            if(block == false)
            {
                if(isGrounded == 0)
                {
                    //Light Attack
                    if (Input.GetButtonDown("Fire1"))
                        Attack(0);

                    //Heavy Attack
                    if (Input.GetButtonDown("Fire2"))
                        Attack(1);
                }
                else
                {
                    if(isGrounded == 2)
                    {
                        //Jump Slam Attack
                        if (Input.GetButtonDown("Fire2"))
                            Attack(2);
                    }
                }
            } 
        }
        else
            canAttack = false;

        //Blocking
        if (Input.GetButton("Fire3") && isGrounded == 0)
        {
            thisAnimator.SetBool("Block", true);
            movementSpeed = blockingMovementSpeed;

            if (weaponCollider.isTrigger != false)
              weaponCollider.isTrigger = false;

            if(!block)
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
        if (Input.GetButtonDown("Jump"))
            jumpInitiated = true;


        //Grounded Check
        if (Physics2D.OverlapArea(pointA.position, pointB.position, floorMask))
        {
            isGrounded = 0;
            thisAnimator.SetInteger("Jump", isGrounded);
        }

       
    }

    void FixedUpdate()
    {
        Vector3 moveForward = new Vector3(movementSpeed * flipRatio, 0f, 0f);
        Vector3 dashForward = new Vector3(currentDashSpeed * flipRatio, 0f, 0f);

        //Movement
        if (canAttack == true && dash == false && (Input.GetAxis("Horizontal") > 0f || Input.GetAxis("Horizontal") < 0f))
        {
            thisAnimator.SetBool("Walk", true);
            transform.position += moveForward * Time.deltaTime;
            
            if(Input.GetAxis("Horizontal") < 0f && isFlipped ==  false)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                flipRatio = -1;
                isFlipped = true;
            }

            if (Input.GetAxis("Horizontal") > 0f && isFlipped == true)
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

        if(lerpToGround)
        {
            if(isGrounded == 0)
            {
                thisAnimator.SetTrigger("JumpAttackSlamExecute");
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
        //attackType: 0 = Light, 1 = Heavy, 2 = Jump Attack

        randomAttack = Random.Range(1, 4);
        thisAnimator.SetInteger("SetRandomAttack", randomAttack);
        currentAttackType = attackType;

        if (activeTimer > 0f)
            thisAnimator.SetTrigger("AttackConsecutive");

        switch(attackType)
        {
            //Light
            case 0:
                currentDashSpeed = lightAttackDashSpeed;
                thisAnimator.SetTrigger("LightAttack");
                activeTimer = lightAttackTime;
                break;
            
            //Heavy
            case 1:
                currentDashSpeed = heavyAttackDashSpeed;
                thisAnimator.SetTrigger("HeavyAttack");
                activeTimer = heavyAttackTime;
                break;

            //Jump Attack
            case 2:
                currentDashSpeed = 0f;
                thisAnimator.SetTrigger("JumpAttackSlam");
                activeTimer = jumpAttackTime;
                lerpToGround = true;
                break;
        }
    }

    public void DealDamage()
    {
        switch (currentAttackType)
        {
            //Light
            case 0:
                currentDamage = lightAttackStrength;
                break;

            //Heavy
            case 1:
                currentDamage = heavyAttackStrength;
                break;

            //Jump Attack
            case 2:
                currentDamage = slamAttackStrength;
                break;
        }

        //
        //## Damage modifiers go here
        // eg currentDamage = currentDamage * boost/ability/whatever
        //
        
        currentKnockback = currentDamage / knockbackRatio;

        var hit = Physics2D.BoxCastAll(boxOrigination.transform.position, new Vector2(2f, 2.3f), 0f, Vector2.zero, attackRange, attackableMask);

        for (var i = 0; i < hit.Length; i++)
            hit[i].transform.gameObject.GetComponentInParent<DamageModifier>().Hit(currentDamage, currentKnockback * flipRatio, critHitObject, criticalChance);
    }

    public void TakeDamage(float takeDamage, float takeKnockback, int takeKnockbackDirection)
    {
        thisAnimator.SetTrigger("TakeHit");
        Instantiate(heroHitParticles, transform.position, transform.rotation);
        currentHealth -= takeDamage;
        thisRigidbody.velocity = new Vector2(-takeKnockback * takeKnockbackDirection, thisRigidbody.velocity.y);
    }


    //If just swords, this system should be removed.
    void SetAnimationType(int setType)
    {
        thisAnimator.SetInteger("AttackType", setType);
    }

    //Set Perks
    //Keep at 1dp for complete synchronization and simpler settings
    //
    // 
    // SUBJECT TO CHANGE - THESE STATS SHOULD BE SET BY ARMOR ITSELF 
    // 
    //
    //

    void SetHeroClass(HeroClass setClass)
    {
        //Movement Speed
        float movS;
        //Attack Speed
        float atkS;
        //Blocking Speed
        float blkS;
        //Attack Anim Speed (No edit required)
        float tempAtkSAnim;

        switch (setClass)
        {
            case HeroClass.Fire:
                //Set temp vars:
                movS = 1.25f;
                atkS = 1.2f;
                blkS = 1.2f;

                //Movement Speed 
                movementSpeed = defaultMovementSpeed * movS;
                setMovementSpeed = movementSpeed;
                moveMultiplier = movS;
                thisAnimator.SetFloat("MoveMultiplier", movS);

                //Attack Speed
                lightAttackTime = lightAttackTime / atkS;
                heavyAttackTime = heavyAttackTime / atkS;
                lightAttackDashSpeed = lightAttackDashSpeed * atkS;
                heavyAttackDashSpeed = heavyAttackDashSpeed * atkS;

                //Calculation for animation speed
                tempAtkSAnim = atkS;
                tempAtkSAnim = tempAtkSAnim - (atkS / 7f);
                thisAnimator.SetFloat("SpeedMultiplier", tempAtkSAnim);

                //Blocking Speed
                blockingMovementSpeed *= blkS;
                blockingMultiplier = blkS / 2f;
                break;

            case HeroClass.Ice:
                movementSpeed = defaultMovementSpeed;
                break;

            case HeroClass.Energy:
                movementSpeed = defaultMovementSpeed * 0.75f;
                break;

            case HeroClass.Wind:
                movementSpeed = defaultMovementSpeed * 1.5f;
                break;

            case HeroClass.Neon:
                //Set temp vars:
                movS = 1.2f;
                atkS = 1.1f;
                blkS = 1.4f;

                //Movement Speed 
                movementSpeed = defaultMovementSpeed * movS;
                setMovementSpeed = movementSpeed;
                moveMultiplier = movS;
                thisAnimator.SetFloat("MoveMultiplier", movS);

                //Attack Speed
                lightAttackTime = lightAttackTime / atkS;
                heavyAttackTime = heavyAttackTime / atkS;
                lightAttackDashSpeed = lightAttackDashSpeed * atkS;
                heavyAttackDashSpeed = heavyAttackDashSpeed * atkS;

                //Calculation for animation speed
                tempAtkSAnim = atkS;
                tempAtkSAnim = tempAtkSAnim - (atkS / 7f);
                thisAnimator.SetFloat("SpeedMultiplier", tempAtkSAnim);

                //Blocking Speed
                blockingMovementSpeed *= blkS;
                blockingMultiplier = blkS / 2f;
                break;

            case HeroClass.Cyber:
                movementSpeed = defaultMovementSpeed * 1.3f;
                break;

            case HeroClass.Nuclear:
                movementSpeed = defaultMovementSpeed * 0.85f;
                break;
        }
    }

    public void ActivateTrail()
    {
         trail.Activate();
    }
}
