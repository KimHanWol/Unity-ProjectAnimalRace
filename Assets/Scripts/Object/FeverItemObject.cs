using System.Collections;
using UnityEngine;

public class FeverItemObject : RuningObject
{
    //InteractableInterface
    public override void Interaction(GameObject InteractObject)
    {
        if (IsActivated == true)
        {
            return;
        }

        base.Interaction(InteractObject);

        IsActivated = true;
        EventManager.Instance.OnFeverStateChangedEvent?.Invoke(true);

        SelfDestroy();
    }
    //~InteractableInterface
}
