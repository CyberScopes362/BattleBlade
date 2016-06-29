using UnityEngine;
using System.Collections.Generic;
using Xft;
using System.Collections;

// Rarity Info:
//
// 0 = Common
// 1 = Rare
// 2 = Mythical
// 3 = Heroic
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
    XWeaponTrail heroTrail;
    GameObject slashMarks;
    TempMove tempMove;
    GameObject absoluteShield;


    void Start()
    {
        objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        slashMarks = objectFinder.slashMarks;
        absoluteShield = objectFinder.absoluteShield;
        heroTrail = objectFinder.heroTrailObj.GetComponent<XWeaponTrail>();
        weaponTrail = weaponRenderer.gameObject.GetComponentInChildren<XWeaponTrail>();
        tempMove = GetComponent<TempMove>();


        //Placeholder: Default stuff
        SetWeapon("Legacy Scorched Blade");
        SetMask("Hellrider");
        SetChestplate("Roars of Hell");
        SetArms("Combustion");
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

        //Visuals
        weaponRenderer.sprite = setWeapon.weaponSprite;
        weaponTrail.MyColor = setWeapon.trailColor;
        heroTrail.MyColor = setWeapon.trailColor;
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
        tempMove.knockbackRatio = setWeapon.knockbackRatio;

        //Value Addons
        tempMove.movS += setWeapon.movAdd;
        tempMove.atkS += setWeapon.atkAdd;
        tempMove.blkS += setWeapon.blkAdd;
        tempMove.blkbstS += setWeapon.blkbstAdd;
        tempMove.hthS += setWeapon.hthAdd;
    }

    public void SetMask(string name)
    {
        Masks setMask = allMasks.Find(x => x.name == name);

        if (setMask == null)
        {
            print("Invalid Mask Choice.");
            return;
        }

        //Visuals
        maskRenderer.sprite = setMask.maskSprite;

        //Values

        //Value Addons
        tempMove.movS += setMask.movAdd;
        tempMove.atkS += setMask.atkAdd;
        tempMove.blkS += setMask.blkAdd;
        tempMove.blkbstS += setMask.blkbstAdd;
        tempMove.hthS += setMask.hthAdd;
    }

    public void SetChestplate(string name)
    {
        Chestplate setChestplate = allChestplates.Find(x => x.name == name);

        if (setChestplate == null)
        {
            print("Invalid Chestplate Choice.");
            return;
        }

        chestplateRenderer[0].sprite = setChestplate.body;
        chestplateRenderer[1].sprite = setChestplate.body;

        //Value Addons
        tempMove.movS += setChestplate.movAdd;
        tempMove.atkS += setChestplate.atkAdd;
        tempMove.blkS += setChestplate.blkAdd;
        tempMove.blkbstS += setChestplate.blkbstAdd;
        tempMove.hthS += setChestplate.hthAdd;
    }

    public void SetArms(string name)
    {
        Arms setArms = allArms.Find(x => x.name == name);

        if (setArms == null)
        {
            print("Invalid Arms Choice.");
            return;
        }

        armsRenderer[0].sprite = setArms.shoulder;
        armsRenderer[1].sprite = setArms.arm;
        armsRenderer[2].sprite = setArms.shoulder;
        armsRenderer[3].sprite = setArms.arm;

        //Value Addons
        tempMove.movS += setArms.movAdd;
        tempMove.atkS += setArms.atkAdd;
        tempMove.blkS += setArms.blkAdd;
        tempMove.blkbstS += setArms.blkbstAdd;
        tempMove.hthS += setArms.hthAdd;
    }

    public void SetLegs(string name)
    {
        Legs setLegs = allLegs.Find(x => x.name == name);

        if (setLegs == null)
        {
            print("Invalid Legs Choice.");
            return;
        }

        legsRenderer[0].sprite = setLegs.upperLeg;
        legsRenderer[1].sprite = setLegs.lowerLeg;
        legsRenderer[2].sprite = setLegs.boot;
        legsRenderer[3].sprite = setLegs.upperLeg;
        legsRenderer[4].sprite = setLegs.lowerLeg;
        legsRenderer[5].sprite = setLegs.boot;

        //Value Addons
        tempMove.movS += setLegs.movAdd;
        tempMove.atkS += setLegs.atkAdd;
        tempMove.blkS += setLegs.blkAdd;
        tempMove.blkbstS += setLegs.blkbstAdd;
        tempMove.hthS += setLegs.hthAdd;
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

        public float movAdd;
        public float atkAdd;
        public float blkAdd;
        public float blkbstAdd;
        public float hthAdd;
    }

    [System.Serializable]
    public class Masks
    {
        public string name;
        public int rarity;
        public string description;

        public Sprite maskSprite;

        public float movAdd;
        public float atkAdd;
        public float blkAdd;
        public float blkbstAdd;
        public float hthAdd;
    }

    [System.Serializable]
    public class Chestplate
    {
        public string name;
        public int rarity;
        public string description;

        public Sprite body;

        public float movAdd;
        public float atkAdd;
        public float blkAdd;
        public float blkbstAdd;
        public float hthAdd;
    }

    [System.Serializable]
    public class Arms
    {
        public string name;
        public int rarity;
        public string description;

        public Sprite shoulder;
        public Sprite arm;

        public float movAdd;
        public float atkAdd;
        public float blkAdd;
        public float blkbstAdd;
        public float hthAdd;
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

        public float movAdd;
        public float atkAdd;
        public float blkAdd;
        public float blkbstAdd;
        public float hthAdd;
    }
}
