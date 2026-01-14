using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Class should be used for gathering information rather than choosing targets
/// </summary>
public class UnitVision : MonoBehaviour
{
    private Unit main;
    [Header("Line of Sight")]
    public LayerMask lineOfSightMask = ~0;
    public SphereCollider visionCollider;
    public float validCoverRadius = 10f;
    public float validCoverDist = 15f;
    [Header("World States")]
    public bool targetsCanBeSeen;
    public bool isLosClear;
    public bool HasCoverNearby;
    public bool isExposed;
    public Vector3 targetLastKnownPos;
    public List<Entity> possibleTargets = new List<Entity>();
    public List<Entity> targetsInView = new List<Entity>();
    public List<Entity> allies = new List<Entity>();
    public List<CoverPoint> possibleCover = new List<CoverPoint>();
    public CoverPoint[] validCovers = new CoverPoint[3];
    void Awake()
    {
        main = GetComponent<Unit>();
    }
    public UnitWorldState Tick(UnitWorldState s)
    {

        Vector3 viewPoint = (s.InCover && main.movement.coverPoint != null)
        ? main.movement.coverPoint.peekPoint.position : main.body.eyes.position;
        for (int i = possibleTargets.Count - 1; i >= 0; i--)
        {
            if (!possibleTargets[i].isAlive) possibleTargets.RemoveAt(i);
        }
        s.Targets = possibleTargets;
        s.ViewableTargets = targetsInView;

        s.TargetsCanBeSeen = ValidateTargets(viewPoint);

        isLosClear = ComputeLineOfSight(main.body.eyes.position);
        s.HasClearShot = isLosClear;

        s.TargetAcquired = main.target != null;
        if (s.TargetAcquired)
            s.TargetAlive = main.target.isAlive;
        else s.TargetAlive = false;

        bool hasCoverNearby;

        if (!s.TargetAcquired) hasCoverNearby = ValidateCover(7f * transform.forward + transform.position);
        else
        {
            if (isLosClear)
                hasCoverNearby = ValidateCover(main.TargetPos);
            else
                hasCoverNearby = ValidateCover(targetLastKnownPos);
        }

        s.HasCoverNearby = hasCoverNearby;


        s.CoverBlown = IsCoverBlown();
        return s;
    }


    /// <summary>
    /// Get TRUE line of sight
    /// </summary>
    /// <param name="viewPoint"></param>
    /// <returns></returns>
    private bool ComputeLineOfSight(Vector3 viewPoint)
    {
        if (main.target == null) return false;

        bool hasLineOfSight = IsSightClear(viewPoint, main.TargetPos);

        if (main.brain.logPlans)
        {
            if (hasLineOfSight)
                Debug.DrawLine(viewPoint, main.TargetPos, Color.green, .5f);
            else
                Debug.DrawLine(viewPoint, main.TargetPos, Color.red, .5f);
        }


        if (hasLineOfSight) targetLastKnownPos = main.TargetPos;
        return hasLineOfSight;
    }

    /// <summary>
    /// Return true if nothing is blocking Line Cast. Return false if its blocked.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private bool IsSightClear(Vector3 from, Vector3 to)
    {
        if (Physics.Linecast(from, to, out RaycastHit hit, lineOfSightMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag(main.ai.targetTag))
                return true;

            return false;
        }

        return false;
    }

    private bool ValidateTargets(Vector3 viewPoint)
    {
        if (possibleTargets.Count == 0) return false;

        bool validTargets = false;
        targetsInView = new List<Entity>();

        foreach (Entity t in possibleTargets)
        {
            if (t == null) continue;
            if (!t.isAlive) continue;

            if (IsSightClear(viewPoint, t.transform.position))
            {
                validTargets = true;
                targetsInView.Add(t);
            }
        }

        return validTargets;
    }

    private bool ValidateCover(Vector3 threatPos)
    {
        if (possibleCover.Count == 0) return false;

        bool valid = false;
        validCovers = new CoverPoint[3];
        float[] bestScore = new float[3];

        foreach (CoverPoint c in possibleCover)
        {
            float distToCover = (transform.position - c.transform.position).magnitude;
            float distFromThreat = (threatPos - c.transform.position).magnitude;
            float angle = Vector3.Angle(c.transform.forward, (threatPos - c.peekLocation.transform.position).normalized);
            if (distToCover >= validCoverRadius) continue;
            if (distFromThreat >= validCoverDist) continue;
            if (c.isTargeted || c.isOccupied) continue;
            if (angle > 65f && angle < 295f) continue;

            if (!IsSightClear(threatPos, c.transform.position))
            {
                valid = true;

                int blocked = 0;
                int advantage = 0;
                foreach (Entity e in possibleTargets)
                {
                    bool enemyBlocked = !IsSightClear(e.transform.position, c.transform.position);

                    bool advantageous = IsSightClear(c.peekPoint.position, e.transform.position);


                    if (enemyBlocked)
                    {
                        blocked++;
                        //Debug.DrawLine(e.transform.position, c.transform.position, Color.green, 1f);
                    }
                    //else
                    //Debug.DrawLine(e.transform.position, c.transform.position, Color.red, 1f);

                    if (advantageous)
                    {
                        advantage++;
                        //Debug.DrawLine(e.transform.position, c.peekPoint.position, Color.yellow, 1f);
                    }
                    //else
                    // Debug.DrawLine(e.transform.position, c.peekPoint.position, Color.magenta, 1f);

                    //if (enemyBlocked && advantageous)
                    //Debug.DrawRay(c.transform.position, Vector3.up * 3f, Color.blue, 4f);

                }
                float score = (advantage * 20) + (blocked * 25) - (distToCover + distFromThreat);
                InsertCover(c, score, bestScore);
            }
        }

        return valid;
    }

    private void InsertCover(CoverPoint coverPoint, float score, float[] bestScore)
    {
        if (score > bestScore[0])
        {
            bestScore[2] = bestScore[1];
            validCovers[2] = validCovers[1];

            bestScore[1] = bestScore[0];
            validCovers[1] = validCovers[0];

            bestScore[0] = score;
            validCovers[0] = coverPoint;
        }
        else if (score > bestScore[1])
        {
            bestScore[2] = bestScore[1];
            validCovers[2] = validCovers[1];

            bestScore[1] = score;
            validCovers[1] = coverPoint;
        }
        else if (score > bestScore[2])
        {
            bestScore[2] = score;
            validCovers[2] = coverPoint;
        }
    }

    /// <summary>
    /// Return true if the target is not in front
    /// </summary>
    /// <returns></returns>
    public bool IsCoverBlown()
    {
        if (!main.movement.coverPoint) return false;
        if (main.target == null) return false;
        

        bool coverIsBlown =  IsSightClear(main.TargetPos, main.movement.coverPoint.transform.position);
        if (main.brain.logPlans)
        {
            if (coverIsBlown)
                Debug.DrawLine(main.TargetPos, main.movement.coverPoint.transform.position, Color.green, .5f);
            else
                Debug.DrawLine(main.TargetPos, main.movement.coverPoint.transform.position, Color.red, .5f);
        }
        return coverIsBlown;
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cover"))
        {
            CoverPoint c = other.GetComponent<CoverPoint>();
            if (!c) return;
            if (!possibleCover.Contains(c)) possibleCover.Add(c);
        }
        else if (other.CompareTag(main.ai.targetTag))
        {
            Entity e = other.GetComponent<Entity>();
            if (!e) return;
            if (!possibleTargets.Contains(e)) possibleTargets.Add(e);
        }
        else if (other.CompareTag(gameObject.tag))
        {
            Entity e = other.GetComponent<Entity>();
            if (!e) return;
            if (!allies.Contains(e)) allies.Add(e);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cover"))
        {
            CoverPoint c = other.GetComponent<CoverPoint>();
            if (!c) return;
            if (possibleCover.Contains(c)) possibleCover.Remove(c);
        }
        else if (other.CompareTag(main.ai.targetTag))
        {
            Entity e = other.GetComponent<Entity>();
            if (!e) return;
            if (possibleTargets.Contains(e)) possibleTargets.Remove(e);
        }
        else if (other.CompareTag(gameObject.tag))
        {
            Entity e = other.GetComponent<Entity>();
            if (!e) return;
            if (allies.Contains(e)) allies.Remove(e);
        }
    }
}