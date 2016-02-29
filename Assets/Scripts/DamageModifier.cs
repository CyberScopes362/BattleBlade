﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class DamageModifier : MonoBehaviour 
{
    public GameObject enemyObject;
    public List<SpriteRenderer> allSpriteRenderers = new List<SpriteRenderer>();

    GameObject slashMarks;
    TempMove tempMove;
    GameObject healthBarObject;
    SpriteRenderer healthBar;
    SpriteRenderer healthBarParent;

    public float currentHealth;
    public float maxHealth;
    public float damageMin;
    public float damageMax;
    public float knockbackMin;
    public float knockbackMax;

    public float knockback;
    public float healthRatio;

    public bool isHit = false;
    public bool isDead = false;

    float showHealthTimer;
    float givenDamage;
    float givenKnockback;
    float defXScale;

    Color healthBarParentColor;

	
    void Start()
    {
        ObjectFinder objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        slashMarks = objectFinder.slashMarks;

        currentHealth = maxHealth;
        tempMove = objectFinder.hero.GetComponent<TempMove>();
        healthBarParent = transform.FindChild("HealthBar").gameObject.GetComponent<SpriteRenderer>();
        healthBarObject = healthBarParent.transform.FindChild("HealthBarInner").gameObject;
        healthBar = healthBarObject.GetComponent<SpriteRenderer>();

        healthBar.color = Color.green;

        defXScale = healthBarObject.transform.localScale.x;
    }

	public void Attack() 
	{
        givenDamage = UnityEngine.Random.Range(damageMin, damageMax);
        givenKnockback = UnityEngine.Random.Range(knockbackMin, knockbackMax);
        tempMove.TakeDamage(givenDamage, givenKnockback);
	}

    public void Hit(float takenDamage, float currentKnockback)
    {
        if(!isHit)
        {
            knockback = currentKnockback;
            isHit = true;
            currentHealth -= takenDamage;
            var createSlash = Instantiate(slashMarks, enemyObject.transform.position, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360))) as GameObject;
            createSlash.GetComponent<SlashMarksScript>().takenDamage = takenDamage;
            showHealthTimer = 5f;
        }
    }

    void Update()
    {
        showHealthTimer -= 1f * Time.deltaTime;

        healthRatio = (currentHealth / maxHealth) * defXScale;
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

        healthBarParent.color = Color.Lerp(healthBarParent.color, healthBarParentColor, 4f * Time.deltaTime);
    }

    void FixedUpdate()
    {
        healthBarParent.transform.position = new Vector2(enemyObject.transform.position.x - healthBarParent.bounds.extents.x, 1.55f);
    }
}
