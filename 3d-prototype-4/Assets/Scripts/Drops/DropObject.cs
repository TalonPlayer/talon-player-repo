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
    public bool isActive = true;
    public float moveSpeed = .5f;
    public Transform targetLocation;
    Coroutine timerRoutine;
    void Start()
    {
        // Sound played when spawned
        audioSource.PlayOneShot(drop.spawnSound, Random.Range(.6f, 1.8f));

        // If the drop is a powerup, create a looping audiosource
        if (drop is Powerup powerup)
            powerup.LoopSound(gameObject);

        // Randomly rotate the drop
        rotateSpeed = Random.Range(25f, 90f);
        rotationAxis = Random.onUnitSphere;

        // The drop has a life span
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
        if (other.tag == "Player" && isActive)
        {
            isActive = false;
            Player player = other.GetComponent<Player>();
            drop.OnPickUp(player);

            // Move the drop to the top left of the screen
            MoveTo(DropManager.Instance.collectLocation);

            // Delay the destroy so that way the item can move there
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

    /// <summary>
    /// Move to a targeted location
    /// </summary>
    /// <param name="target"></param>
    public void MoveTo(Transform target)
    {
        targetLocation = target;
        isMoving = true;
    }
}
