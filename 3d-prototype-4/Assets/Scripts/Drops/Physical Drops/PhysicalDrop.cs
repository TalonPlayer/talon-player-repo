using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicalDrop : MonoBehaviour
{
    public UnityEvent onPickUp;
    public float value;
    public float lifeTime = 25f;
    public Rigidbody rb;
    void Start()
    {
        value = Random.Range(.75f, 1.25f);
        transform.localScale *= value;
        StartCoroutine(LifeTime());
    }

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    public void SkullPickUp()
    {
        Weapon w = PlayerManager.Instance.player.hand.hand;
        Weapon upgrade = WeaponLibrary.Instance.Upgrade(w);
        if (value >= 1.225f)
        {
            upgrade = WeaponLibrary.Instance.Upgrade(upgrade);
        }
        

        PlayerManager.Instance.player.hand.Equip(upgrade);
    }
    

    public void GemPickUp()
    {
        PlayerManager.Instance.AddMultiplier((int) (value * 150f));
    }

    public void LaunchUp()
    {
        Vector3 rand = RandExt.RandomDirection(85f, 95f);
        rand += Vector3.up;
        Vector3 rotation = RandExt.ApplyTorque(rand, Random.Range(20f, 35f));

        rb.AddForce(rand.normalized * 15f, ForceMode.Impulse);
        rb.AddTorque(rotation);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            onPickUp?.Invoke();
            Destroy(gameObject);
        }
    }
}
