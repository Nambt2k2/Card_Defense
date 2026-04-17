using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "Curve", menuName = "DataConfigSO/CurveSO")]
public class CurveSO : ScriptableObject {
    public AnimationCurve OutBack, OutQuad;

    public static IEnumerator IEScale(Transform transform, Vector3 targerScale, float duration, AnimationCurve curve) {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, targerScale, curve.Evaluate(t));
            yield return null;
        }
        transform.localScale = targerScale;
    }

     public static IEnumerator IELocalRotate(Transform transform, Vector3 startRotate, Vector3 targetRotate, float duration, AnimationCurve curve) {
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localRotation = Quaternion.Euler(Vector3.Lerp(startRotate, targetRotate, curve.Evaluate(t)));
            yield return null;
        }
        transform.localRotation = Quaternion.Euler(targetRotate);
    }

    public static IEnumerator IELocalMove(MonoBehaviour script, Transform transform, Vector3 targetPosition, float duration, AnimationCurve curve, Action onComplete = null) {
        float elapsed = 0f;
        Vector3 startPosition = transform.localPosition;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, curve.Evaluate(t));
            yield return null;
        }
        transform.localPosition = targetPosition;
        onComplete?.Invoke();
    }

    public static IEnumerator IELocalMoveLoop(MonoBehaviour script, Transform transform, Vector3 targetPosition, float duration, AnimationCurve curve) {
        float elapsed = 0f;
        Vector3 startPosition = transform.localPosition;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, curve.Evaluate(t));
            yield return null;
        }
        transform.localPosition = targetPosition;
        script.StartCoroutine(IELocalMoveLoop(script, transform, startPosition, duration, curve));
    }

    public static IEnumerator IELocalRotateLoop(MonoBehaviour script, Transform transform, Vector3 startRotate, Vector3 targetRotate, float duration, AnimationCurve curve) {
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localRotation = Quaternion.Euler(Vector3.Lerp(startRotate, targetRotate, curve.Evaluate(t)));
            yield return null;
        }
        transform.localRotation = Quaternion.Euler(targetRotate);
        script.StartCoroutine(IELocalRotateLoop(script, transform, targetRotate, startRotate, duration, curve));
    }
}