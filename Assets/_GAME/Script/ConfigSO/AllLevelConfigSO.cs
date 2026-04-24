using UnityEngine;

[CreateAssetMenu(fileName = "AllLevel", menuName = "DataConfigSO/AllLevelConfigSO", order = 4)]
public class AllLevelConfigSO : ScriptableObject {
    public DataLevelConfigSO[] levelDatas;

    public DataLevelConfigSO GetLevelData(int level) {
        if (level < 0) {
            return null;
        }
        return levelDatas[level % levelDatas.Length];
    }
}