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

    public List<Texture> skyboxTextures = new List<Texture>();

    public List<Sprite> backgroundSprites = new List<Sprite>();
    public List<Sprite> midgroundSprites = new List<Sprite>();
    public List<Sprite> foregroundSprites = new List<Sprite>();
    public List<Sprite> actualgroundSprites = new List<Sprite>();

    public List<Color> backgroundSpritesColors = new List<Color>();
    public List<Color> midgroundSpritesColors = new List<Color>();
    public List<Color> foregroundSpritesColors = new List<Color>();
    public List<Color> gradientColors = new List<Color>();
    public List<GameObject> skyEffects = new List<GameObject>();

    public float floorSizeX;
    public float pointGapWidth;
    public float parallaxGap;

    public string environmentType;

    GameObject setScenery;
    GameObject setSceneryParent;

    SpriteRenderer floorRenderer;

    List<GameObject> createdScenery = new List<GameObject>();

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

        //Set Environment Type
        switch (environmentType)
        {
            case "Mountains":
                RenderSettings.skybox.SetTexture("_FrontTex", skyboxTextures[0]);
                skyEffects[0].SetActive(true);

                backgroundScenery.GetComponent<SpriteRenderer>().sprite = backgroundSprites[0];
                midgroundScenery.GetComponent<SpriteRenderer>().sprite = midgroundSprites[0];
                foregroundScenery.GetComponent<SpriteRenderer>().sprite = foregroundSprites[0];
                actualgroundScenery.GetComponent<SpriteRenderer>().sprite = actualgroundSprites[0];

                backgroundScenery.GetComponent<SpriteRenderer>().color = backgroundSpritesColors[0];
                midgroundScenery.GetComponent<SpriteRenderer>().color = midgroundSpritesColors[0];
                foregroundScenery.GetComponent<SpriteRenderer>().color = foregroundSpritesColors[0];

                gradientTop.GetComponent<SpriteRenderer>().color = gradientColors[0];
                gradientBottom.GetComponent<SpriteRenderer>().color = gradientColors[0];
                break;

            case "Valleys":
                RenderSettings.skybox.SetTexture("_FrontTex", skyboxTextures[1]);
                skyEffects[1].SetActive(true);

                backgroundScenery.GetComponent<SpriteRenderer>().sprite = backgroundSprites[1];
                midgroundScenery.GetComponent<SpriteRenderer>().sprite = midgroundSprites[1];
                foregroundScenery.GetComponent<SpriteRenderer>().sprite = foregroundSprites[1];
                actualgroundScenery.GetComponent<SpriteRenderer>().sprite = actualgroundSprites[1];

                backgroundScenery.GetComponent<SpriteRenderer>().color = backgroundSpritesColors[1];
                midgroundScenery.GetComponent<SpriteRenderer>().color = midgroundSpritesColors[1];
                foregroundScenery.GetComponent<SpriteRenderer>().color = foregroundSpritesColors[1];

                gradientTop.GetComponent<SpriteRenderer>().color = gradientColors[1];
                gradientBottom.GetComponent<SpriteRenderer>().color = gradientColors[1];
                break;
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
    }
}
