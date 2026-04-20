using UnityEngine;

public class Enemy : MonoBehaviour {
    public DataEnemyConfigSO dataEnemyConfig;
    public float speedMove;
    Vector3 targetMove;

    public void Init() {
        
    }

    public void SetTargetMove(Vector3 target) {
        targetMove = target;
    }

    public void Move() {
        transform.position = Vector3.MoveTowards(transform.position, targetMove, speedMove * Time.deltaTime);    
    }
}