using UnityEngine;
using System.Collections;

public class CriticalHitMovement : MonoBehaviour
{
    TextMesh thisText;
    public Color setColor;

    float timer = 0f;
    public float move;

    void Start()
    {
        thisText = GetComponent<TextMesh>();
        thisText.GetComponent<Renderer>().sortingLayerName = "Effects";
    }

    void Update()
    {
        timer += 1.4f * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y - move * Time.deltaTime, transform.position.z);

        if (timer > 0.6f)
        {
            setColor = Color.Lerp(setColor, Color.clear, 2.6f * Time.deltaTime);

            if (setColor.a <= 0.08f)
                Destroy(gameObject);
        }

        thisText.color = setColor;
    }
}

//
// Old system - Colour changing
//
/* 
    TextMesh thisText;
    public float moveUp;
    Color setColor;

    float timer = 0f;
    float newHue;
    float s;
    float v;

    void Start()
    {
        thisText = GetComponent<TextMesh>();
        thisText.GetComponent<Renderer>().sortingLayerName = "Effects";
        setColor = new Color(1, 1, 1, 1);
    }

    void Update()
    {
        timer += 1f * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y + moveUp * Time.deltaTime, transform.position.z);

        if (timer < 0.75f)
        {
            float h;
            Color.RGBToHSV(setColor, out h, out s, out v);
            newHue += 2.5f;

            if (newHue >= 255)
                newHue = 0;

            s = 1f;
            v = 1f;

            setColor = Color.HSVToRGB(newHue / 255, s, v);
        }
        else
        {
            setColor = Color.Lerp(setColor, Color.clear, 4f * Time.deltaTime);

            if(setColor.a <= 0.1f)
                Destroy(gameObject);
        }

        thisText.color = setColor;
    }
*/
