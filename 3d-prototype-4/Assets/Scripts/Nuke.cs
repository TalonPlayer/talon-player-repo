using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nuke : MonoBehaviour
{
    // Kill all enemies when activated
    public GameObject model;
    public GameObject explosion;
    public AudioSource explosionSound;
    public bool active = true;
    public string owner;
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.down * 20f, ForceMode.Impulse);
    }

    public void Init(Player player)
    {
        player.NukeImmunity();
        owner = player._name;
    }
    void OnCollisionEnter(Collision other)
    {
        if (active)
        {
            EntityManager.Instance.KillAllEnemies();
            Invoke(nameof(DelayDestroy), 5f);
            model.SetActive(false);
            explosion.SetActive(true);
            explosionSound.Play();
            active = false;
        }

    }

    void DelayDestroy()
    {
        Destroy(gameObject);
    }
}
