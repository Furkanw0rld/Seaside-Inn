using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldWindController : MonoBehaviour
{
#pragma warning disable 0649
    [Header("Time")]
    [SerializeField] [Min(0f)] private float minimumTimeBetweenEffects = 25f;
    [SerializeField] [Min(0f)] private float maximumTimeBetweenEffects = 100f;
    private readonly float minimumYPosition = 12f;
    private readonly float maximumYPosition = 18f;

    private readonly float minimumXPosition = -30f;
    private readonly float maximumXPosition = 90f;

    private readonly float minimumZPosition = -100f;
    private readonly float maximumZPosition = 100f;
    [Space]
    [Header("Single Wind Objects")]
    public GameObject[] windEffects = new GameObject[] { };
#pragma warning restore 0649

    private Vector3 windDirection;
    private GameTimeManager gameTime;
    private readonly int animationStates = 6; // Must match the amount of animations in animator controller. (Inclusive)

    void Start()
    {
        gameTime = GameTimeManager.Instance;
        GenerateNextWind();
    }

    private Vector3 GetRandomDirection()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        return new Vector3(randomDir.x, 0, randomDir.y).normalized;
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 position = Vector3.zero;
        position.x = Random.Range(minimumXPosition, maximumXPosition);
        position.y = Random.Range(minimumYPosition, maximumYPosition);
        position.z = Random.Range(minimumZPosition, maximumZPosition);

        return position;
    }

    private int RandomState()
    {
        return Random.Range(0, animationStates + 1);
    }

    private void GenerateNextWind()
    {
        windDirection = GetRandomDirection();
        int amount = Random.Range(0, 10);

        for(int i = 0; i <= amount; i++)
        {
            GameObject windEffect = Instantiate(windEffects[0], this.transform);
            windEffect.transform.position = GetRandomPosition();
            windEffect.transform.rotation = Quaternion.LookRotation(windDirection, Vector3.up);
            windEffect.GetComponent<Animator>().SetInteger("state", RandomState());
        }

        ulong nextTriggerTime = gameTime.GetNextWorldTime(gameTime.GetWorldTime(), (ushort)Random.Range(minimumTimeBetweenEffects, maximumTimeBetweenEffects));
        gameTime.WaitUntilWorldTime(nextTriggerTime, GenerateNextWind);
    }
}
