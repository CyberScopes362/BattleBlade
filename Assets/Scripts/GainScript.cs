using UnityEngine;
using System.Collections;

public class GainScript : MonoBehaviour
{
    SpriteRenderer bgEffect;
    public ParticleSystem particles;

    Color transparent;
    Color full;

    float dieTimer;
    GameObject hero;

    void Start()
    {
        bgEffect = GetComponentInChildren<SpriteRenderer>();
        transparent = new Color(bgEffect.color.r, bgEffect.color.g, bgEffect.color.b, 0f);
        full = bgEffect.color;
        hero = GameObject.FindGameObjectWithTag("Player");
        particles = GetComponent<ParticleSystem>();

        bgEffect.color = transparent;
    }

    void Update()
    {
        dieTimer += Time.deltaTime;

        if(dieTimer < 0.6f)
            bgEffect.color = Color.Lerp(bgEffect.color, full, 18f * Time.deltaTime);            

        if (dieTimer > 4f)
            bgEffect.color = Color.Lerp(bgEffect.color, transparent, 0.42f * Time.deltaTime);

        if (dieTimer > 9f)
            particles.Stop();

        if (dieTimer > 10f)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        transform.position = new Vector2(hero.transform.position.x + (0.1f * hero.transform.localScale.x), hero.transform.position.y - 1.2f);
    }
}
