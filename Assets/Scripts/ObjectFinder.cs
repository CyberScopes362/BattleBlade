using UnityEngine;
using Xft;
using System.Collections;

public class ObjectFinder : MonoBehaviour
{
    public GameObject floor;
    public GameObject gradientTop;
    public GameObject gradientBottom;

    public GameObject hero;

    public GameObject backgroundSceneryParent;
    public GameObject midgroundSceneryParent;
    public GameObject foregroundSceneryParent;

    public GameObject backgroundScenery;
    public GameObject midgroundScenery;
    public GameObject foregroundScenery;

    public GameObject startPoint;
    public GameObject endPoint;
    public GameObject spawnPoint;

    public LayerMask floorMask;
    public int layerOrder = 0;
    public GameObject slashMarks;
}
