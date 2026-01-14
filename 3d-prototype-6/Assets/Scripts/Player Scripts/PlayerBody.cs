using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    private Player main;
    [Header("Body Parts")]
    public Transform head;
    public Transform rightHand, leftHand;
    public Animator headAnimator;
    public RuntimeAnimatorController weaponController;
    public Animator weaponAnimator;
    public
    void Awake()
    {
        main = GetComponent<Player>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Play(string stringParam)
    {
        headAnimator.SetTrigger(stringParam);
    }
    public void Play(string stringParam, float floatParam)
    {
        headAnimator.SetFloat(stringParam, floatParam);
    }
    public void Play(string stringParam, int intParam)
    {
        headAnimator.SetInteger(stringParam, intParam);
    }
    public void Play(string stringParam, bool boolParam)
    {
        headAnimator.SetBool(stringParam, boolParam);
    }
    public void SetWeapon(RuntimeAnimatorController animator)
    {
        weaponController = animator;
    }

    public void PlayWeapon(string stringParam)
    {
        if (!weaponController) return;

        weaponAnimator.SetTrigger(stringParam);
    }
    public void PlayWeapon(string stringParam, float floatParam)
    {
        if (!weaponController) return;

        weaponAnimator.SetFloat(stringParam, floatParam);
    }
    public void PlayWeapon(string stringParam, int intParam)
    {
        if (!weaponController) return;

        weaponAnimator.SetInteger(stringParam, intParam);
    }
    public void PlayWeapon(string stringParam, bool boolParam)
    {
        if (!weaponController) return;

        weaponAnimator.SetBool(stringParam, boolParam);
    }
}
