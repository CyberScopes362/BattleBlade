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

    public Button slashButton;
    public Button blockButton;

    public Color grayedOutColor;

    bool resetSelf = false;
    string resetParameter;

    bool dragged = false;

    void Start()
    {
        tempMove = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>().hero.GetComponent<TempMove>();

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

        //Slash Button
        if (tempMove.currentStamina >= tempMove.staminaList[1])
            slashButton.interactable = true;
        else
            slashButton.interactable = false;

        //Block Button
        if (tempMove.currentStamina > 0f && tempMove.canStaminaBlock)
            blockButton.interactable = true;
        else
        {
            blockButton.interactable = false;
            ExecuteCommand("BlockRelease");
        }

        //Jump Slam Attack Indicator
        if (tempMove.currentStamina >= tempMove.staminaList[0])
            jumpSlamIndicator.color = Color.white;
        else
            jumpSlamIndicator.color = grayedOutColor;

        //Air Attack Indicator
        if (tempMove.currentStamina >= tempMove.staminaList[2])
            airAttackIndicator.color = Color.white;
        else
            airAttackIndicator.color = grayedOutColor;
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
