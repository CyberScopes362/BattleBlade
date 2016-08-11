using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameUIStatus : MonoBehaviour 
{
    TempMove tempMove;
    InventoryController inventoryController;

    public Image lifeBarPrnt;
    public Image staminaBarPrnt;
    public Image lifeBar;
    public Image staminaBar;

    public Text healthTextTime;
    public Text staminaTextTime;
    float healthTextfloat;
    float staminaTextfloat;
    Color defHealthTextCol;
    Color defStaminaTextCol;

    float currentHealth;
    float maxHealth;
    float currentStamina;
    float maxStamina;

    public Color customHealthStart;
    public Color customHealthEnd;
    public Color customStaminaStart;
    public Color customStaminaEnd;

    Color healthflash;
    Color staminaflash;

    bool isBoostingHealth;
    bool isBoostingStamina;

    float timerHealthFlash;
    float timerStaminaFlash;

    bool unsetH = false;
    bool unsetS = false;


    void Start() 
	{
        ObjectFinder objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        inventoryController = objectFinder.inventory.GetComponent<InventoryController>();
        tempMove = objectFinder.hero.GetComponent<TempMove>();

        lifeBar.fillAmount = 1f;

        healthflash = Color.white;
        staminaflash = Color.white;

        defHealthTextCol = healthTextTime.color;
        defStaminaTextCol = staminaTextTime.color;

        healthTextTime.color = Color.clear;
        staminaTextTime.color = Color.clear;
    }
	
	void Update() 
	{
        maxHealth = tempMove.maxHealth;
        currentHealth = tempMove.currentHealth;
        maxStamina = tempMove.maxStamina;
        currentStamina = tempMove.currentStamina;

        isBoostingHealth = inventoryController.boosting[0];
        isBoostingStamina = inventoryController.boosting[1];


        //Calculation for bar amount
        lifeBar.fillAmount = Mathf.Lerp(lifeBar.fillAmount, currentHealth / maxHealth, 8f * Time.deltaTime);
        lifeBar.color = Color.Lerp(customHealthEnd, customHealthStart, currentHealth / maxHealth);

        staminaBar.fillAmount = Mathf.Lerp(staminaBar.fillAmount, currentStamina / maxStamina, 8f * Time.deltaTime);
        staminaBar.color = Color.Lerp(customStaminaEnd, customStaminaStart, currentStamina / maxStamina);


        //Flashing Bars
        if(isBoostingHealth)
        {
            timerHealthFlash += 2f * Time.deltaTime;

            if (timerHealthFlash > 0.5f)
                healthflash = Color.green;

            if (timerHealthFlash > 1f)
            {
                healthflash = Color.white;
                timerHealthFlash = 0;
            }

            lifeBarPrnt.color = Color.Lerp(lifeBarPrnt.color, healthflash, 8f * Time.deltaTime);
            SetHealthTimer();
        }
        else
        {
            if (lifeBarPrnt.color != Color.white)
                lifeBarPrnt.color = Color.Lerp(lifeBarPrnt.color, Color.white, 6f * Time.deltaTime);

            if (healthTextTime.color.a > 0)
                healthTextTime.color = Color.Lerp(healthTextTime.color, Color.clear, 4f * Time.deltaTime);

            if(unsetH)
            {
                timerHealthFlash = 0f;
                healthflash = Color.white;
                unsetH = false;
            }
        }


        if (isBoostingStamina)
        {
            timerStaminaFlash += 2f * Time.deltaTime;

            if (timerStaminaFlash > 0.5f)
                staminaflash = Color.blue;

            if (timerStaminaFlash > 1f)
            {
                staminaflash = Color.white;
                timerStaminaFlash = 0;
            }

            staminaBarPrnt.color = Color.Lerp(staminaBarPrnt.color, staminaflash, 8f * Time.deltaTime);
            SetStaminaTimer();
        }
        else
        {
            if (staminaBarPrnt.color != Color.white)
                staminaBarPrnt.color = Color.Lerp(staminaBarPrnt.color, Color.white, 6f * Time.deltaTime);

            if (staminaTextTime.color.a > 0)
                staminaTextTime.color = Color.Lerp(staminaTextTime.color, Color.clear, 4f * Time.deltaTime);

            if (unsetS)
            {
                timerStaminaFlash = 0f;
                staminaflash = Color.white;
                unsetS = false;
            }
        }
    }

    void SetHealthTimer()
    {
        if(!unsetH)
        {
            healthTextTime.color = defHealthTextCol;
            healthTextfloat = 11f;
            unsetH = true;
        }

        healthTextfloat -= Time.deltaTime;
        var inted = Mathf.FloorToInt(healthTextfloat);
        healthTextTime.text = inted.ToString();
    }

    void SetStaminaTimer()
    {
        if (!unsetS)
        {
            staminaTextTime.color = defStaminaTextCol;
            staminaTextfloat = 11f;
            unsetS = true;
        }

        staminaTextfloat -= Time.deltaTime;
        var inted = Mathf.FloorToInt(staminaTextfloat);
        staminaTextTime.text = inted.ToString();
    }
}
