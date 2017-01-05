using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaptorSwoopCollider : MonoBehaviour
{
    public RaptorScript raptorScript;
    Collider2D thisCollider;

    void Start()
    {
        thisCollider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 12)
        {
            raptorScript.SwoopAttack();
            thisCollider.enabled = false;
        }
    }
}
