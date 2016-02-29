using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelBuilder : MonoBehaviour 
{
    Material mainSkybox;

    GameObject floor;
    GameObject startPoint;
    GameObject endPoint;
    GameObject backgroundScenery;
    GameObject midgroundScenery;
    GameObject foregroundScenery;
    GameObject backgroundSceneryParent;
    GameObject midgroundSceneryParent;
    GameObject foregroundSceneryParent;

    public List<Texture> skyboxTextures = new List<Texture>();
    public List<Sprite> backgroundSprites = new List<Sprite>();
    public List<Sprite> midgroundSprites = new List<Sprite>();
    public List<Sprite> foregroundSprites = new List<Sprite>();

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
        startPoint = objectFinder.startPoint;
        endPoint = objectFinder.endPoint;
        backgroundScenery = objectFinder.backgroundScenery;
        midgroundScenery = objectFinder.midgroundScenery;
        foregroundScenery = objectFinder.foregroundScenery;
        backgroundSceneryParent = objectFinder.backgroundSceneryParent;
        midgroundSceneryParent = objectFinder.midgroundSceneryParent;
        foregroundSceneryParent = objectFinder.foregroundSceneryParent;

        floorRenderer = floor.GetComponent<SpriteRenderer>();

        //Set Environment Type
        switch (environmentType)
        {
            case "Mountains":
                RenderSettings.skybox.SetTexture("_FrontTex", skyboxTextures[0]);
                backgroundScenery.GetComponent<SpriteRenderer>().sprite = backgroundSprites[0];
                midgroundScenery.GetComponent<SpriteRenderer>().sprite = midgroundSprites[0];
                foregroundScenery.GetComponent<SpriteRenderer>().sprite = foregroundSprites[0];
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

        //Scenery Multiplier
        float totalSceneryWidth = sceneryRenderer.bounds.size.x;
        int i = 1;

        //Use lists instead of arrays; Flexibility and dynamic growth
        //For loop for each ground, while loop for each scenery in section
        for (int x = 0; x < 3; x++)
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
