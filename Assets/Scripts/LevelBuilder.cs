using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelBuilder : MonoBehaviour 
{
    Material mainSkybox;

    GameObject floor;
    GameObject gradientTop;
    GameObject gradientBottom;
    GameObject startPoint;
    GameObject endPoint;

    GameObject backgroundScenery;
    GameObject midgroundScenery;
    GameObject foregroundScenery;
    GameObject actualgroundScenery;

    GameObject backgroundSceneryParent;
    GameObject midgroundSceneryParent;
    GameObject foregroundSceneryParent;
    GameObject actualgroundSceneryParent;

    public List<LevelComponents> allLevels = new List<LevelComponents>();

    public float floorSizeX;
    public float pointGapWidth;
    public float parallaxGap;

    public string environmentType;

    GameObject setScenery;
    GameObject setSceneryParent;

    SpriteRenderer floorRenderer;

    List<GameObject> createdScenery = new List<GameObject>();

    //For fading
    List<SpriteRenderer> allFadeSpritesBG = new List<SpriteRenderer>();
    List<SpriteRenderer> allFadeSpritesMG = new List<SpriteRenderer>();
    List<SpriteRenderer> allFadeSpritesFG = new List<SpriteRenderer>();
    List<SpriteRenderer> allFadeSpritesAG = new List<SpriteRenderer>();
    Color[] newColorSet = new Color[4];
    Sprite[] newSpriteSet = new Sprite[4];
    Color[] newColor = new Color[4];
    Color gradientColor;
    float colorAlpha;
    public bool _switch;
    Sprite[] gradients = new Sprite[2];




    [System.Serializable]
    public class LevelComponents
    {
        public string name;
        public Texture skyboxGradient;
        public Sprite bgSprite;
        public Sprite mgSprite;
        public Sprite fgSprite;
        public Sprite agSprite;
        public Color bgColor;
        public Color mgColor;
        public Color fgColor;
        public Color floorAuraColor;
        public GameObject skyEffect;
    }

    void Start()
    {
        //Get Objects from Finder Script
        ObjectFinder objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        floor = objectFinder.floor;
        gradientTop = objectFinder.gradientTop;
        gradientBottom = objectFinder.gradientBottom;
        startPoint = objectFinder.startPoint;
        endPoint = objectFinder.endPoint;

        backgroundScenery = objectFinder.backgroundScenery;
        midgroundScenery = objectFinder.midgroundScenery;
        foregroundScenery = objectFinder.foregroundScenery;
        actualgroundScenery = objectFinder.actualgroundScenery;

        backgroundSceneryParent = objectFinder.backgroundSceneryParent;
        midgroundSceneryParent = objectFinder.midgroundSceneryParent;
        foregroundSceneryParent = objectFinder.foregroundSceneryParent;
        actualgroundSceneryParent = objectFinder.actualgroundSceneryParent;

        floorRenderer = floor.GetComponent<SpriteRenderer>();


        foreach (LevelComponents level in allLevels)
        {
            if(level.name == environmentType)
            {
                RenderSettings.skybox.SetTexture("_FrontTex", level.skyboxGradient);
                RenderSettings.skybox.SetFloat("_Blend", 0f);
                Instantiate(level.skyEffect, level.skyEffect.transform.localPosition + backgroundScenery.transform.position, level.skyEffect.transform.localRotation, backgroundScenery.transform);

                backgroundScenery.GetComponent<SpriteRenderer>().sprite = level.bgSprite;
                midgroundScenery.GetComponent<SpriteRenderer>().sprite = level.mgSprite;
                foregroundScenery.GetComponent<SpriteRenderer>().sprite = level.fgSprite;
                actualgroundScenery.GetComponent<SpriteRenderer>().sprite = level.agSprite;

                backgroundScenery.GetComponent<SpriteRenderer>().color = level.bgColor;
                midgroundScenery.GetComponent<SpriteRenderer>().color = level.mgColor;
                foregroundScenery.GetComponent<SpriteRenderer>().color = level.fgColor;

                gradientTop.GetComponent<SpriteRenderer>().color = gradientBottom.GetComponent<SpriteRenderer>().color = level.floorAuraColor;
                break;
            }
        }
  
        Vector3 vectorFloorSize = new Vector3(floorSizeX, floor.transform.localScale.y, floor.transform.localScale.z);

        floor.transform.localScale = vectorFloorSize;
        floor.transform.position = new Vector3(0f, 0f, 0f);

        float floorWidth = floorRenderer.bounds.size.x;
        float floorWidthMax = floorRenderer.bounds.max.x;
        float floorWidthMin = floorRenderer.bounds.min.x;
        float floorHeightMax = floorRenderer.bounds.max.y;

        startPoint.transform.position = new Vector3(floorWidthMin + pointGapWidth, 0.5f, 1f);
        endPoint.transform.position = new Vector3(floorWidthMax - pointGapWidth, 0.5f, 1f);

        //Use back scenery as template for all cause theyre the same size
        SpriteRenderer sceneryRenderer = backgroundScenery.GetComponent<SpriteRenderer>();

        //Scenery Auto-Adjust for levels
        Vector3 groundsPosition = new Vector3(floorWidthMin + sceneryRenderer.bounds.extents.x + parallaxGap, floorHeightMax + sceneryRenderer.bounds.extents.y, 1f);

        backgroundScenery.transform.position = groundsPosition;
        midgroundScenery.transform.position = groundsPosition;
        foregroundScenery.transform.position = groundsPosition;
        actualgroundScenery.transform.position = new Vector2(groundsPosition.x, groundsPosition.y - 0.06f);

        //Scenery Multiplier
        float totalSceneryWidth = sceneryRenderer.bounds.size.x;
        int i = 1;

        //Use lists instead of arrays; Flexibility and dynamic growth
        //For loop for each ground, while loop for each scenery in section
        for (int x = 0; x < 4; x++)
        {
            //Setting for each ground
            switch(x)
            {
                case 0:
                    setScenery = backgroundScenery;
                    setSceneryParent = backgroundSceneryParent;
                    break;

                case 1:
                    setScenery = midgroundScenery;
                    setSceneryParent = midgroundSceneryParent;
                    break;

                case 2:
                    setScenery = foregroundScenery;
                    setSceneryParent = foregroundSceneryParent;
                    break;

                case 3:
                    setScenery = actualgroundScenery;
                    setSceneryParent = actualgroundSceneryParent;
                    break;
            }

            //Refresh values
            createdScenery.Clear();
            createdScenery.Add(setScenery);
            totalSceneryWidth = sceneryRenderer.bounds.size.x;
            i = 1;

            while (totalSceneryWidth < floorWidth)
            {
                //Instantiate variable
                var temp = Instantiate(setScenery);

                //Add Instantiation to list
                createdScenery.Add(temp);

                //Put instantiation in respective parent
                createdScenery[i].transform.SetParent(setSceneryParent.transform);

                //Change position of instantiation
                createdScenery[i].transform.position = new Vector3(createdScenery[i - 1].transform.position.x + sceneryRenderer.bounds.size.x, createdScenery[i - 1].transform.position.y, 1f);

                //Progress values
                totalSceneryWidth += sceneryRenderer.bounds.size.x;
                i += 1;
            }
        }

        //Making these arrays now for the fading system
        foreach (Transform child in backgroundSceneryParent.transform)
            allFadeSpritesBG.Add(child.GetComponent<SpriteRenderer>());

        foreach (Transform child in midgroundSceneryParent.transform)
            allFadeSpritesMG.Add(child.GetComponent<SpriteRenderer>());

        foreach (Transform child in foregroundSceneryParent.transform)
            allFadeSpritesFG.Add(child.GetComponent<SpriteRenderer>());

        foreach (Transform child in actualgroundSceneryParent.transform)
            allFadeSpritesAG.Add(child.GetComponent<SpriteRenderer>());

        gradientColor = gradientTop.GetComponent<SpriteRenderer>().color;
    }

    void Update()
    {
        //Temp switch to change levels
        if (_switch)
        {
            LevelSwitch(environmentType);
            _switch = false;
        }
    }



    public void LevelSwitch(string levelName)
    {
        foreach (LevelComponents level in allLevels)
        {
            if (level.name == levelName)
            {
                RenderSettings.skybox.SetTexture("_FrontTex2", level.skyboxGradient);
                foreach(Transform childMain in backgroundSceneryParent.transform)
                {
                    foreach(Transform child in childMain.transform)
                    {
                        child.gameObject.GetComponent<ParticleSystem>().Stop();
                        child.gameObject.GetComponent<KillAfterTime>().enabled = true;
                    }

                    Instantiate(level.skyEffect, level.skyEffect.transform.localPosition + childMain.transform.position, level.skyEffect.transform.localRotation, childMain.transform);
                }
                

                newColorSet[0] = level.bgColor;
                newColorSet[1] = level.mgColor;
                newColorSet[2] = level.fgColor;
                newColorSet[3] = level.floorAuraColor;

                newSpriteSet[0] = level.bgSprite;
                newSpriteSet[1] = level.mgSprite;
                newSpriteSet[2] = level.fgSprite;
                newSpriteSet[3] = level.agSprite;
            }
        }

        colorAlpha = allFadeSpritesBG[0].color.a;
        newColor[0] = new Color(allFadeSpritesBG[0].color.r, allFadeSpritesBG[0].color.g, allFadeSpritesBG[0].color.b, colorAlpha);
        newColor[1] = new Color(allFadeSpritesMG[0].color.r, allFadeSpritesMG[0].color.g, allFadeSpritesMG[0].color.b, colorAlpha);
        newColor[2] = new Color(allFadeSpritesFG[0].color.r, allFadeSpritesFG[0].color.g, allFadeSpritesFG[0].color.b, colorAlpha);
        newColor[3] = gradientColor;

        StartCoroutine(LevelFaderA());
    }

    IEnumerator LevelFaderA()
    {
        if (colorAlpha > 0.3f)
        {
            colorAlpha -= 0.01f;
            newColor[0] = new Color(allFadeSpritesBG[0].color.r, allFadeSpritesBG[0].color.g, allFadeSpritesBG[0].color.b, colorAlpha);
            newColor[1] = new Color(allFadeSpritesMG[0].color.r, allFadeSpritesMG[0].color.g, allFadeSpritesMG[0].color.b, colorAlpha);
            newColor[2] = new Color(allFadeSpritesFG[0].color.r, allFadeSpritesFG[0].color.g, allFadeSpritesFG[0].color.b, colorAlpha);
            newColor[3] = new Color(gradientColor.r, gradientColor.g, gradientColor.b, colorAlpha);

            foreach (SpriteRenderer rend in allFadeSpritesBG)
                rend.color = newColor[0];

            foreach (SpriteRenderer rend in allFadeSpritesMG)
                rend.color = newColor[1];

            foreach (SpriteRenderer rend in allFadeSpritesFG)
                rend.color = newColor[2];

            foreach (SpriteRenderer rend in allFadeSpritesAG)
                rend.color = new Color(0, 0, 0, colorAlpha);

            gradientTop.GetComponent<SpriteRenderer>().color = gradientBottom.GetComponent<SpriteRenderer>().color = newColor[3];

            yield return 0;
            StartCoroutine(LevelFaderA());
        }
        else
        {
            SpriteSwap();
            StartCoroutine(LevelFaderB());
            yield break;
        }
    }

    void SpriteSwap()
    {
        foreach (SpriteRenderer rend in allFadeSpritesBG)
            rend.sprite = newSpriteSet[0];

        foreach (SpriteRenderer rend in allFadeSpritesMG)
            rend.sprite = newSpriteSet[1];

        foreach (SpriteRenderer rend in allFadeSpritesFG)
            rend.sprite = newSpriteSet[2];

        foreach (SpriteRenderer rend in allFadeSpritesAG)
            rend.sprite = newSpriteSet[3];
    }

    IEnumerator LevelFaderB()
    {
        if (RenderSettings.skybox.GetFloat("_Blend") < 1f)
            RenderSettings.skybox.SetFloat("_Blend", RenderSettings.skybox.GetFloat("_Blend") + 0.02f);

        if (colorAlpha < newColorSet[0].a)
        {
            colorAlpha += 0.01f;
            newColor[0] = new Color(newColorSet[0].r, newColorSet[0].g, newColorSet[0].b, colorAlpha);
            newColor[1] = new Color(newColorSet[1].r, newColorSet[1].g, newColorSet[1].b, colorAlpha);
            newColor[2] = new Color(newColorSet[2].r, newColorSet[2].g, newColorSet[2].b, colorAlpha);
            newColor[3] = new Color(newColorSet[3].r, newColorSet[3].g, newColorSet[3].b, colorAlpha);

            foreach (SpriteRenderer rend in allFadeSpritesBG)
                rend.color = newColor[0];

            foreach (SpriteRenderer rend in allFadeSpritesMG)
                rend.color = newColor[1];

            foreach (SpriteRenderer rend in allFadeSpritesFG)
                rend.color = newColor[2];

            foreach (SpriteRenderer rend in allFadeSpritesAG)
                rend.color = new Color(0, 0, 0, colorAlpha);

            gradientTop.GetComponent<SpriteRenderer>().color = gradientBottom.GetComponent<SpriteRenderer>().color = newColor[3];

            yield return 0;
            StartCoroutine(LevelFaderB());
        }
        else
        {
            RenderSettings.skybox.SetTexture("_FrontTex", RenderSettings.skybox.GetTexture("_FrontTex2"));
            RenderSettings.skybox.SetFloat("_Blend", 0f);
            gradientColor = gradientTop.GetComponent<SpriteRenderer>().color;

            yield break;
        }
    }
}
