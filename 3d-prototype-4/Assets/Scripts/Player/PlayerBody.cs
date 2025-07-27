using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    public Animator animator;
    public Transform body;
    private Player player;
    void Awake()
    {
        player = GetComponent<Player>();
    }
    public void Play(string para, bool val)
    {
        animator.SetBool(para, val);
    }

    public void Play(string para, float val)
    {
        animator.SetFloat(para, val);
    }

    public void Play(string para)
    {
        animator.SetTrigger(para);
    }

    public void RagDoll(bool ragdoll)
    {
        animator.enabled = !ragdoll;
        Rigidbody[] rigidbodies = body.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody r in rigidbodies)
        {
            r.isKinematic = !ragdoll;
        }
    }
}
