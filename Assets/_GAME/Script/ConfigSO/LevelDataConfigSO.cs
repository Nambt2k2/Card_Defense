using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "DataConfigSO/DataLevelConfigSO", order = 5)]
public class DataLevelConfigSO : ScriptableObject {
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
}

[System.Serializable]
public struct S_WaveEnemy {
    [Tooltip("Thời gian chờ trước khi bắt đầu wave này tính từ khi tiêu diệt hết enemy ở wave trước, nếu là 0 thì sẽ bắt đầu ngay lập tức")]
    public float timeWaitStartWave;
    public S_EnemySpawnInfo[] enemySpawnInfos;
    public S_EnemySpawnInfo GetEnemySpawnInfo(int index) {
        if (index < 0 || index >= enemySpawnInfos.Length) {
            return enemySpawnInfos[0];
        }
        return enemySpawnInfos[index];
    }
    
    public bool CheckEnemySpawnInfoExist(int index) {
        // Kiểm tra enemy spawn info có tồn tại
        return index >= 0 && index < enemySpawnInfos.Length;
    }

    public bool IsStartWaveNow() {
        // kiểm tra có next wave luôn hay không
        return timeWaitStartWave == 0;
    }
}

[System.Serializable]
public struct S_EnemySpawnInfo {
    public E_idEnemy idEnemynemy;
    [Tooltip("Thời gian chờ trước khi spawn lượt enemy này tính từ khi thời điểm spawn lượt enemy trước (là lượt đầu trong wave thì sẽ tính từ thời điểm bắt đầu wave), nếu là 0 thì sẽ spawn ngay lập tức")]
    public float timeWaitSpawn;
    public int amountEnemySpawn;
    public int[] indexCellXCanSpawn;
    
    public int GetIndexCellXSpawn(int rangeIndex) {
        if (indexCellXCanSpawn == null || indexCellXCanSpawn.Length == 0) {
            return Random.Range(0, rangeIndex);
        }
        // nếu có logic chi tiết về vị trí spawn random ra thì bổ sung thêm vào đây
        return indexCellXCanSpawn[Random.Range(0, indexCellXCanSpawn.Length)];
    }

    public bool IsSpawnEnemyNow() {
        // kiểm tra có spawn enemy luôn hay không
        return timeWaitSpawn == 0;
    }
}