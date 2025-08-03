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
        transform.localScale *= value; // Value of drop changes the size
        StartCoroutine(LifeTime());
    }

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    /// <summary>
    /// Skulls upgrade current weapon
    /// </summary>
    public void SkullPickUp()
    {
        Weapon w = PlayerManager.Instance.player.hand.hand;
        Weapon upgrade = WeaponLibrary.Instance.Upgrade(w);
        if (value >= 1.225f) // if value is above 1.225f, double upgrade
            upgrade = WeaponLibrary.Instance.Upgrade(upgrade);

        PlayerManager.Instance.player.hand.Equip(upgrade);
    }
    
    /// <summary>
    /// Gems increase player's multiplier by a large amount
    /// </summary>
    public void GemPickUp()
    {
        PlayerManager.Instance.AddMultiplier((int)(value * 150f));
    }

    /// <summary>
    /// Launches drop upwards
    /// </summary>
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
        if (other.tag == "Player") // Player picked up drop
        {
            onPickUp?.Invoke();
            Destroy(gameObject);
        }
    }
}
