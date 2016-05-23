using UnityEngine;
using System.Collections;

public class CriticalHitMovement : MonoBehaviour
{
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
}
