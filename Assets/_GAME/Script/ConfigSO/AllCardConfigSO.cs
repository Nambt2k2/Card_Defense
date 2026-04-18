using UnityEngine;

[CreateAssetMenu(fileName = "AllCard", menuName = "DataConfigSO/AllCardConfigSO", order = 0)]
public class AllCardConfigSO : ScriptableObject {
    public DataCardConfigSO[] cardDatas;

    [ContextMenu("Sort Card Data")]
    public void SortCardData() {
        System.Array.Sort(cardDatas, (a, b) => a.id.CompareTo(b.id));
    }

    public DataCardConfigSO GetCardData(E_idCard id) {
        if (id < 0 || (int)id >= cardDatas.Length) {
            return null;
        }
        return cardDatas[(int)id];
    }
}