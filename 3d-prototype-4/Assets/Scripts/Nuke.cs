using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nuke : MonoBehaviour
{
    // Kill all enemies when activated
    public GameObject model;
    public GameObject explosion;
    public AudioSource explosionSound;
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        PlayerManager.Instance.NukeImmunity();
        rb.AddForce(Vector3.down * 20f, ForceMode.Impulse);
    }
    void OnCollisionEnter(Collision other)
    {
        EntityManager.Instance.KillAllEnemies();
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
