using UnityEngine;
using System.Collections;

public class SlashMarksScript : MonoBehaviour
{
    SpriteRenderer thisRenderer;

    public float maxTime;

    [HideInInspector]
    public float takenDamage;
    float timer;

    Vector3 setLocalScale;

    void Start()
    {
        thisRenderer = GetComponent<SpriteRenderer>();
        thisRenderer.color = new Color(thisRenderer.color.r, thisRenderer.color.g, thisRenderer.color.b, 0.96f);
        setLocalScale = new Vector3(0.75f + Mathf.Abs((Mathf.Pow(takenDamage, 2)) / 2000f), 0.75f + Mathf.Abs((Mathf.Pow(takenDamage, 2)) / 2000f), 1f);
        transform.localScale = new Vector3(0.25f, 0.25f, 1f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if(timer > 0.4f)
            thisRenderer.color = new Color(thisRenderer.color.r, thisRenderer.color.g, thisRenderer.color.b, thisRenderer.color.a - 1.8f * Time.deltaTime);
        else
            transform.localScale = Vector3.Lerp(transform.localScale, setLocalScale, Mathf.Pow((0.4f + Time.deltaTime), 2f));

        if (timer >= maxTime)
            Destroy(gameObject);
    }
}