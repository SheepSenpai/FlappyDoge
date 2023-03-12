
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle
{
    public Transform upperObs;
    public Transform lowerObs;
    public bool canGiveScore = true;

    public Obstacle(Transform upper, Transform lower)
    {
        upperObs = upper;
        upperObs.localEulerAngles = new Vector3(0, 0, 180f);
        lowerObs = lower;
    }

    public void Move(Vector3 movement)
    {
        upperObs.position += movement;
        lowerObs.position += movement;
    }

    public void Reset(Vector3 resetPosition)
    {
        canGiveScore = true;
        float randomY = Random.Range(-ObstacleController.randomYMax, ObstacleController.randomYMax);
        resetPosition.y = ObstacleController.vertDisBetObs + randomY;
        upperObs.position = resetPosition;
        resetPosition.y = -ObstacleController.vertDisBetObs + randomY;
        lowerObs.position = resetPosition;
    }

    public float X
    {
        get { return upperObs.position.x; }
    }
}

[RequireComponent(typeof(AudioSource))]
public class ObstacleController : MonoBehaviour
{
    public GameObject pipePrefab;
    private readonly List<Obstacle> obstacles = new();
    public float speed = -4f;
    public float maxX = 8f;
    public float horizDisBetObs = 4f;
    public static float vertDisBetObs = 3.8f;
    public int amount;
    public static float randomYMax = 2f;
    Vector3 movementVector;
    private AudioSource audioSource;

    void Start()
    {
        movementVector.x = speed;
        CreateObstacles();
        audioSource = GetComponent<AudioSource>();
    }
    void CreateObstacles()
    {
        Vector3 pipeSpawnPos = new(maxX, 0, 0);
        float randomYPosition = Random.Range(-1.5f, 1.5f);

        for (int i = 0; i < amount; i++)
        {
            pipeSpawnPos.y = ObstacleController.vertDisBetObs + randomYPosition;
            GameObject upperPipe = Instantiate(pipePrefab, pipeSpawnPos, Quaternion.identity);

            pipeSpawnPos.y = -ObstacleController.vertDisBetObs + randomYPosition;
            GameObject lowerPipe = Instantiate(pipePrefab, pipeSpawnPos, Quaternion.identity);

            obstacles.Add(new Obstacle(upperPipe.transform, lowerPipe.transform));

            pipeSpawnPos.x += horizDisBetObs;
            randomYPosition = Random.Range(-1f, 2f);
        }
    }

    void Update()
    {
        if (GameController.instance.curState == GameController.State.Playing)
        {
            MoveObstacles();
        }
    }

    public void ResetObstacles()
    {
        for (int i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].Reset(new Vector3(maxX + (i * horizDisBetObs), 0, 0));
        }
    }

    void MoveObstacles()
    {
        for (int i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].Move(movementVector * Time.deltaTime);

            if (obstacles[i].X <= -maxX)
            {
                obstacles[i].Reset(new Vector3(maxX, 0, 0));
            }
            if (obstacles[i].canGiveScore == true && obstacles[i].X <= 0)
            {
                obstacles[i].canGiveScore = false;
                GameController.instance.Score++;
                audioSource.Play();
            }
        }
    }
}