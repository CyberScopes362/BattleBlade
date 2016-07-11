using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameUIStatus : MonoBehaviour 
{
    TempMove tempMove;

    public Image lifeBar;
    public Image staminaBar;

    float currentHealth;
    float maxHealth;
    float currentStamina;
    float maxStamina;

    public Color customHealthStart;
    public Color customHealthEnd;
    public Color customStaminaStart;
    public Color customStaminaEnd;


	void Start() 
	{
        ObjectFinder objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        tempMove = objectFinder.hero.GetComponent<TempMove>();

        lifeBar.fillAmount = 1f;
    }
	
	void Update() 
	{
        maxHealth = tempMove.maxHealth;
        currentHealth = tempMove.currentHealth;
        maxStamina = tempMove.maxStamina;
        currentStamina = tempMove.currentStamina;

        //Calculation for bar amount
        lifeBar.fillAmount = Mathf.Lerp(lifeBar.fillAmount, currentHealth / maxHealth, 8f * Time.deltaTime);
        lifeBar.color = Color.Lerp(customHealthEnd, customHealthStart, currentHealth / maxHealth);

        staminaBar.fillAmount = Mathf.Lerp(staminaBar.fillAmount, currentStamina / maxStamina, 8f * Time.deltaTime);
        staminaBar.color = Color.Lerp(customStaminaEnd, customStaminaStart, currentStamina / maxStamina);
    }
}
