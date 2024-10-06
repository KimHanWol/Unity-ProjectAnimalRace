using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject CurrentMapObject;
    public GameObject NextMapObject;

    //0 좌표로부터 얼마큼 떨어져야 맵 위치가 바뀌는 지
    public int MaxDistanceFromZero = 18;

    public void OnPlayerAccelerated(float NewVelocity)
    {
        Rigidbody2D CurrentMapRigid2D = CurrentMapObject.GetComponent<Rigidbody2D>();
        Rigidbody2D NextMapRigid2D = NextMapObject.GetComponent<Rigidbody2D>();

        Vector2 MovementVector = new Vector2(-NewVelocity * GameManager.Get().RunVelocityRate, 0);
        CurrentMapRigid2D.AddForce(MovementVector);
        NextMapRigid2D.AddForce(MovementVector);
    }

    void Update()
    {
        if(CurrentMapObject.transform.localPosition.x < -MaxDistanceFromZero)
        {
            Vector3 NewPosition = CurrentMapObject.transform.position;
            NewPosition.x = MaxDistanceFromZero;
            CurrentMapObject.transform.position = NewPosition;

            GameObject SwitchTempObject = CurrentMapObject;
            CurrentMapObject = NextMapObject;
            NextMapObject = SwitchTempObject;
        }
    }
}
