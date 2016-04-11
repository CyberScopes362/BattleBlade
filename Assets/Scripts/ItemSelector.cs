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
    public SpriteRenderer[] chestplateRenderer;
    public SpriteRenderer[] armsRenderer;
    public SpriteRenderer[] legsRenderer;


    public List<Weapons> allWeapons = new List<Weapons>();
    public List<Masks> allMasks = new List<Masks>();
    public List<Chestplate> allChestplates = new List<Chestplate>();
    public List<Arms> allArms = new List<Arms>();
    public List<Legs> allLegs = new List<Legs>();


    public string currentWeapon;
    public string currentMask;
    public string currentChestplate;
    public string currentArms;
    public string currentLegs;

    public bool invokeChanges;

    XWeaponTrail weaponTrail;
    GameObject slashMarks;
    TempMove tempMove;
    GameObject absoluteShield;


    void Start()
    {
        objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        slashMarks = objectFinder.slashMarks;
        absoluteShield = objectFinder.absoluteShield;
        weaponTrail = weaponRenderer.gameObject.GetComponentInChildren<XWeaponTrail>();
        tempMove = GetComponent<TempMove>();


        //Placeholder: Default stuff
        SetWeapon("Crystalized Frostsbite");
        SetMask("Hellrider");
        SetChestplate("Roars of Hell");
        SetArms("Exo Arms");
        SetLegs("Purging Rage");
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

            if (currentChestplate != "")
                SetChestplate(currentChestplate);

            if (currentArms != "")
                SetArms(currentArms);

            if (currentLegs != "")
                SetLegs(currentLegs);

            invokeChanges = false;
        }
    }

    
    public void SetWeapon(string name)
    {
        //Lambda operation to find equipment names.
        Weapons setWeapon = allWeapons.Find(x => x.name == name);

        if(setWeapon == null)
        {
            print("Invalid Weapon Choice.");
            return;
        }
        else
            print("Weapon Changed To: " + setWeapon.name);

        //Visuals
        weaponRenderer.sprite = setWeapon.weaponSprite;
        weaponTrail.MyColor = setWeapon.trailColor;
        slashMarks.GetComponent<SpriteRenderer>().sprite = setWeapon.slashSprite;
        slashMarks.GetComponent<SpriteRenderer>().color = setWeapon.slashColor;
        slashMarks.GetComponent<ParticleSystem>().startColor = setWeapon.slashColor;
        slashMarks.GetComponent<ParticleSystemRenderer>().sharedMaterial.mainTexture = setWeapon.slashParticlesTexture;
        absoluteShield.GetComponent<AbsoluteShieldEffector>().setColor = setWeapon.slashColor;

        //Values
        tempMove.lightAttackStrength = setWeapon.lightAttackDamage;
        tempMove.heavyAttackStrength = setWeapon.heavyAttackDamage;
        tempMove.slamAttackStrength = setWeapon.slamAttackDamage;
        tempMove.criticalChance = setWeapon.criticalChance;
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

    public void SetChestplate(string name)
    {
        Chestplate setChestplate = allChestplates.Find(x => x.name == name);

        if (setChestplate == null)
        {
            print("Invalid Chestplate Choice.");
            return;
        }
        else
            print("Chestplate Changed To: " + setChestplate.name);

        chestplateRenderer[0].sprite = setChestplate.body;
        chestplateRenderer[1].sprite = setChestplate.body;
    }

    public void SetArms(string name)
    {
        Arms setArms = allArms.Find(x => x.name == name);

        if (setArms == null)
        {
            print("Invalid Arms Choice.");
            return;
        }
        else
            print("Arms Changed To: " + setArms.name);

        armsRenderer[0].sprite = setArms.shoulder;
        armsRenderer[1].sprite = setArms.arm;
        armsRenderer[2].sprite = setArms.shoulder;
        armsRenderer[3].sprite = setArms.arm;
    }

    public void SetLegs(string name)
    {
        Legs setLegs = allLegs.Find(x => x.name == name);

        if (setLegs == null)
        {
            print("Invalid Legs Choice.");
            return;
        }
        else
            print("Legs Changed To: " + setLegs.name);

        legsRenderer[0].sprite = setLegs.upperLeg;
        legsRenderer[1].sprite = setLegs.lowerLeg;
        legsRenderer[2].sprite = setLegs.boot;
        legsRenderer[3].sprite = setLegs.upperLeg;
        legsRenderer[4].sprite = setLegs.lowerLeg;
        legsRenderer[5].sprite = setLegs.boot;
    }




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
        public Texture slashParticlesTexture;
        public float lightAttackDamage;
        public float heavyAttackDamage;
        public float slamAttackDamage;
        public float knockbackRatio;
        public float criticalChance;
    }

    [System.Serializable]
    public class Masks
    {
        public string name;
        public int rarity;
        public string description;

        public Sprite maskSprite;
    }

    [System.Serializable]
    public class Chestplate
    {
        public string name;
        public int rarity;
        public string description;

        public Sprite body;
    }

    [System.Serializable]
    public class Arms
    {
        public string name;
        public int rarity;
        public string description;

        public Sprite shoulder;
        public Sprite arm;

    }

    [System.Serializable]
    public class Legs
    {
        public string name;
        public int rarity;
        public string description;

        public Sprite upperLeg;
        public Sprite lowerLeg;
        public Sprite boot;
    }
}
