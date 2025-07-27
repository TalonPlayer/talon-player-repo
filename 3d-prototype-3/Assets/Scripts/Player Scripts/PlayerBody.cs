using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    public Animator animator;
    private Player player;
    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {

    }

    public void Play(string name)
    {
        animator.Play(name);
    }

    public void Play(string name, bool active)
    {
        animator.SetBool(name, active);
    }
}
