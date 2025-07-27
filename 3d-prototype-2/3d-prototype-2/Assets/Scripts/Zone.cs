using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Zone : MonoBehaviour
{
    public UnityEvent onEnterZone;
    public UnityEvent onEnterPit;
    public bool isAPit = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player"){
            onEnterZone.Invoke();
            Player p = other.GetComponent<Player>();
            if (isAPit)
            {
                if (p.isAlive)
                    HUDController.Instance.PlayDeathAnim(true);
                WorldController.Instance.Reset();

            }
        }
        else if (other.gameObject.tag == "Enemy"){
            Enemy e = other.GetComponent<Enemy>();
            if (isAPit && e.isAlive)
            {
                e.onDeath?.Invoke();
            }
        }
    }
}
