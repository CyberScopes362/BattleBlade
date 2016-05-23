using UnityEngine;
using System.Collections;

public class AbsoluteShieldEffector : MonoBehaviour
{
    public SpriteRenderer leftPart;
    public SpriteRenderer rightPart;
    public Color setColor;

    void Start()
    {
        foreach(GameObject i in GameObject.FindGameObjectsWithTag("AbsoluteShield"))
        {
            if(i != gameObject)
                Destroy(i);
        }

        leftPart.color = setColor;
        rightPart.color = setColor;

        transform.localScale = new Vector2(GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>().hero.transform.localScale.x, transform.localScale.y);
    }

    void Update()
    {
        setColor = Color.Lerp(setColor, Color.clear, 2f * Time.deltaTime);

        leftPart.color = setColor;
        rightPart.color = setColor;
    }
}