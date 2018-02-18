using UnityEngine;
using System.Collections;

public class AbsoluteShieldEffector : MonoBehaviour
{
    public SpriteRenderer leftPart;
    public SpriteRenderer rightPart;
    public Color setColor;
    bool switchSize;
    float xAdder;
    float xOG;

    void Start()
    {
        // --! This is inefficient - change to another way if possible

        foreach(GameObject i in GameObject.FindGameObjectsWithTag("AbsoluteShield"))
        {
            if(i != gameObject)
                Destroy(i);
        }

        leftPart.color = setColor;
        rightPart.color = setColor;

        transform.localScale = new Vector2(GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>().hero.transform.localScale.x, transform.localScale.y);
        xOG = transform.localScale.x;

        xAdder = 0.2f;
        if (transform.localScale.x < 0)
            xAdder = -xAdder;
        
        StartCoroutine("ShrinkTimer");
    }

    void Update()
    {
        setColor = Color.Lerp(setColor, Color.clear, 2f * Time.deltaTime);

        leftPart.color = setColor;
        rightPart.color = setColor;

        if(!switchSize)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(transform.localScale.x + xAdder, transform.localScale.y + 0.2f), 15f * Time.deltaTime);
        else
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(xOG, 1f), 10f * Time.deltaTime);
    }

    IEnumerator ShrinkTimer()
    {
        yield return new WaitForSeconds(0.05f);
        switchSize = true;
    }
}