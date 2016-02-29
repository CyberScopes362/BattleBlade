using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class AutoLayer : MonoBehaviour
{
    List<SpriteRenderer> allSpriteRenderers = new List<SpriteRenderer>();
    int thisLayerOrder;
    ObjectFinder objectFinder;

    void Start()
    {
        allSpriteRenderers = GetComponent<DamageModifier>().allSpriteRenderers;
        objectFinder = GameObject.FindGameObjectWithTag("Initializer").GetComponent<ObjectFinder>();

        if (objectFinder.layerOrder >= 100)
            objectFinder.layerOrder = 0;

        thisLayerOrder = objectFinder.layerOrder;

        foreach (SpriteRenderer eachRenderer in allSpriteRenderers)
            eachRenderer.sortingOrder = thisLayerOrder;

        thisLayerOrder += 1;
        objectFinder.layerOrder = thisLayerOrder;
    }
}