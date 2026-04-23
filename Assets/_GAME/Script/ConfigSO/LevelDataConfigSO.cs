using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "DataConfigSO/DataLevelConfigSO", order = 5)]
public class DataLevelConfigSO : ScriptableObject {
    public float timeWaitStartLevel;
    public S_WaveEnemy[] waveEnemies;

    public S_WaveEnemy GetWaveEnemy(int wave) {
        if (wave < 0 || wave >= waveEnemies.Length) {
            return waveEnemies[0];
        }
        return waveEnemies[wave];
    }

    public int GetAmountEnemySpawnInWave(S_WaveEnemy wave) {
        int amount = 0;
        for (int i = 0; i < wave.enemySpawnInfos.Length; i++)
            if (wave.enemySpawnInfos[i].timeSpawmEnemy >= 0)
                amount += wave.enemySpawnInfos[i].amountEnemySpawn;
        return amount;
    }

    public bool CheckWaveEnemyExist(int wave) {
        // Kiểm tra wave có tồn tại
        return wave >= 0 && wave < waveEnemies.Length;
    }
}

[System.Serializable]
public struct S_WaveEnemy {
    public float timeWave;
    public S_EnemySpawnInfo[] enemySpawnInfos;

    public List<S_EnemySpawnInfo> GetListEnemySpawnInWave(int wave) {
        List<S_EnemySpawnInfo> results = enemySpawnInfos.ToList();
        results.Sort((S_EnemySpawnInfo a, S_EnemySpawnInfo b) => b.timeSpawmEnemy.CompareTo(a.timeSpawmEnemy));
        return results;
    }
}

[System.Serializable]
public struct S_EnemySpawnInfo {
    public E_idEnemy idEnemy;
    public float timeSpawmEnemy;
    public int amountEnemySpawn;
    public int[] indexCellXCanSpawn;
    public int amountLoopSpawm;
    public float timeWaitLoopSpawm;

    public int GetIndexCellXSpawn(int rangeIndex) {
        if (indexCellXCanSpawn == null || indexCellXCanSpawn.Length == 0) {
            return Random.Range(0, rangeIndex);
        }
        // nếu có logic chi tiết về vị trí spawn random ra thì bổ sung thêm vào đây
        return indexCellXCanSpawn[Random.Range(0, indexCellXCanSpawn.Length)];
    }
}