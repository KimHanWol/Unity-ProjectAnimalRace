using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[System.Serializable]
public class SpawnData
{
    public GameObject TargetObject;
    public float Probability;
}

public class ObjectSpawner : MonoBehaviour
{
    private List<GameObject> SpawnedObjectList;

    private bool IsStarted = false;

    public float FirstDelayTime = 1f;
    public float SpawnMinTime = 5f;
    public float SpawnMaxTime = 10f;
    public bool SpawnOneByOne = true; //한번에 하나씩 소환하는 지

    public SpawnData[] SpawnDataList;

    // Start is called before the first frame update
    void Start()
    {
        SpawnedObjectList = new List<GameObject>();
    }

    public void EnableSpawn(bool Enabled)
    {
        IsStarted = Enabled;

        if (IsStarted == true)
        {
            StartCoroutine(StartFirstDelay());
        }
        else
        {
            StopAllCoroutines();
            for (int i = 0; i < SpawnedObjectList.Count; i++)
            {
                GameObject SpawnedObject = SpawnedObjectList[i];
                SpawnedObjectList.RemoveSwapBack(SpawnedObject);
                Destroy(SpawnedObject);
            }
        }
    }

    IEnumerator StartFirstDelay()
    {
        yield return new WaitForSeconds(FirstDelayTime);
        StartCoroutine(StartSpawnLoop());
    }

    IEnumerator StartSpawnLoop()
    {
        while (true)
        {
            TrySpawn();
            yield return new WaitForSeconds(1);
        }
    }

    private void TrySpawn()
    {
        CheckSpawnedObjectList();

        if (SpawnOneByOne == true && SpawnedObjectList.Count > 0)
        {
            return;
        }

        GameObject SpawnedObject = null;

        float TotalPercentage = SpawnDataList.Length;
        float RandomValue = Random.Range(0, TotalPercentage);

        float CurrentPercentage = 0;
        foreach (SpawnData InSpawnData in SpawnDataList)
        {
            CurrentPercentage += InSpawnData.Probability;
            if (RandomValue < CurrentPercentage)
            {
                SpawnedObject = Instantiate(InSpawnData.TargetObject);
                SpawnedObject.transform.position = transform.position;
                break;
            }
        }

        if (SpawnedObject != null)
        {
            SpawnedObjectList.Add(SpawnedObject);
        }
    }

    private void CheckSpawnedObjectList()
    {
        for (int i = 0; i < SpawnedObjectList.Count; i++)
        {
            GameObject SpawnedObject = SpawnedObjectList[i];

            bool IsNeedToRemove = false;
            if (SpawnedObject == null)
            {
                IsNeedToRemove = true;
            }

            SpawnableObject SpawnableObject = SpawnedObject.GetComponent<SpawnableObject>();
            if (SpawnableObject != null && SpawnableObject.IsDestroying == true)
            {
                IsNeedToRemove = true;
            }

            if (IsNeedToRemove == true)
            {
                SpawnedObjectList.RemoveSwapBack(SpawnedObject);
                Destroy(SpawnedObject);
            }
        }
    }
}