using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    TempMove tempMove;
    public GameObject hero;
    public SpriteRenderer potionPlaceholder;
    public float healthBoost;
    public float staminaBoost;

    public bool[] boosting;

    public Image[] potionRenderers;
    public Sprite[] potionSprites;
    public Text[] potionCounters;
    public int[] potionsTotal;
    public GameObject[] particleObjects;
    public Button[] potionButtons;

    Animator heroAnimator;
    public bool animating;

    int itemTypeHeld;


    void Start()
    {
        ObjectFinder objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        hero = objectFinder.hero;
        heroAnimator = hero.GetComponent<Animator>();
        tempMove = hero.GetComponent<TempMove>();

        boosting = new bool[2];
    }

    void Update()
    {
        potionCounters[0].text = potionsTotal[0].ToString();
        potionCounters[1].text = potionsTotal[1].ToString();
        potionCounters[2].text = potionsTotal[2].ToString();

        if(tempMove.isGrounded != 0 || !tempMove.canAttack || tempMove.isReplenishing || tempMove.currentHealth < 0)
        {
            potionRenderers[0].color = Color.gray;
            potionRenderers[1].color = Color.gray;
            potionRenderers[2].color = Color.gray;
        }
        else
        {
            if (potionsTotal[0] == 0 || boosting[0])
                potionRenderers[0].color = Color.gray;
            else
                potionRenderers[0].color = Color.white;

            if (potionsTotal[1] == 0 || boosting[1])
                potionRenderers[1].color = Color.gray;
            else
                potionRenderers[1].color = Color.white;

            if (potionsTotal[2] == 0 || boosting[0] || boosting[1])
                potionRenderers[2].color = Color.gray;
            else
                potionRenderers[2].color = Color.white;
        }



        //Potion Colours and Button Interactivity
        if (potionRenderers[0].color == Color.gray)
        {
            if (potionButtons[0].interactable)
                potionButtons[0].interactable = false;
        }
        else
        {
            if (!potionButtons[0].interactable)
                potionButtons[0].interactable = true;
        }

        if (potionRenderers[1].color == Color.gray)
        {
            if (potionButtons[1].interactable)
                potionButtons[1].interactable = false;
        }
        else
        {
            if (!potionButtons[1].interactable)
                potionButtons[1].interactable = true;
        }

        if (potionRenderers[2].color == Color.gray)
        {
            if (potionButtons[2].interactable)
                potionButtons[2].interactable = false;
        }
        else
        {
            if (!potionButtons[2].interactable)
                potionButtons[2].interactable = true;
        }
    }

    public void OnClick(int itemType)
    {
        //CHECKS AND PREREQUIREMENTS (Re-edit: No longer required. Checks done within button interactavity in update above.)
        switch (itemType)
        {
            case 0:
                heroAnimator.SetTrigger("ActivateItem");
                potionPlaceholder.sprite = potionSprites[0];
                tempMove.isReplenishing = true;
                break;

            case 1:
                heroAnimator.SetTrigger("ActivateItem");
                potionPlaceholder.sprite = potionSprites[1];
                tempMove.isReplenishing = true;
                break;

            case 2:
                heroAnimator.SetTrigger("ActivateItem");
                potionPlaceholder.sprite = potionSprites[2];
                tempMove.isReplenishing = true;
                break;
        }

        itemTypeHeld = itemType;
    }

    public void OnStartParticles()
    {
        switch(itemTypeHeld)
        {
            case 0:
                Instantiate(particleObjects[0], new Vector2(hero.transform.position.x, 1.855f), transform.rotation);
                tempMove.potionHealthBoost = healthBoost;
                potionsTotal[0] -= 1;
                tempMove.isReplenishing = false;

                StartCoroutine(EndBoostA(10f));
                break;

            case 1:
                Instantiate(particleObjects[1], new Vector2(hero.transform.position.x, 1.855f), transform.rotation);
                tempMove.potionStaminaBoost = staminaBoost;
                potionsTotal[1] -= 1;
                tempMove.isReplenishing = false;

                StartCoroutine(EndBoostB(10f));
                break;

            case 2:
                Instantiate(particleObjects[2], new Vector2(hero.transform.position.x, 1.855f), transform.rotation);
                tempMove.potionHealthBoost = healthBoost / 1.45f;
                tempMove.potionStaminaBoost = staminaBoost / 1.45f;
                potionsTotal[2] -= 1;
                tempMove.isReplenishing = false;

                StartCoroutine(EndBoostC(10f));
                break;
        }
    }

    IEnumerator EndBoostA(float waitTime)
    {
        boosting[0] = true;
        yield return new WaitForSeconds(waitTime);
        tempMove.potionHealthBoost = 1f;
        boosting[0] = false;
    }

    IEnumerator EndBoostB(float waitTime)
    {
        boosting[1] = true;
        yield return new WaitForSeconds(waitTime);
        tempMove.potionStaminaBoost = 1f;
        boosting[1] = false;
    }

    IEnumerator EndBoostC(float waitTime)
    {
        boosting[0] = true;
        boosting[1] = true;
        yield return new WaitForSeconds(waitTime);
        tempMove.potionHealthBoost = 1f;
        tempMove.potionStaminaBoost = 1f;
        boosting[0] = false;
        boosting[1] = false;
    }
}