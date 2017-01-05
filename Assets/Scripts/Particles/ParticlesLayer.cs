using UnityEngine;
using System.Collections;

public class ParticlesLayer : MonoBehaviour
{
    public SpriteRenderer setRenderer;

    void Start()
    {
        GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = setRenderer.sortingOrder;
    }
}