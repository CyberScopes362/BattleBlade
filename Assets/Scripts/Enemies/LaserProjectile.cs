using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    Vector3 targetDirection;
    public float projectileSpeed;
    public float minDmg;
    public float maxDmg;
    public float knockback;

    TempMove tempMove;
    SpriteRenderer projectileSprite;
    public ParticleSystem mainParticles;
    public ParticleSystem endParticles;

    bool isHitHero;
    float endTimer;

    void Start()
    {
        isHitHero = false;
        projectileSprite = GetComponent<SpriteRenderer>();

        ObjectFinder objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();
        tempMove = objectFinder.hero.GetComponent<TempMove>();
    }
    
    void Update()
    {
        endTimer += Time.deltaTime;
        transform.position += transform.up * projectileSpeed * Time.deltaTime;
        projectileSpeed += (3f * Time.deltaTime);

        if(!endParticles.isEmitting && isHitHero)
        {
            Destroy(endParticles.gameObject);
            Destroy(gameObject);
        }

        if(endTimer >= 5f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D otherCol)
    {
        if(!isHitHero)
        {
            if (otherCol.gameObject.layer == 12)
            {
                endParticles.transform.parent = null;
                endParticles.transform.localScale = new Vector3(1f, 1f, 1f);
                endParticles.Play();
                mainParticles.Stop();
                projectileSprite.color = Color.clear;

                float givenDamage = Random.Range(minDmg, maxDmg);
                int givenKnockbackDirection;

                if (transform.rotation.z < 0)
                    givenKnockbackDirection = -1;
                else
                    givenKnockbackDirection = 1;

                tempMove.TakeDamage(givenDamage, knockback, givenKnockbackDirection);

                isHitHero = true;
            }
        }
    }
}