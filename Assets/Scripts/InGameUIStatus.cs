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

    public Color customHealthStart;
    public Color customHealthEnd;


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

        //Calculation for bar amount
        lifeBar.fillAmount = Mathf.Lerp(lifeBar.fillAmount, currentHealth / maxHealth, 8f * Time.deltaTime);
        lifeBar.color = Color.Lerp(customHealthEnd, customHealthStart, currentHealth / maxHealth);

    }
}
