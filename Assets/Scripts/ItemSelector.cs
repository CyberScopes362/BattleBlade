using UnityEngine;
using System.Collections.Generic;
using Xft;
using System.Collections;

// Rarity Info:
//
// 0 = Common
// 1 = Rare
// 2 = Heroic
// 3 = Mythical
//
//

public class ItemSelector : MonoBehaviour
{
    ObjectFinder objectFinder;

    public SpriteRenderer weaponRenderer;
    public SpriteRenderer maskRenderer;
    public SpriteRenderer[] armorRenderer;

    public List<Weapons> allWeapons = new List<Weapons>();
    public List<Masks> allMasks = new List<Masks>();
    public List<Armor> allArmor = new List<Armor>();

    public string currentWeapon;
    public string currentMask;
    public string currentArmor;
    public bool invokeChanges;

    XWeaponTrail weaponTrail;
    GameObject slashMarks;


    void Start()
    {
        objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        slashMarks = objectFinder.slashMarks;
        weaponTrail = weaponRenderer.gameObject.GetComponentInChildren<XWeaponTrail>();

#if UNITY_EDITOR
        //
        //Create system to save and load allweapons from an asset file.
        //
#endif

        SetWeapon("Requiem Slicer");
        SetMask("Hellrider");
        //SetArmor("Advanced Flame");
    }


    //This update script is for the inspector (Active changing)
    void Update()
    {
        if(invokeChanges)
        {
            if (currentWeapon != "")
                SetWeapon(currentWeapon);

            if (currentMask != "")
                SetMask(currentMask);

            // if (currentArmor != "")
            //     SetArmor(currentArmor);

            invokeChanges = false;
        }
    }


    //Lambda operation to find equipment names.
    public void SetWeapon(string name)
    {
        Weapons setWeapon = allWeapons.Find(x => x.name == name);

        if(setWeapon == null)
        {
            print("Invalid Weapon Choice.");
            return;
        }
        else
            print("Weapon Changed To: " + setWeapon.name);

        weaponRenderer.sprite = setWeapon.weaponSprite;
        weaponTrail.MyColor = setWeapon.trailColor;
        slashMarks.GetComponent<SpriteRenderer>().sprite = setWeapon.slashSprite;
        slashMarks.GetComponent<SpriteRenderer>().color = setWeapon.slashColor;
        slashMarks.GetComponent<ParticleSystem>().startColor = setWeapon.slashColor;
    }

    public void SetMask(string name)
    {
        Masks setMask = allMasks.Find(x => x.name == name);

        if (setMask == null)
        {
            print("Invalid Mask Choice.");
            return;
        }
        else
            print("Mask Changed To: " + setMask.name);

        maskRenderer.sprite = setMask.maskSprite;
    }

    /*  public void SetArmor(string name)
      {
          Armor setArmor = allArmor.Find(x => x.name == name);

          armorRenderer[0].sprite = setArmor.mask;
          armorRenderer[1].sprite = setArmor.body;
          armorRenderer[2].sprite = setArmor.body;
          armorRenderer[3].sprite = setArmor.shoulder;
          armorRenderer[4].sprite = setArmor.arm;
          armorRenderer[5].sprite = setArmor.shoulder;
          armorRenderer[6].sprite = setArmor.arm;
          armorRenderer[7].sprite = setArmor.upperLeg;
          armorRenderer[8].sprite = setArmor.lowerLeg;
          armorRenderer[9].sprite = setArmor.boot;
          armorRenderer[10].sprite = setArmor.upperLeg;
          armorRenderer[11].sprite = setArmor.lowerLeg;
          armorRenderer[12].sprite = setArmor.boot;
      }*/


    [System.Serializable]
    public class Weapons
    {
        public string name;
        public int rarity;
        public string description;

        public Sprite weaponSprite;
        public Color trailColor;
        public Sprite slashSprite;
        public Color slashColor;
        public float lightAttackDamage;
        public float heavyAttackDamage;
        public float slamAttackDamage;
        public float knockbackRatio;
    }

    [System.Serializable]
    public class Masks
    {
        public string name;
        public int rarity;
        public string description;
        public string effects;

        public Sprite maskSprite;
    }

    [System.Serializable]
    public class Armor
    {
        public string name;
        public int rarity;
        public string description;
        public string effects;

        public Sprite mask;
        public Sprite body;
        public Sprite shoulder;
        public Sprite arm;
        public Sprite upperLeg;
        public Sprite lowerLeg;
        public Sprite boot;
    }
}
