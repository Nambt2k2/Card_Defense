using UnityEngine;

public class Enemy : MonoBehaviour {
    public DataEnemyConfigSO dataEnemyConfig;
    public SpriteRenderer icon;
    Vector3 scaleCur;
    public float speedMove;
    Vector3 targetMove;

    void Awake() {
        scaleCur = icon.transform.localScale;
    }

    public void Init(AnimationCurve curveScale) {

        StartCoroutine(AllCurveConfigSO.IEScaleLoop(this, icon.transform, scaleCur, scaleCur * 1.075f, .65f, curveScale));
    }

    public void SetTargetMove(Vector3 target) {
        targetMove = target;
    }

    public void Move() {
        transform.position = Vector3.MoveTowards(transform.position, targetMove, speedMove * Time.deltaTime);    
    }
}