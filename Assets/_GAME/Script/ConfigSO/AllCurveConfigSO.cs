using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "AllCurve", menuName = "DataConfigSO/AllCurveConfigSO", order = 6)]
public class AllCurveConfigSO : ScriptableObject {
    public AnimationCurve OutBack, OutQuad;

    public static IEnumerator IEValueChange(float startValue, float targetValue, float duration, AnimationCurve curve, Action<float> onUpdate = null, Action onComplete = null) {
        float elapsed = 0f;
        float valueTmp = startValue;
        onUpdate?.Invoke(startValue);
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            valueTmp = Mathf.Lerp(startValue, targetValue, curve.Evaluate(t));
            onUpdate?.Invoke(valueTmp);
            yield return null;
        }
        onUpdate?.Invoke(targetValue);
        onComplete?.Invoke();
    }

    public static IEnumerator IEScale(Transform transform, Vector3 startScale, Vector3 targetScale, float duration, AnimationCurve curve, Action onComplete = null) {
        float elapsed = 0f;
        transform.localScale = startScale;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, curve.Evaluate(t));
            yield return null;
        }
        transform.localScale = targetScale;
        onComplete?.Invoke();
    }

    public static IEnumerator IEScaleLoop(MonoBehaviour script, Transform transform, Vector3 startScale, Vector3 targetScale, float duration, AnimationCurve curve) {
        float elapsed = 0f;
        transform.localScale = startScale;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, curve.Evaluate(t));
            yield return null;
        }
        transform.localScale = targetScale;
        script.StartCoroutine(IEScaleLoop(script, transform, targetScale, startScale, duration, curve));
    }

    public static IEnumerator IELocalRotate(Transform transform, Vector3 startRotate, Vector3 targetRotate, float duration, AnimationCurve curve, Action onComplete = null) {
        float elapsed = 0f;
        transform.localRotation = Quaternion.Euler(startRotate);
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localRotation = Quaternion.Euler(Vector3.Lerp(startRotate, targetRotate, curve.Evaluate(t)));
            yield return null;
        }
        transform.localRotation = Quaternion.Euler(targetRotate);
        onComplete?.Invoke();
    }

    public static IEnumerator IELocalRotateLoop(MonoBehaviour script, Transform transform, Vector3 startRotate, Vector3 targetRotate, float duration, AnimationCurve curve) {
        float elapsed = 0f;
        transform.localRotation = Quaternion.Euler(startRotate);
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localRotation = Quaternion.Euler(Vector3.Lerp(startRotate, targetRotate, curve.Evaluate(t)));
            yield return null;
        }
        transform.localRotation = Quaternion.Euler(targetRotate);
        script.StartCoroutine(IELocalRotateLoop(script, transform, targetRotate, startRotate, duration, curve));
    }

    public static IEnumerator IELocalMove(MonoBehaviour script, Transform transform, Vector3 startPosition, Vector3 targetPosition, float duration, AnimationCurve curve, Action onComplete = null) {
        float elapsed = 0f;
        transform.localPosition = startPosition;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, curve.Evaluate(t));
            yield return null;
        }
        transform.localPosition = targetPosition;
        onComplete?.Invoke();
    }

    public static IEnumerator IELocalMoveLoop(MonoBehaviour script, Transform transform, Vector3 startPosition, Vector3 targetPosition, float duration, AnimationCurve curve) {
        float elapsed = 0f;
        transform.localPosition = startPosition;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, curve.Evaluate(t));
            yield return null;
        }
        transform.localPosition = targetPosition;
        script.StartCoroutine(IELocalMoveLoop(script, transform, targetPosition, startPosition, duration, curve));
    }

    public static IEnumerator IEFadeColorImage(Image img, float startAlpha, float endAlpha, float duration, AnimationCurve curve, Action onComplete = null) {
        float elapsed = 0f;
        Color startColor = img.color;
        startColor.a = startAlpha;
        img.color = startColor; 
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, endAlpha);
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            img.color = Color.Lerp(startColor, targetColor, curve.Evaluate(t));
            yield return null;
        }
        img.color = targetColor;
        onComplete?.Invoke();
    }

    public static IEnumerator IEColorSprite(SpriteRenderer sprite, Color startColor, Color targetColor, float duration, AnimationCurve curve, Action onComplete = null) {
        float elapsed = 0f;
        sprite.color = startColor;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            sprite.color = Color.Lerp(startColor, targetColor, curve.Evaluate(t));
            yield return null;
        }
        sprite.color = targetColor;
        onComplete?.Invoke();
    }

    public static IEnumerator IEColorImage(Image image, Color startColor, Color targetColor, float duration, AnimationCurve curve, Action onComplete = null) {
        float elapsed = 0f;
        image.color = startColor;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            image.color = Color.Lerp(startColor, targetColor, curve.Evaluate(t));
            yield return null;
        }
        image.color = targetColor;
        onComplete?.Invoke();
    }

    public static IEnumerator IEFadeCanvas(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration, float delay, AnimationCurve curve, Action onComplete = null) {
        float elapsed = 0f;
        canvasGroup.alpha = startAlpha;
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, curve.Evaluate(t));
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
        onComplete?.Invoke();
    }
}