using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomAnimationComponent : MonoBehaviour
{
    [Header("Animation")]
    public float JumpEffectForce = 100f;
    public float ShakeAmount = 0.1f;
    public float GrowMaxSizeScale = 3f;

    public void PlayJumpEffect()
    {
        Rigidbody2D PlayerRigidbody = GetComponentInParent<Rigidbody2D>();
        PlayerRigidbody.velocity = Vector2.zero;
        PlayerRigidbody.AddForce(new Vector2(0, JumpEffectForce));
    }

    public void PlayShakeEffect(float ShakeDuration)
    {
        StartCoroutine(ShakeObject(ShakeDuration));
    }

    private IEnumerator ShakeObject(float ShakeDuration)
    {
        Vector3 OriginalPosition = transform.parent.position;
        float elapsedTime = 0f;

        while (elapsedTime < ShakeDuration)
        {
            float offsetX = Random.Range(-ShakeAmount, ShakeAmount);
            float offsetY = Random.Range(-ShakeAmount, ShakeAmount);

            transform.parent.position = OriginalPosition + new Vector3(offsetX, offsetY, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.parent.position = OriginalPosition; // 흔들기 종료 후 원래 위치로 복구
    }

    public void TurnReverse(bool Reverse)
    {
        if (Reverse == true)
        {
            transform.parent.rotation = Quaternion.Euler(transform.parent.eulerAngles + new Vector3(0f, 180f, 0f));
        }
        else
        {
            transform.parent.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }
    }

    public void GrowCharacter(bool IsGrow, float TransformDuration)
    {
        StartCoroutine(GrowCharacter_Internal(IsGrow, TransformDuration));
    }

    private IEnumerator GrowCharacter_Internal(bool IsGrow, float TransformDuration)
    {
        float CurrentTime = 0f;

        if (IsGrow == true)
        {
            // 플레이어 크기 확대
            while (CurrentTime < TransformDuration)
            {
                float NewScale = 1f + CurrentTime / TransformDuration * (GrowMaxSizeScale - 1);
                transform.parent.localScale = new Vector3(NewScale, NewScale, NewScale);

                yield return new WaitForSeconds(0.01f);
                CurrentTime += 0.01f;
            }
            transform.parent.localScale = new Vector3(GrowMaxSizeScale, GrowMaxSizeScale, GrowMaxSizeScale);
        }
        else
        {
            // 플레이어 크기 축소
            float MaxScale = transform.parent.localScale.x;
            float ShrinkUnit = (MaxScale - 1f) * 0.01f / TransformDuration;

            while (CurrentTime < TransformDuration)
            {
                transform.parent.localScale -= new Vector3(ShrinkUnit, ShrinkUnit, ShrinkUnit);

                yield return new WaitForSeconds(0.01f);
                CurrentTime += 0.01f;
            }
            transform.parent.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}