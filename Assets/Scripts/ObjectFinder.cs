using UnityEngine;
using Xft;
using System.Collections;

public class ObjectFinder : MonoBehaviour
{
    public GameObject floor;
    public GameObject gradientTop;
    public GameObject gradientBottom;

    public GameObject hero;
    public GameObject inventory;

    public GameObject backgroundSceneryParent;
    public GameObject midgroundSceneryParent;
    public GameObject foregroundSceneryParent;
    public GameObject actualgroundSceneryParent;

    public GameObject backgroundScenery;
    public GameObject midgroundScenery;
    public GameObject foregroundScenery;
    public GameObject actualgroundScenery;

    public GameObject startPoint;
    public GameObject endPoint;
    public GameObject spawnPoint;

    public LayerMask floorMask;
    public LayerMask playerLayer;
    public int layerOrder = 0;

    public GameObject slashMarks;
    public GameObject absoluteShield;
    public GameObject criticalHitObj;
    public GameObject heroTrailObj;
}
