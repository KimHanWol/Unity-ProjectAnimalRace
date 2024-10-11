using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnData
{
    public GameObject TargetObject;
    public float Probability;
}

public class ObjectSpawner : MonoBehaviour
{
    public float FirstDelayTime;
    public float SpawnMinTime;
    public float SpawnMaxTime;

    public SpawnData[] SpawnDataList;

    private List<GameObject> SpawnedObjectList;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartFirstDelay());
        SpawnedObjectList = new List<GameObject>();
    }

    IEnumerator StartFirstDelay() 
    {
        yield return new WaitForSeconds(FirstDelayTime);
        StartCoroutine(StartSpawnLoop());
    }

    IEnumerator StartSpawnLoop()
    {
        while(true)
        {
            TrySpawn();
            yield return new WaitForSeconds(1);
        }
    }

    private void TrySpawn()
    {
        GameObject SpawnObject = null;

        float TotalPercentage = SpawnDataList.Length;
        float RandomValue = Random.Range(0, TotalPercentage);

        float CurrentPercentage = 0;
        foreach(SpawnData InSpawnData in SpawnDataList)
        {
            CurrentPercentage += InSpawnData.Probability;
            if(RandomValue < CurrentPercentage)
            {
                SpawnObject = InSpawnData.TargetObject;
                break;
            }
        }

        if(SpawnObject != null)
        {
            Instantiate(SpawnObject);
            SpawnedObjectList.Add(SpawnObject);
        }
    }
}