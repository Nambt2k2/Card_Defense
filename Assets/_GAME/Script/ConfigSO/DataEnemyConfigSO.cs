using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "DataConfigSO/DataEnemyConfigSO", order = 3)]
public class DataEnemyConfigSO : ScriptableObject {
    public E_idEnemy id;
    public E_typeEnemy type;
    public E_areaEnemy area;
    public E_typeMoveEnemy typeMove;
    public E_typeTaget_EnemyNotAtk typeTargetNotAtk;
    public E_element element;
    new public string name;
    public string description;
    public Sprite icon;
    public Enemy prefab;
}