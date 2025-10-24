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
    public MeshRenderer mr;
    private Player player;
    void Start()
    {
        value = Random.Range(.75f, 1.25f);
        transform.localScale *= value; // Value of drop changes the size
        StartCoroutine(LifeTime());

        // If the drop has a mesh renderer attached, randomly change the color.
        // This is for gems
        if (mr)
        {
            int rand = Random.Range(0, 5);
            switch (rand)
            {
                case 0:
                    mr.material.color = Color.red;
                    break;
                case 1:
                    mr.material.color = Color.blue;
                    break;
                case 2:
                    mr.material.color = Color.yellow;
                    break;
                case 3:
                    mr.material.color = Color.white;
                    break;
                case 4:
                    mr.material.color = Color.green;
                    break;

            }

        }
    }

    /// <summary>
    /// Destroy after a certain time, usually 25 seconds
    /// </summary>
    /// <returns></returns>
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
        Weapon w = player.hand.hand;
        Weapon upgrade = WeaponLibrary.Instance.Upgrade(player, w);
        if (value >= 1.225f) // if value is above 1.225f, double upgrade
            upgrade = WeaponLibrary.Instance.Upgrade(player, upgrade);

        player.hand.Equip(upgrade);
        player.info.skulls++;
    }

    /// <summary>
    /// Gems increase player's multiplier by a large amount
    /// </summary>
    public void GemPickUp()
    {
        player.stats.AddMultiplier((int)(value * 150f));
        player.info.gems++;
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
            player = other.GetComponent<Player>();
            onPickUp?.Invoke();
            Destroy(gameObject);
        }
    }
}
