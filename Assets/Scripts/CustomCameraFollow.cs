using UnityEngine;
using System.Collections;

public class CustomCameraFollow : MonoBehaviour
{
    Transform hero;

    Vector3 setPosition;
    Vector3 currentPosition;
    Vector3 prevTransform;

    public Vector3 offset;
    public float offsetYPreset;
    public float smoothSpeed;
    public float camPointGap;
    public float xOffset;

    float startPointX;
    float endPointX;

    GameObject startPoint;
    GameObject endPoint;

    void Start()
    {
        ObjectFinder objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        hero = objectFinder.hero.transform;
        startPoint = objectFinder.startPoint;
        endPoint = objectFinder.endPoint;

        float camHalfWidth = (Camera.main.orthographicSize * 2f * Camera.main.aspect) / 2f;

        startPointX = startPoint.transform.position.x + (camHalfWidth - camPointGap);
        endPointX = endPoint.transform.position.x - (camHalfWidth - camPointGap);
    }

    //Fixed Update for smooth interpolation and sync with hero
    void FixedUpdate()
    {
        setPosition = new Vector3(hero.position.x + offset.x, transform.position.y, offset.z);

        //Movement related to start position
        if (prevTransform.x < startPointX || prevTransform.x > endPointX)
        {
            prevTransform = Vector3.SmoothDamp(prevTransform, setPosition, ref currentPosition, smoothSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, (hero.position.y + offset.y) * offsetYPreset, transform.position.z);
        }
        else
        {
            prevTransform = transform.position;
            transform.position = new Vector3(transform.position.x, (hero.position.y + offset.y) * offsetYPreset, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, setPosition, ref currentPosition, smoothSpeed * Time.deltaTime);
        }

        


        if (hero.transform.localScale.x == -1)
            offset.x = -xOffset;
        else
            offset.x = xOffset;
    }
}
