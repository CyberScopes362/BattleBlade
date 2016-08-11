using UnityEngine;
using System.Collections;

public class ItemCollect : MonoBehaviour
{
    SpriteRenderer bgImage;
    float lifetime;
    Color setColor;
    Color transparent;
    bool colorShift;
    bool destroying;
    GameObject hero;

    InventoryController inventoryCollector;

    public string thisItem;

    float dieTimer;

    void Start()
    {
        ObjectFinder objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        hero = objectFinder.hero;
        inventoryCollector = objectFinder.inventory.GetComponent<InventoryController>();

        bgImage = GetComponent<SpriteRenderer>();
        colorShift = false;

        transparent = new Color(1f, 1f, 1f, 0f);
    }

    void Update()
    {
        if (colorShift)
            setColor = Color.white;
        else
            setColor = transparent;

        if (bgImage.color.a < 0.2f)
            colorShift = true;

        if (bgImage.color.a >= 0.8f)
            colorShift = false;

        bgImage.color = Color.Lerp(bgImage.color, setColor, 1.5f * Time.deltaTime);       
    }

    void FixedUpdate()
    {
        if (destroying)
        {
            dieTimer += Time.deltaTime;

            if (dieTimer < 0.65f)
            {
                transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(0f, 0f), 1f * Time.deltaTime);
                transform.position = Vector2.Lerp(transform.position, hero.transform.position, 2f * (dieTimer * 10f) * Time.deltaTime);
            }
            else
            {
                if (thisItem == "Health")
                    inventoryCollector.potionsTotal[0] += 1;

                if (thisItem == "Stamina")
                    inventoryCollector.potionsTotal[1] += 1;

                if (thisItem == "Multi")
                    inventoryCollector.potionsTotal[2] += 1;

                Destroy(gameObject);
            }
        }
    }

    void OnTriggerStay2D(Collider2D heroCol)
    {
        if(heroCol.tag == "Player" && !destroying)
            destroying = true;
    }
}

