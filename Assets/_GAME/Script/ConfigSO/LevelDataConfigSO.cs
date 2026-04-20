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

    public bool CheckWaveEnemyExist(int wave) {
        // Kiểm tra wave có tồn tại
        return wave >= 0 && wave < waveEnemies.Length;
    }

    public float GetTimeWave(int wave) {
        return GetWaveEnemy(wave).timeWave;
    }
}

[System.Serializable]
public struct S_WaveEnemy {
    public float timeWave;
    public S_EnemySpawnInfo[] enemySpawnInfos;
    public S_EnemySpawnInfo GetEnemySpawnInfo(int index) {
        if (index < 0 || index >= enemySpawnInfos.Length) {
            return enemySpawnInfos[0];
        }
        return enemySpawnInfos[index];
    }
}

[System.Serializable]
public struct S_EnemySpawnInfo {
    public E_idEnemy idEnemy;
    public S_EnemyStepSpawnInfo[] enemyStepSpawnInfos;
}

[System.Serializable]
public struct S_EnemyStepSpawnInfo {
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