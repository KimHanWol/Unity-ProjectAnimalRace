using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LayerData
{
    public Renderer Renderer;
    public float LayerSpeed = 1;
}

public class MapManager : MonoBehaviour
{
    public float MapSpeedRate = 100;
    public LayerData[] LayerDataList;

    private bool NeedToStopMove = false;

    public void UpdateSpeed(float Velocity)
    {
        if(NeedToStopMove == true)
        {
            return;
        }

        foreach(LayerData InLayerData in LayerDataList)
        {
            InLayerData.Renderer.material.mainTextureOffset += new Vector2(Velocity, 0) / MapSpeedRate * InLayerData.LayerSpeed;
        }
    }

    public void EnableMovement(bool Enabled)
    {
        if(NeedToStopMove == Enabled)
        {
            return;
        }

        NeedToStopMove = !Enabled;

        if (NeedToStopMove == true)
        {
            foreach (LayerData InLayerData in LayerDataList)
            {
                InLayerData.Renderer.material.mainTextureOffset = Vector2.zero;
            }
        }
    }
}
