using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class ShakeComponent : MonoBehaviour
{  
    public float ShakeAmount = 0.05f;

    private bool IsShakeEnabled = false;
    private Dictionary<GameObject, Vector3> ObjectDefaultPositionDic;

    private void Start()
    {
        ObjectDefaultPositionDic = new Dictionary<GameObject, Vector3>();
    }

    public void EnableShake(GameObject TargetObject, bool Enabled)
    {
        if(IsShakeEnabled == Enabled)
        {
            return;
        }

        IsShakeEnabled = Enabled;

        StopAllCoroutines();

        if (IsShakeEnabled == true)
        {
            ObjectDefaultPositionDic.Add(TargetObject, TargetObject.transform.position);
            StartCoroutine(ShakeObject(TargetObject));
        }
        else
        {
            TargetObject.transform.position = ObjectDefaultPositionDic.GetValueOrDefault(TargetObject, TargetObject.transform.position);
            ObjectDefaultPositionDic.Remove(TargetObject);
        }
    }

    public void EnableShake(GameObject TargetObject, bool Enabled, float ShakeDuration)
    {
        IsShakeEnabled = Enabled;

        StopAllCoroutines();

        if (IsShakeEnabled == true)
        {
            ObjectDefaultPositionDic.Add(TargetObject, TargetObject.transform.position);
            StartCoroutine(ShakeObject(TargetObject, ShakeDuration));
        }
        else
        {
            TargetObject.transform.position = ObjectDefaultPositionDic.GetValueOrDefault(TargetObject, TargetObject.transform.position);
            ObjectDefaultPositionDic.Remove(TargetObject);
        }
    }

    private IEnumerator ShakeObject(GameObject TargetObject)
    {
        Vector3 ObjectDefaultPosition = ObjectDefaultPositionDic.GetValueOrDefault(TargetObject, TargetObject.transform.position);
        while (IsShakeEnabled == true)
        {
            float offsetX = Random.Range(-ShakeAmount, ShakeAmount);
            float offsetY = Random.Range(-ShakeAmount, ShakeAmount);

            TargetObject.transform.position = ObjectDefaultPosition + new Vector3(offsetX, offsetY, 0);
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator ShakeObject(GameObject TargetObject, float ShakeDuration)
    {
        Vector3 ObjectDefaultPosition = ObjectDefaultPositionDic.GetValueOrDefault(TargetObject, TargetObject.transform.position);

        float elapsedTime = 0f;
        while (elapsedTime < ShakeDuration)
        {
            float offsetX = Random.Range(-ShakeAmount, ShakeAmount);
            float offsetY = Random.Range(-ShakeAmount, ShakeAmount);

            TargetObject.transform.position = ObjectDefaultPosition + new Vector3(offsetX, offsetY, 0);
            elapsedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        TargetObject.transform.position = ObjectDefaultPosition; // 흔들기 종료 후 원래 위치로 복구
    }
}