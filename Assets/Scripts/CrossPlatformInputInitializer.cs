using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;

public class CrossPlatformInputInitializer : MonoBehaviour
{
    public GameObject touchControls;
    CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;

    CrossPlatformInputManager.VirtualButton jumpButton;
    CrossPlatformInputManager.VirtualButton fire1Button;
    CrossPlatformInputManager.VirtualButton fire2Button;
    CrossPlatformInputManager.VirtualButton fire3Button;

    void Start()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        touchControls.SetActive(false);
#endif

        //Register Horizontal Axis
        horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis("Horizontal");
        CrossPlatformInputManager.RegisterVirtualAxis(horizontalVirtualAxis);

        //Register Jump
        jumpButton = new CrossPlatformInputManager.VirtualButton("Jump");
        CrossPlatformInputManager.RegisterVirtualButton(jumpButton);

        //Register Fire1
        fire1Button = new CrossPlatformInputManager.VirtualButton("Fire1");
        CrossPlatformInputManager.RegisterVirtualButton(fire1Button);

        //Register Fire2
        fire2Button = new CrossPlatformInputManager.VirtualButton("Fire2");
        CrossPlatformInputManager.RegisterVirtualButton(fire2Button);

        //Register Fire3
        fire3Button = new CrossPlatformInputManager.VirtualButton("Fire3");
        CrossPlatformInputManager.RegisterVirtualButton(fire3Button);
    }
}
