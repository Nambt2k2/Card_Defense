using UnityEngine;

[CreateAssetMenu(fileName = "Curve", menuName = "DataConfigSO/CurveSO")]
public class CurveSO : ScriptableObject {
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1.8f), new Keyframe(1f, 1f, 0.4f, 0f));
}