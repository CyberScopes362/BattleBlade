﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class DamageModifier : MonoBehaviour 
{
    public GameObject enemyObject;
    public List<SpriteRenderer> allSpriteRenderers = new List<SpriteRenderer>();
    public List<Color> allSpriteOGColors = new List<Color>();
    public List<Vector2> allSpriteOGScale = new List<Vector2>();

    GameObject slashMarks;
    TempMove tempMove;
    GameObject healthBarObject;
    SpriteRenderer healthBar;
    SpriteRenderer healthBarParent;

    GameObject hero;

    public float currentHealth;
    public float maxHealth;
    public float damageMin;
    public float damageMax;
    public float knockbackMin;
    public float knockbackMax;

    [HideInInspector]
    public float knockback;
    public float healthRatio;

    public bool isHit = false;
    public bool isDead = false;

    [HideInInspector]
    public float showHealthTimer;
    float givenDamage;
    float givenKnockback;
    int givenKnockbackDirection;
    float defXScale;

    Color healthBarParentColor;

    bool critHit;
    bool knockbackHit;
    public bool sprReset;

    public bool overriderHealthbarPlacement;

    float xDistBar;
    float yDistBar;

    void Start()
    {
        ObjectFinder objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();
        sprReset = false;

        foreach(SpriteRenderer sprRend in allSpriteRenderers)
        {
            allSpriteOGColors.Add(sprRend.color);
            allSpriteOGScale.Add(sprRend.transform.localScale);
        }

        slashMarks = objectFinder.slashMarks;
        hero = objectFinder.hero;

        currentHealth = maxHealth;
        tempMove = objectFinder.hero.GetComponent<TempMove>();
        healthBarParent = transform.Find("HealthBar").gameObject.GetComponent<SpriteRenderer>();
        healthBarObject = healthBarParent.transform.Find("HealthBarInner").gameObject;
        healthBar = healthBarObject.GetComponent<SpriteRenderer>();

        healthBar.color = Color.green;

        defXScale = healthBarObject.transform.localScale.x;

        xDistBar = healthBarParent.transform.localPosition.x;
        yDistBar = healthBarParent.transform.localPosition.y;
    }

	public void Attack(float setDamage = default(float)) 
	{
        givenDamage = UnityEngine.Random.Range(damageMin, damageMax);
        givenKnockback = UnityEngine.Random.Range(knockbackMin, knockbackMax);

        if (enemyObject.transform.localScale.x < 0)
            givenKnockbackDirection = -1;
        else
            givenKnockbackDirection = 1;

        if (setDamage != 0)
            givenDamage = setDamage;

        tempMove.TakeDamage(givenDamage, givenKnockback, givenKnockbackDirection);
	}

    public void RayAttack(float setDamage = default(float), float staminaRemoval = default(float))
    {
        givenDamage = UnityEngine.Random.Range(damageMin, damageMax);

        if (setDamage != 0)
            givenDamage = setDamage;

        if (enemyObject.transform.localScale.x < 0)
            givenKnockbackDirection = -1;
        else
            givenKnockbackDirection = 1;

        tempMove.TakeDamage(givenDamage, 0f, givenKnockbackDirection, staminaRemoval);
        
    }

    public void PushOut()
    {
        givenDamage = UnityEngine.Random.Range(damageMin, damageMax);

        if (hero.GetComponent<Rigidbody2D>().velocity.normalized.x < 0)
            givenKnockbackDirection = -1;
        else
            givenKnockbackDirection = 1;

        tempMove.TakeDamage(givenDamage, 0f, 0);
        hero.GetComponent<Rigidbody2D>().velocity = new Vector2(5f * givenKnockbackDirection, 5f);
        tempMove.DoStun();
        tempMove.thisAnimator.SetTrigger("TakeHit");
    }

    public void Hit(float takenDamage, float currentKnockback, GameObject critHitObject, float criticalChance, float knockbackChance)
    {
        if(!isHit)
        {
            isHit = true;

            //Knockback Chance Generator + Randomizer
            if (UnityEngine.Random.Range(0f, 1f) > knockbackChance)
            {
                knockback = currentKnockback;
                knockback = knockback + UnityEngine.Random.Range(-0.8f, 0.8f);
                knockbackHit = true;
            }
            else
                knockback = 0f;

            //Crit Generator
            float critRandom = UnityEngine.Random.Range(0.0f, 1.0f);

            if (criticalChance > critRandom)
            {
                takenDamage *= UnityEngine.Random.Range(1.5f, 3.0f);
                Instantiate(critHitObject, new Vector3(enemyObject.transform.position.x, healthBarObject.transform.position.y - 0.22f, enemyObject.transform.position.z), critHitObject.transform.rotation);
                critHit = true;
            }

            foreach(SpriteRenderer sprRend in allSpriteRenderers)
            {
                sprRend.transform.localScale = new Vector2(sprRend.transform.localScale.x + (Mathf.Sqrt(takenDamage) / 10f), sprRend.transform.localScale.y + (Mathf.Sqrt(takenDamage) / 10f));
                sprRend.color = Color.red;
                sprReset = true;
            }

            StopCoroutine("SpriteTimer");
            currentHealth -= takenDamage;

            var createSlash = Instantiate(slashMarks, enemyObject.transform.position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360))) as GameObject;

            /* Below is commented because it didnt look full of 'effects' - change if necessary */
            //Disable particles if there is no knockback
            //if (knockback == 0f)
            //    createSlash.GetComponent<ParticleSystem>().Stop();

            createSlash.GetComponent<SlashMarksScript>().takenDamage = takenDamage;

            showHealthTimer = 5f;
        }
    }

    void Update()
    {
        showHealthTimer -= 1f * Time.deltaTime;

        healthRatio = (currentHealth / maxHealth) * defXScale;
        if (healthRatio < 0f)
            healthRatio = 0;

        healthBarObject.transform.localScale = Vector2.Lerp(healthBarObject.transform.localScale, new Vector2(healthRatio, healthBarObject.transform.localScale.y), 12f * Time.deltaTime);

        if(showHealthTimer <= 0f)
        {
            healthBar.color = Color.Lerp(healthBar.color, Color.clear, 4f * Time.deltaTime);
            healthBarParentColor = Color.clear;
        }
        else
        {
            if (currentHealth <= 0f)
            {
                healthBar.color = Color.Lerp(healthBar.color, Color.clear, 6f * Time.deltaTime);
                healthBarParentColor = Color.clear;

                isDead = true;
            }
            else
            {
                healthBar.color = Color.Lerp(Color.red, Color.green, currentHealth / maxHealth);
                healthBarParentColor = Color.white;
            }
        }

        if (knockbackHit)
        {
            healthBarParent.color = Color.yellow;
            knockbackHit = false;
        }

        if (critHit)
        {
            healthBarParent.color = Color.red;
            critHit = false;
        }

        if(sprReset)
        {
            StartCoroutine("SpriteTimer");
            int i = 0;
            foreach (SpriteRenderer sprRend in allSpriteRenderers)
            {
                sprRend.transform.localScale = Vector2.Lerp(sprRend.transform.localScale, allSpriteOGScale[i], Time.deltaTime * 10f);
                sprRend.color = Color.Lerp(sprRend.color, allSpriteOGColors[i], Time.deltaTime * 10f);
                i++;
            }
        }

        healthBarParent.color = Color.Lerp(healthBarParent.color, healthBarParentColor, 4f * Time.deltaTime);
    }

    IEnumerator SpriteTimer()
    {
        yield return new WaitForSeconds(1.5f);
        sprReset = false;
    }

    void FixedUpdate()
    {
        if(!overriderHealthbarPlacement)
            healthBarParent.transform.position = new Vector2(enemyObject.transform.position.x - healthBarParent.bounds.extents.x, 1.55f);
        else
            healthBarParent.transform.position = new Vector2(enemyObject.transform.position.x - healthBarParent.bounds.extents.x + xDistBar, enemyObject.transform.position.y - healthBarParent.bounds.extents.y + yDistBar);
    }
}
