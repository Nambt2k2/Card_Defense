using UnityEngine;

public class Tower : MonoBehaviour {
    public SpriteRenderer icon;
    public SpriteRenderer iconCellCheck;

    public void StartDrag(int sortingOrderTowerDrag, int sortingOrderCellCheckTower, bool canDragCard) {
        gameObject.SetActive(canDragCard);
        icon.sortingOrder = sortingOrderTowerDrag;
        iconCellCheck.gameObject.SetActive(true);
        iconCellCheck.sortingOrder = sortingOrderCellCheckTower;
    }

    public void Drop(int sortingOrderTower, bool canDrop) {
        if (canDrop)
            icon.sortingOrder = sortingOrderTower;
        else
            gameObject.SetActive(false);
        HideCellCheck();   
    }

    public void DisplayCellCheckOnDrag(Color colorCellCheck, Vector3 pos) {
        gameObject.SetActive(true);
        transform.position = pos;
        iconCellCheck.color = colorCellCheck;
    }

    public void DisplayCellCheckInGrid(int sortingOrderCellCheckTowerInGrid, Color colorCellCheck) {
        iconCellCheck.gameObject.SetActive(true);
        iconCellCheck.color = colorCellCheck;
        iconCellCheck.sortingOrder = sortingOrderCellCheckTowerInGrid;
    }

    public void HideCellCheck() {
        iconCellCheck.gameObject.SetActive(false);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
