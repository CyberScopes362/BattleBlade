using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class HeroSkinner : MonoBehaviour
{
    public HeroParts heroParts;
    public List<Armor> allArmor = new List<Armor>();

    [System.Serializable]
    public class Armor
    {
        public string armorSet;
        public Sprite mask;
        public Sprite body;
        public Sprite shoulder;
        public Sprite arm;
        public Sprite upperLeg;
        public Sprite lowerLeg;
        public Sprite boot;
    }

    void SetArmor(string armorType, string armorName)
    {

    }

    /*
    void Start()
    {
        ChangeMask(flameAdvanced);
        ChangeBody(flameAdvanced);
        ChangeArmR(flameAdvanced);
        ChangeArmL(flameAdvanced);
        ChangeLegR(flameAdvanced);
        ChangeLegL(flameAdvanced);
    }

    void ChangeMask(Sprite[] changeSelected)
    {
        heroParts.mask.sprite = changeSelected[0];
    }

    void ChangeBody(Sprite[] changeSelected)
    {
        heroParts.frontBody.sprite = changeSelected[1];
        heroParts.backBody.sprite = changeSelected[1];
    }

    void ChangeArmR(Sprite[] changeSelected)
    {
        heroParts.rShoulder.sprite = changeSelected[2];
        heroParts.rArm.sprite = changeSelected[3];
    }

    void ChangeArmL(Sprite[] changeSelected)
    {
        heroParts.lShoulder.sprite = changeSelected[2];
        heroParts.lArm.sprite = changeSelected[3];
    }

    void ChangeLegR(Sprite[] changeSelected)
    {
        heroParts.rUpperLeg.sprite = changeSelected[4];
        heroParts.rLowerLeg.sprite = changeSelected[5];
        heroParts.rFoot.sprite = changeSelected[6];
    }

    void ChangeLegL(Sprite[] changeSelected)
    {
        heroParts.lUpperLeg.sprite = changeSelected[4];
        heroParts.lLowerLeg.sprite = changeSelected[5];
        heroParts.lFoot.sprite = changeSelected[6];
    }
    */
}

[System.Serializable]
public class HeroParts
{
    public SpriteRenderer mask;
    public SpriteRenderer frontBody;
    public SpriteRenderer backBody;
    public SpriteRenderer rShoulder;
    public SpriteRenderer rArm;
    public SpriteRenderer lShoulder;
    public SpriteRenderer lArm;
    public SpriteRenderer rUpperLeg;
    public SpriteRenderer rLowerLeg;
    public SpriteRenderer rFoot;
    public SpriteRenderer lUpperLeg;
    public SpriteRenderer lLowerLeg;
    public SpriteRenderer lFoot;
}


