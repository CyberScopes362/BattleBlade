using UnityEngine;
using System.Collections;

public class AutoKillParticles : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().main.startLifetime.constant + GetComponent<ParticleSystem>().main.duration);
    }
}