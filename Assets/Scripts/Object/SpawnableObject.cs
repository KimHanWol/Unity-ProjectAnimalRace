using System.Collections;
using UnityEngine;

public class SpawnableObject : MonoBehaviour, InteractableInterface
{
    public bool IsActivated = false;
    public float DestroyDuration = 0.5f;

    //InteractableInterface
    public virtual void Interaction(GameObject InteractObject) { }
    //~InteractableInterface

    //TODO: 게임 종료 시에는 아이템 바로 사라지도록 만들기
    public virtual bool IsSpawnable()
    {
        return true;
    }

    private IEnumerator PlayDisappearEffect_Internal()
    {
        BoxCollider2D BoxCollider2D = GetComponent<BoxCollider2D>();
        BoxCollider2D.enabled = false;

        SpriteRenderer SpriteRenderer = GetComponent<SpriteRenderer>();

        float CurrentTime = 0f;
        while (CurrentTime < DestroyDuration)
        {
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, 1f - CurrentTime / DestroyDuration);

            yield return new WaitForSeconds(0.1f);
            CurrentTime += 0.1f;
        }
        SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, 0f);

        Destroy(gameObject);
    }

    protected void SelfDestroy()
    {
        StartCoroutine(PlayDisappearEffect_Internal());
    }
}
