using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Perform Actions here
/// </summary>
public class UnitBehaviour : MonoBehaviour
{
    private Unit main;
    public int AILevel;
    public UnitType unitType;
    public IntentType intent;
    public MoveUrgency movementType;
    public FacingMode orientation;
    [HideInInspector] public string targetTag;
    [HideInInspector] public LayerMask layers;
    void Awake()
    {
        main = GetComponent<Unit>();

        AILevel = Mathf.Clamp(AILevel, 1, 10);

        switch ((int)unitType)
        {
            case 0:
                targetTag = "Player";
                layers = ~(Layer.Enemy | 1 << 2);
                break;
            case 1:
                targetTag = "Enemy";
                layers = ~(Layer.Ally | Layer.Player | 1 << 2);
                break;
        }
    }

    void Start()
    {

    }

    /// <summary>
    /// Returns the index of element number decided on the AI based on level
    /// </summary>
    /// <returns></returns>
    public int AILevelChoice(int choiceAmount)
    {
        // 0 is best -> # = to count is worst
        // 1 -> 10% will choose best
        // 2 -> 20% 3 -> 30% 4 -> 40% 5 -> 50%

        if (AILevel == 10) return 0;

        float[] chances = new float[choiceAmount];
        // Calculate Best Chances first
        chances[0] = Mathf.Pow((float)AILevel / choiceAmount, 2);
        float remaining = 1f - chances[0];
        float prev = 0;
        for (int i = 1; i < choiceAmount; i++)
        {
            float perc = Mathf.Pow((float)i / choiceAmount, 2);
            chances[i] = (perc - prev) * remaining;
            prev = perc;
        }

        float rand = Random.value;
        float total = 0f;
        int chosenIndex = 0;

        for (int i = 0; i < choiceAmount; i++)
        {
            total += chances[i];

            if (rand <= total)
            {
                chosenIndex = i;
                break;
            }
        }
        //Debug.Log($"I chose index {chosenIndex}!");
        return chosenIndex;
    }

    public void Tick()
    {
        if (intent == IntentType.Seek)
        {
            orientation = FacingMode.FaceMoveDirection;
        }
        else if (intent == IntentType.Engage)
        {
            orientation = FacingMode.FaceTarget;
        }

        orientation = FacingMode.FaceTarget;

        if (movementType == MoveUrgency.None)
        {
            //main.movement.ToggleMovement(false);
        }
        else
        {
            //main.movement.ToggleMovement(true);
        }
    }
}

public enum UnitType
{
    Enemy = 0,
    Ally = 1,
    Neither = 2,
}

public enum FacingMode
{
    FaceMoveDirection,
    FaceDirection,
    FaceTarget,
}

