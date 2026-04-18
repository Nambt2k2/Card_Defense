using UnityEngine;

[CreateAssetMenu(fileName = "AllEnemy", menuName = "DataConfigSO/AllEnemyConfigSO", order = 2)]
public class AllEnemyConfigSO : ScriptableObject {
    public DataEnemyConfigSO[] enemyDatas;

    [ContextMenu("Sort Enemy Data")]
    public void SortEnemyData() {
        System.Array.Sort(enemyDatas, (a, b) => a.id.CompareTo(b.id));
    }

    public DataEnemyConfigSO GetEnemyData(E_idEnemy id) {
        if (id < 0 || (int)id >= enemyDatas.Length) {
            return null;
        }
        return enemyDatas[(int)id];
    }
}