using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class EnemyDeath : MonoBehaviour 
{
    public float waitTillDeathStart;
    public ParticleSystem deathParticles;
    List<SpriteRenderer> allSpriteRenderers = new List<SpriteRenderer>();

    bool startParticles = false;

    void Start()
    {
        allSpriteRenderers = GetComponent<DamageModifier>().allSpriteRenderers;
    }
	
	void Update() 
	{
        waitTillDeathStart -= 1f * Time.deltaTime;

        if(waitTillDeathStart <= 0f)
        {
            foreach (SpriteRenderer x in allSpriteRenderers)
                x.color = Color.Lerp(x.color, Color.clear, 2f * Time.deltaTime);

            if (startParticles == false)
            {
                deathParticles.Play();
                startParticles = true;
            }
            else
            {
                if (!deathParticles.isPlaying)
                    Destroy(gameObject);
            }     
        }
    }
}
