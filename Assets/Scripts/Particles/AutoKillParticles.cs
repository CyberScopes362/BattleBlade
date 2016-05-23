using UnityEngine;
using System.Collections;

public class AutoKillParticles : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().startLifetime + GetComponent<ParticleSystem>().duration);
    }
}