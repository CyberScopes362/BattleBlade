using UnityEngine;
using System.Collections;

public class SlashMarksScript : MonoBehaviour
{
    ParticleSystem thisSystem;
    SpriteRenderer thisRenderer;
    ItemSelector itemSelector;

    public float maxTime;

    [HideInInspector]
    public float takenDamage;
    float timer;

    void Start()
    {
        thisRenderer = GetComponent<SpriteRenderer>();
        thisSystem = GetComponent<ParticleSystem>();
        thisSystem.GetComponent<Renderer>().sortingLayerName = "Effects";
        thisRenderer.color = new Color(thisRenderer.color.r, thisRenderer.color.g, thisRenderer.color.b, 1f);
        gameObject.transform.localScale = new Vector2(transform.localScale.x + Mathf.Abs((Mathf.Pow(takenDamage, 2)) / 2000f), transform.localScale.x + Mathf.Abs((Mathf.Pow(takenDamage, 2)) / 2000f));
    }

    void Update()
    {
        timer += Time.deltaTime;

        if(timer > 0.4f)
            thisRenderer.color = new Color(thisRenderer.color.r, thisRenderer.color.g, thisRenderer.color.b, thisRenderer.color.a - 1f * Time.deltaTime);

        if (timer >= maxTime)
            Destroy(gameObject);
    }
}