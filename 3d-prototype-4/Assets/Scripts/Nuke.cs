using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nuke : MonoBehaviour
{
    public GameObject model;
    public GameObject explosion;
    public AudioSource explosionSound;
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.AddForce(Vector3.down * 20f, ForceMode.Impulse);
    }
    void OnCollisionEnter(Collision other)
    {

        foreach (Enemy e in EntityManager.Instance.enemies)
        {
            e.OnHit(9999);
        }
        PlayerManager.Instance.NukeImmunity();
        Invoke(nameof(DelayDestroy), 5f);
        model.SetActive(false);
        explosion.SetActive(true);
        explosionSound.Play();
    }

    void DelayDestroy()
    {
        Destroy(gameObject);
    }
}
