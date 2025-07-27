using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MonoBehaviour
{
    public Drop drop;
    public GameObject modelObj;
    public float rotateSpeed;
    private Vector3 rotationAxis;
    public AudioSource audioSource;
    public bool isMoving = false;
    public float moveSpeed = .5f;
    public Transform targetLocation;
    Coroutine timerRoutine;
    void Start()
    {
        audioSource.PlayOneShot(drop.spawnSound, Random.Range(.6f, 1.8f));
        if (drop is Powerup powerup)
            powerup.LoopSound(gameObject);
        rotateSpeed = Random.Range(25f, 90f);
        rotationAxis = Random.onUnitSphere;
        timerRoutine = StartCoroutine(Timer());
    }
    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, targetLocation.position, Time.deltaTime * moveSpeed);
        }
        transform.Rotate(rotationAxis * rotateSpeed * Time.deltaTime);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            drop.OnPickUp();
            MoveTo(DropManager.Instance.collectLocation);
            Invoke(nameof(DelayDestroy), 1f);
        }
    }

    void DelayDestroy()
    {
        Destroy(gameObject);        
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(25f);
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        DropManager.Instance.CheckDrop(drop);
    }

    public void MoveTo(Transform target)
    {
        targetLocation = target;
        isMoving = true;
    }
}
