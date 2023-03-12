using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneryController : MonoBehaviour
{
    public GameObject wavePrefab;
    private List<Transform> waveTransforms = new List<Transform>();
    public float waveSpeed = -2f;
    public float maxX = 8f;
    public int waveAmount = 16;
    Vector3 movementVector;
    Vector3 movementVectorClouds;
    public int cloudAmount = 7;
    public float cloudSpeed = -0.5f;
    public GameObject cloudPrefab;
    private List<Transform> cloudTransforms = new List<Transform>();

    void Start()
    {
        movementVector.x = waveSpeed;
        movementVectorClouds.x = cloudSpeed;
        CreateWave();
        CreateClouds();
    }
    void CreateWave()
    {
        Vector3 spawnPos = new Vector3(-maxX, -2.75f, 0);

        for (int i = 0; i < waveAmount; i++)
        {
            GameObject newGrass = Instantiate(wavePrefab, spawnPos, Quaternion.identity);
            waveTransforms.Add(newGrass.transform);
            spawnPos.x += 1f;
        }
    }

    void Update()
    {
        if (GameController.instance.curState != GameController.State.Dead && GameController.instance.curState != GameController.State.Dying)
        {
            MoveWave();
            MoveClouds();
        }
    }

    void MoveWave()
    {
        for (int i = 0; i < waveTransforms.Count; i++)
        {
            if (waveTransforms[i].localPosition.x <= -maxX)
            {
                waveTransforms[i].localPosition += new Vector3(maxX * 2, 0, 0);
            }

            waveTransforms[i].localPosition += movementVector * Time.deltaTime;
        }
    }

    void CreateClouds()
    {
        Vector3 spawnPos = new Vector3(-maxX, Random.Range(1.5f, 3f), 0);
        Vector3 randomScale = Vector3.one * Random.Range(1f, 2f);
        for (int i = 0; i < cloudAmount; i++)
        {
            GameObject newGrass = Instantiate(cloudPrefab, spawnPos, Quaternion.identity);
            newGrass.transform.localScale = randomScale;
            cloudTransforms.Add(newGrass.transform);
            spawnPos.x += Random.Range(1f, 3f);
            spawnPos.y = Random.Range(1.5f, 3f);
            randomScale = Vector3.one * Random.Range(1f, 2f);
        }
    }

    void MoveClouds()
    {
        for (int i = 0; i < cloudTransforms.Count; i++)
        {
            if (cloudTransforms[i].localPosition.x <= -maxX)
            {
                cloudTransforms[i].localPosition += new Vector3(maxX * 2 + 1.5f + Random.Range(-1.5f, 1.5f), 0, 0);
                cloudTransforms[i].localPosition = new Vector3(cloudTransforms[i].localPosition.x, Random.Range(1.5f, 3f), cloudTransforms[i].localPosition.z);
                cloudTransforms[i].localScale = Vector3.one * Random.Range(1f, 2f);
            }
            cloudTransforms[i].localPosition += movementVectorClouds * Time.deltaTime * cloudTransforms[i].localScale.x;
        }
    }
}