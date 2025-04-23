using System.Collections;
using System.Linq;
using UnityEngine;

public class FeverItemObject : SpawnableObject
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

        // 현재 동물이 처음 피버 상태면
        AnimalType CurrentAnimalType = GameManager.Instance.Player.GetCurrentAnimalType();
        if (AnimalDataManager.Instance.UnlockedFeverAnimalList.Contains(CurrentAnimalType) == false)
        {
            EventManager.Instance.OnNewAnimalFeverUnlockedEvent?.Invoke(CurrentAnimalType);
        }

        SoundManager SoundManager = SoundManager.Instance;
        SoundManager.PlaySFX(SoundManager.ESFX.SFX_EAT);

        SelfDestroy();
    }
    //~InteractableInterface
}
