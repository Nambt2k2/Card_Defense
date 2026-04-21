using UnityEngine;

public class Enemy : MonoBehaviour {
    public DataEnemyConfigSO dataEnemyConfig;
    public SpriteRenderer icon;
    Vector3 scaleCur;
    public float speedMove;
    Vector3 targetMove, targetMoveCur;
    float stepMove;

    void Awake() {
        scaleCur = icon.transform.localScale;
    }

    public void Init(AnimationCurve curveScale, float stepMove) {
        this.stepMove = stepMove;
        StartCoroutine(AllCurveConfigSO.IEScaleLoop(this, icon.transform, scaleCur, scaleCur * 1.075f, .5f, curveScale));
    }

    public void SetTargetMove(Vector3 target) {
        targetMove = target;
        targetMoveCur = transform.position + (target - transform.position).normalized * stepMove;
        if (targetMoveCur.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = Vector3.one; 
    }

    public void Move() {
        transform.position = Vector3.MoveTowards(transform.position, targetMoveCur, speedMove * Time.deltaTime);
        if (Vector2.SqrMagnitude(transform.position - targetMoveCur) < 0.005f)
            SetTargetMove(targetMove);
    }
}