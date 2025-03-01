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

    private bool IsPlaying = false;
    private bool IsFeverTime = false;

    public float FirstDelayTime = 1f;
    public float SpawnMinTime = 5f;
    public float SpawnMaxTime = 10f;
    public bool SpawnOneByOne = true; //한번에 하나씩 소환하는 지

    public SpawnData[] SpawnDataList;

    private void Awake()
    {
        EventManager.Instance.OnPlayGameEvent.AddListener(OnPlayGame);
        EventManager.Instance.OnGameOverEvent.AddListener(OnGameOver);
        EventManager.Instance.OnFeverStateChangedEvent.AddListener(OnFeverStateChanged);
    }

    void Start()
    {
        SpawnedObjectList = new List<GameObject>();
    }

    private void OnPlayGame()
    {
        IsPlaying = true;
        StartCoroutine(StartFirstDelay());
    }

    private void OnGameOver()
    {
        IsPlaying = false;
        StopAllCoroutines();
    }

    private void OnFeverStateChanged(bool Enabled)
    {
        IsFeverTime = Enabled;
    }

    IEnumerator StartFirstDelay()
    {
        yield return new WaitForSeconds(FirstDelayTime);
        StartCoroutine(StartSpawnLoop());
    }

    IEnumerator StartSpawnLoop()
    {
        while (IsPlaying == true)
        {
            TrySpawn();
            yield return new WaitForSeconds(1);
        }
    }

    private void TrySpawn()
    {
        if(IsSpawnable() == false)
        {
            return;
        }

        // 스폰 불가능한 오브젝트 제외
        List<SpawnData> SpawnableObjectDataList = new List<SpawnData>();
        for (int i = 0; i < SpawnDataList.Length; i++)
        {
            SpawnableObject SpawnableObject = SpawnDataList[i].TargetObject.GetComponent<SpawnableObject>();
            if (SpawnableObject.IsSpawnable() == true)
            {
                SpawnableObjectDataList.Add(SpawnDataList[i]);
            }
        }

        if (SpawnableObjectDataList.Count <= 0)
        {
            Debug.Log("Spawn Failed : There's no spawnable Object");
            return;
        }

        // 총 확률 구하기
        float MaxPercentage = 0f;
        foreach (SpawnData InSpawnData in SpawnableObjectDataList)
        {
            MaxPercentage += InSpawnData.Probability;
        }

        // 랜덤 변수 뽑은 다음 비교해서 뽑기
        float RandomValue = Random.Range(0f, 1f);
        float CurrentPercentage = 0;
        GameObject SpawnedObject = null;
        foreach (SpawnData InSpawnData in SpawnableObjectDataList)
        {
            CurrentPercentage += InSpawnData.Probability;
            if (RandomValue < CurrentPercentage)
            {
                SpawnedObject = Instantiate(InSpawnData.TargetObject);
                SpawnedObject.transform.position = transform.position;

                string DebugString = "Spawn Success : " + InSpawnData.TargetObject.name + "\n";
                DebugString += "Spawn Random value : " + RandomValue + "\n";
                foreach (SpawnData DebugSpawnData in SpawnableObjectDataList)
                {
                    DebugString += DebugSpawnData.TargetObject.name + " : " + DebugSpawnData.Probability + "\n";
                }

                Debug.Log(DebugString);
                break;
            }
        }

        if(CurrentPercentage <= 0f)
        {
            Debug.Log("Spawn Failed : Spawnable objects have no chance to be spawned.");
            return;
        }

        SpawnedObjectList.Add(SpawnedObject);
    }

    private void CheckSpawnedObjectList()
    {
        for (int i = 0; i < SpawnedObjectList.Count; i++)
        {
            GameObject SpawnedObject = SpawnedObjectList[i];
            if (SpawnedObject == null)
            {
                SpawnedObjectList.RemoveSwapBack(SpawnedObject);
            }
        }
    }

    private bool IsSpawnable()
    {
        if(IsPlaying == false || IsFeverTime == true)
        {
            return false;
        }

        CheckSpawnedObjectList();

        if (SpawnOneByOne == true && SpawnedObjectList.Count > 0)
        {
            return false;
        }

        return true;
    }
}