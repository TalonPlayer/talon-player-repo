using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.UI;
public class EnemyBody : MonoBehaviour
{

    [Header("Body")]
    public Animator animator;
    public int idleAnims = 1;
    public int atkAnims = 1;
    [Header("Items")]
    public List<GameObject> heldItems = new List<GameObject>();
    private Enemy enemy;
    private Vector3 direction;
    private Vector3 torque;
    public void SetEnemy(Enemy e) => enemy = e;
    void Start()
    {
        animator.applyRootMotion = true;
    }
    
    public void Dropitems()
    {
        foreach (GameObject item in heldItems)
        {
            GameObject newItem = Instantiate(item, item.transform.position, Quaternion.identity);
            newItem.transform.localScale = item.transform.lossyScale;
            Rigidbody rb = newItem.GetComponent<Rigidbody>();
            BoxCollider bc = newItem.GetComponent<BoxCollider>();
            SphereCollider sc = newItem.GetComponent<SphereCollider>();

            item.SetActive(false);

            EnemyManager.Instance.droppedItems.Add(newItem);

            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;

            if (bc) bc.enabled = true;
            if (sc) sc.enabled = true;
            direction = GameManager.Instance.RandomDirection(30f, 80f); // Upward arc between 30° and 80°
            torque = GameManager.Instance.ApplyTorque(direction, 15f);
            rb.AddForce(direction * 10f, ForceMode.Impulse);
            rb.AddTorque(torque, ForceMode.Impulse);

            rb.AddForce(enemy.kbDirection * 10f, ForceMode.Impulse);
            rb.AddForce(Vector3.up * 5f);
        }
    }

    public void RagDoll(bool activate)
    {
        
        //hip.enabled = activate;
        enemy.pause = activate;
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        enemy.head.transform.parent = EnemyManager.Instance.ragdollFolder;
        enemy.rb.isKinematic = false;
        foreach (Rigidbody r in rigidbodies)
        {
            r.isKinematic = !activate;
            r.AddForce(enemy.kbDirection * (enemy.kbForce * .05f), ForceMode.Impulse);
            r.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            r.gameObject.layer = LayerMask.NameToLayer("Ragdoll");
        }

    }

    public void PlayAnim(string anim)
    {
        animator.SetTrigger(anim);
    }

    public void IntAnim(string anim, int num)
    {
        animator.SetInteger(anim, num);
    }


    public void BoolAnim(string anim, bool active)
    {
        animator.SetBool(anim, active);
    }
}
