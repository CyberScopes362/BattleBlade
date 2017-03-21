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

    public Image healthTimerImg;
    public Image staminaTimerImg;
    //public Text healthTextTime;
    //public Text staminaTextTime;
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

        defHealthTextCol = healthTimerImg.color;
        defStaminaTextCol = staminaTimerImg.color;

        healthTimerImg.color = Color.clear;
        staminaTimerImg.color = Color.clear;
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

            if (healthTimerImg.color.a > 0)
                healthTimerImg.color = Color.Lerp(healthTimerImg.color, Color.clear, 4f * Time.deltaTime);

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

            if (staminaTimerImg.color.a > 0)
                staminaTimerImg.color = Color.Lerp(staminaTimerImg.color, Color.clear, 4f * Time.deltaTime);

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
            healthTimerImg.color = defHealthTextCol;
            healthTextfloat = 10f;
            unsetH = true;
        }

        healthTextfloat -= Time.deltaTime;
        healthTimerImg.fillAmount = healthTextfloat / 10f;
    }

    void SetStaminaTimer()
    {
        if (!unsetS)
        {
            staminaTimerImg.color = defStaminaTextCol;
            staminaTextfloat = 10f;
            unsetS = true;
        }

        staminaTextfloat -= Time.deltaTime;
        staminaTimerImg.fillAmount = staminaTextfloat / 10f;
    }
}
