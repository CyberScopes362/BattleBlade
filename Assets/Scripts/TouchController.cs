using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TouchController : MonoBehaviour
{
    TempMove tempMove;
    public Image leftArrow;
    public Image rightArrow;

    public Image jumpSlamIndicator;
    public Image airAttackIndicator;
    public Image slashIndicator;
    public Image blockIndicator;

    public Button slashButton;
    public Button blockButton;

    public Color chargingColor;

    bool resetSelf = false;
    string resetParameter;

    bool dragged = false;

    float currentStamina;
    float[] staminaList;
    float minStaminaBlock;

    void Start()
    {
        tempMove = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>().hero.GetComponent<TempMove>();
        minStaminaBlock = tempMove.minStaminaBlock;

        leftArrow.color = new Color(1f, 1f, 1f, 0.7f);
        rightArrow.color = new Color(1f, 1f, 1f, 0.7f);
    }

    public void ExecuteCommand(string command)
    {
        switch (command)
        {
            case "MoveLeft":
                CrossPlatformInputManager.SetAxis("Horizontal", -1f);
                leftArrow.color = new Color(1f, 1f, 1f, 1f);
                break;

            case "MoveRight":
                CrossPlatformInputManager.SetAxis("Horizontal", 1f);
                rightArrow.color = new Color(1f, 1f, 1f, 1f);
                break;

            case "StopMoveLeft":
                CrossPlatformInputManager.SetAxis("Horizontal", 0f);
                leftArrow.color = new Color(1f, 1f, 1f, 0.7f);
                break;

            case "StopMoveRight":
                CrossPlatformInputManager.SetAxis("Horizontal", 0f);
                rightArrow.color = new Color(1f, 1f, 1f, 0.7f);
                break;

            case "LightAttack":
                if(!dragged)
                {
                    CrossPlatformInputManager.SetButtonDown("Fire1");
                    resetSelf = true;
                    resetParameter = "LightAttack";
                }

                dragged = false;
                break;

            case "HeavyAttack":
                CrossPlatformInputManager.SetButtonDown("Fire2");
                resetSelf = true;
                resetParameter = "HeavyAttack";

                dragged = true;
                break;

            case "Jump":
                CrossPlatformInputManager.SetButtonDown("Jump");
                resetSelf = true;
                resetParameter = "Jump";
                break;

            case "SlashAttack":
                CrossPlatformInputManager.SetButtonDown("Fire4");
                resetSelf = true;
                resetParameter = "SlashAttack";
                break;

            case "Block":
                CrossPlatformInputManager.SetButtonDown("Fire3");
                break;

            case "BlockRelease":
                CrossPlatformInputManager.SetButtonUp("Fire3");
                break;
        }
    }

    void Update()
    {
        //For allowing of clicking buttons/indicator systems:
        currentStamina = tempMove.currentStamina;
        staminaList = tempMove.staminaList;

        //Slash Button and Indicator
        if (currentStamina >= staminaList[1])
        {
            slashButton.interactable = true;
            slashIndicator.color = Color.white;
        }
        else
        {
            slashButton.interactable = false;
            slashIndicator.color = chargingColor;
        }

        //Block Button
        if (currentStamina > 0f && tempMove.canStaminaBlock)
        {
            blockButton.interactable = true;
            blockIndicator.color = Color.white;
        }
        else
        {
            blockButton.interactable = false;
            ExecuteCommand("BlockRelease");
            blockIndicator.color = chargingColor;
        }

        //Jump Slam Attack Indicator
        if (currentStamina >= staminaList[0])
            jumpSlamIndicator.color = Color.white;
        else
            jumpSlamIndicator.color = chargingColor;

        //Air Attack Indicator
        if (currentStamina >= staminaList[2])
            airAttackIndicator.color = Color.white;
        else
            airAttackIndicator.color = chargingColor;

        jumpSlamIndicator.fillAmount = currentStamina / staminaList[0];
        airAttackIndicator.fillAmount = currentStamina / staminaList[2];
        slashIndicator.fillAmount = currentStamina / staminaList[1];
        blockIndicator.fillAmount = currentStamina / minStaminaBlock;
    }

    void LateUpdate()
    {
        if(resetSelf)
        {
            switch (resetParameter)
            {
                case "Jump":
                    CrossPlatformInputManager.SetButtonUp("Jump");
                    break;

                case "SlashAttack":
                    CrossPlatformInputManager.SetButtonUp("Fire4");
                    break;

                case "LightAttack":
                    CrossPlatformInputManager.SetButtonUp("Fire1");
                    break;

                case "HeavyAttack":
                    CrossPlatformInputManager.SetButtonUp("Fire2");
                    break;
            }

            resetSelf = false;
        }
    }
}
