using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "DataConfigSO/DataCardConfigSO")]
public class DataCardConfigSO : ScriptableObject {
    public E_idCard id;
    public E_typeCard type;
    public E_targetCard[] target;
    public E_element element;
    public Vector2Int size;
    public int mana;
    new public string name;
    public string description;
    public Sprite icon;
    public GameObject prefab;
}