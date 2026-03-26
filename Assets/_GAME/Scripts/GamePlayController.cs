using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayController : MonoBehaviour {
    public void OnClickReset() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #region MAINFLOW
    public RectTransform canvasWorld;
    void Awake() {
        touchPositionAction = playerInput.actions["Position"];
        touchPressAction = playerInput.actions["Press"];
        offsetCheckToTouch = new Vector3(0, sizeCell * 3, 0);
        offsetCheckToTouch2n = new Vector3(sizeCell / 2, sizeCell * 3, 0);
        InitCard();
        InitMana();
        if (Screen.height / Screen.width >= 2) {
            Camera.main.orthographicSize = Screen.height / 2;
        }
    }

    void Update() {
        UpdateMana();
        if (idCardSelected >= 0 && idCardSelected < curDataCardConfigSOs.Length) {
            if (touchPressAction.IsPressed()) {
                DragCardInGrid(Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>()));
            }
            if (touchPressAction.WasReleasedThisFrame()) {
                DropCardInGrid();
            }
        }
    }
    #endregion
    #region INPUT
    [Header("     --- INPUT ---")]
    public PlayerInput playerInput;
    InputAction touchPositionAction;
    InputAction touchPressAction;
    Vector3 offsetCheckToTouch;
    Vector3 offsetCheckToTouch2n;
    #endregion
    #region GRID
    [Header("     --- GRID ---")]
    public Grid grid;
    public float heightStartGrid;
    public int amountCellStartGrid;
    public Vector2Int size;
    public SpriteRenderer cellPrefab;
    public S_GridCell gridCell;
    public Color colorCellDefault, colorCellActive, colorCellDeactive;
    public float sizeCell;
    List<Vector3Int> indexCellChecks = new List<Vector3Int>();
    Vector3Int indexCellInGridCur;

    [ContextMenu("SetPosCell")]
    public void SetPosCell() {
        for (int i = 0; i < gridCell.rows.Length; i++)
            for (int j = 0; j < gridCell.rows[i].cols.Length; j++)
                DestroyImmediate(gridCell.rows[i].cols[j].cell.gameObject);
        gridCell.rows = new S_ColCell[size.y];
        for (int i = 0; i < size.y; i++)
            gridCell.rows[i].cols = new S_cell[size.x];
        grid.transform.position = new Vector3(0, heightStartGrid, 0);
        grid.transform.position = new Vector3(grid.GetCellCenterWorld(new Vector3Int(-(size.x + 1) / 2, 0, 0)).x, grid.GetCellCenterWorld(new Vector3Int(0, -amountCellStartGrid, 0)).y, 0);
        if (size.x % 2 == 0)
            grid.transform.position -= new Vector3((grid.GetCellCenterWorld(Vector3Int.one).x - grid.GetCellCenterWorld(Vector3Int.zero).x) / 2f, 0, 0);
        for (int i = 0; i < size.x; i++)
            for (int j = 0; j < size.y; j++) {
                gridCell.rows[j].cols[i].cell = Instantiate(cellPrefab, grid.GetCellCenterWorld(new Vector3Int(i, j, 0)), Quaternion.identity, grid.transform);
                gridCell.rows[j].cols[i].isFull = false;
            }
    }

    bool hasLeft, hasRight;
    bool CalcIndexCanPlaces(Vector3Int indexCellMin) {
        Vector2Int size = curDataCardConfigSOs[idCardSelected].size;
        HashSet<Vector2Int> pathWallBlocks = new HashSet<Vector2Int>();
        // bound left
        if (indexCellMin.x > 0) {
            for (int i = 0; i < size.y; i++)
                if (gridCell.rows[indexCellMin.y + i].cols[indexCellMin.x - 1].isFull)
                    pathWallBlocks.Add(new Vector2Int(indexCellMin.x - 1, indexCellMin.y + i));
            // bound left down
            if (indexCellMin.y > 0 && gridCell.rows[indexCellMin.y - 1].cols[indexCellMin.x - 1].isFull)
                pathWallBlocks.Add(new Vector2Int(indexCellMin.x - 1, indexCellMin.y - 1));
        }
        // bound down
        if (indexCellMin.y > 0) {
            for (int i = 0; i < size.x; i++)
                if (gridCell.rows[indexCellMin.y - 1].cols[indexCellMin.x + i].isFull)
                    pathWallBlocks.Add(new Vector2Int(indexCellMin.x + i, indexCellMin.y - 1));
            //bound down right
            if (indexCellMin.x < this.size.x - size.x && gridCell.rows[indexCellMin.y - 1].cols[indexCellMin.x + size.x].isFull)
                pathWallBlocks.Add(new Vector2Int(indexCellMin.x + size.x, indexCellMin.y - 1));
        }
        // bound right
        if (indexCellMin.x < this.size.x - size.x) {
            for (int i = 0; i < size.y; i++)
                if (gridCell.rows[indexCellMin.y + i].cols[indexCellMin.x + size.x].isFull)
                    pathWallBlocks.Add(new Vector2Int(indexCellMin.x + size.x, indexCellMin.y + i));
            //bound right up
            if (indexCellMin.y < this.size.y - size.y && gridCell.rows[indexCellMin.y + size.y].cols[indexCellMin.x + size.x].isFull)
                pathWallBlocks.Add(new Vector2Int(indexCellMin.x + size.x, indexCellMin.y + size.y));
        }
        // bound up
        if (indexCellMin.y < this.size.y - size.y) {
            for (int i = 0; i < size.x; i++)
                if (gridCell.rows[indexCellMin.y + size.y].cols[indexCellMin.x + i].isFull)
                    pathWallBlocks.Add(new Vector2Int(indexCellMin.x + i, indexCellMin.y + size.y));
            // bound up left
            if (indexCellMin.x > 0 && gridCell.rows[indexCellMin.y + size.y].cols[indexCellMin.x - 1].isFull)
                pathWallBlocks.Add(new Vector2Int(indexCellMin.x - 1, indexCellMin.y + size.y));
        }
        if (pathWallBlocks.Count == 0)
            return true;
        else {
            hasLeft = hasRight = false;
            for (int i = 0; i < indexCellChecks.Count; i++)
                pathWallBlocks.Add(new Vector2Int(indexCellChecks[i].x, indexCellChecks[i].y));
            foreach (Vector2Int block in pathWallBlocks) {
                if (block.x == 0)
                    hasLeft = true;
                else if (block.x == this.size.x - 1)
                    hasRight = true;
            }
            List<Vector2Int> pathWallBlocksTemp = pathWallBlocks.ToList();
            foreach (Vector2Int block in pathWallBlocksTemp) {
                CheckCellPath8Dir(block, ref pathWallBlocks);
                if (hasLeft && hasRight)
                    return false;
            }
            return !hasLeft || !hasRight;
        }
    }

    void CheckCellPath8Dir(Vector2Int cell, ref HashSet<Vector2Int> pathWallBlocks) {
        if (hasLeft && hasRight)
            return;
        List<Vector2Int> cells = new List<Vector2Int>();
        // bound left
        if (cell.x > 0) {
            cells.Add(new Vector2Int(cell.x - 1, cell.y));
            // bound left down
            if (cell.y > 0)
                cells.Add(new Vector2Int(cell.x - 1, cell.y - 1));
        }
        // bound down
        if (cell.y > 0) {
            cells.Add(new Vector2Int(cell.x, cell.y - 1));
            //bound down right
            if (cell.x < size.x - 1)
                cells.Add(new Vector2Int(cell.x + 1, cell.y - 1));
        }
        // bound right
        if (cell.x < size.x - 1) {
            cells.Add(new Vector2Int(cell.x + 1, cell.y));
            //bound right up
            if (cell.y < size.y - 1)
                cells.Add(new Vector2Int(cell.x + 1, cell.y + 1));
        }
        // bound up
        if (cell.y < size.y - 1) {
            cells.Add(new Vector2Int(cell.x, cell.y + 1));
            // bound up left
            if (cell.x > 0)
                cells.Add(new Vector2Int(cell.x - 1, cell.y + 1));
        }
        for (int i = 0; i < cells.Count; i++)
            if (!pathWallBlocks.Contains(cells[i]))
                if (gridCell.rows[cells[i].y].cols[cells[i].x].isFull) {
                    pathWallBlocks.Add(cells[i]);
                    if (cells[i].x == 0) {
                        hasLeft = true;
                        if (hasLeft && hasRight)
                            return;
                    } else if (cells[i].x == size.x - 1) {
                        hasRight = true;
                        if (hasLeft && hasRight)
                            return;
                    }
                    CheckCellPath8Dir(cells[i], ref pathWallBlocks);
                }
    }

    #endregion
    #region CARD
    [Header("     --- CARD ---")]
    public DataCardConfigSO[] dataCardConfigSOs;
    public DataCardConfigSO[] curDataCardConfigSOs;
    public Transform towerParent;
    public int idCardSelected;
    bool canDropCard;
    public Card cardPrefab;
    public Card[] curCards;
    public Transform[] slotCards;
    public Sprite bgWood, bgFire, bgEarth, bgMetal, bgWater;

    void InitCard() {
        idCardSelected = -1;
        curDataCardConfigSOs = new DataCardConfigSO[slotCards.Length];
        curCards = new Card[slotCards.Length];
        for (int i = 0; i < curDataCardConfigSOs.Length; i++) {
            curDataCardConfigSOs[i] = RandomDataCard();
            curCards[i] = Instantiate(cardPrefab, slotCards[i]);
            curCards[i].InitUI(GetBgElementCard(curDataCardConfigSOs[i].element), curDataCardConfigSOs[i].icon, curDataCardConfigSOs[i].name, curDataCardConfigSOs[i].mana);
        }
    }

    void DragCardInGrid(Vector3 posTouch) {
        Vector3Int indexCellTmp = grid.WorldToCell(posTouch + (curDataCardConfigSOs[idCardSelected].size.x % 2 == 0 ? offsetCheckToTouch2n : offsetCheckToTouch));
        if (indexCellInGridCur == indexCellTmp)
            return;
        indexCellInGridCur = indexCellTmp;
        canDropCard = true;
        for (int i = 0; i < indexCellChecks.Count; i++)
            gridCell.rows[indexCellChecks[i].y].cols[indexCellChecks[i].x].cell.color = colorCellDefault;
        indexCellChecks.Clear();
        Color c = colorCellActive;
        for (int i = -curDataCardConfigSOs[idCardSelected].size.x / 2; i < curDataCardConfigSOs[idCardSelected].size.x - curDataCardConfigSOs[idCardSelected].size.x / 2; i++)
            for (int j = 0; j < curDataCardConfigSOs[idCardSelected].size.y; j++)
                if (indexCellInGridCur.x + i < size.x && indexCellInGridCur.y + j < size.y && indexCellInGridCur.x + i >= 0 && indexCellInGridCur.y + j >= 0) {
                    if (!gridCell.rows[indexCellInGridCur.y + j].cols[indexCellInGridCur.x + i].isFull)
                        indexCellChecks.Add(new Vector3Int(indexCellInGridCur.x + i, indexCellInGridCur.y + j, 0));
                    else {
                        canDropCard = false;
                        c = colorCellDeactive;
                    }
                } else {
                    canDropCard = false;
                    c = colorCellDeactive;
                }
        if (canDropCard) {
            canDropCard = CalcIndexCanPlaces(indexCellChecks[0]);
            if (!canDropCard)
                c = colorCellDeactive;
        }
        for (int i = 0; i < indexCellChecks.Count; i++)
            gridCell.rows[indexCellChecks[i].y].cols[indexCellChecks[i].x].cell.color = c;
    }

    void DropCardInGrid() {
        for (int i = 0; i < indexCellChecks.Count; i++)
            gridCell.rows[indexCellChecks[i].y].cols[indexCellChecks[i].x].cell.color = colorCellDefault;
        curCards[idCardSelected].AnimDeselect();
        if (canDropCard) {
            UseMana(curDataCardConfigSOs[idCardSelected].mana);
            for (int i = 0; i < indexCellChecks.Count; i++)
                gridCell.rows[indexCellChecks[i].y].cols[indexCellChecks[i].x].isFull = true;
            Instantiate(curDataCardConfigSOs[idCardSelected].prefab, grid.CellToWorld(indexCellChecks[0]) + new Vector3(sizeCell * curDataCardConfigSOs[idCardSelected].size.x / 2, sizeCell * curDataCardConfigSOs[idCardSelected].size.y / 2, 0), Quaternion.identity, towerParent);
            curCards[idCardSelected].gameObject.SetActive(false);
            curDataCardConfigSOs[idCardSelected] = RandomDataCard();
            curCards[idCardSelected].InitUI(GetBgElementCard(curDataCardConfigSOs[idCardSelected].element), curDataCardConfigSOs[idCardSelected].icon, curDataCardConfigSOs[idCardSelected].name, curDataCardConfigSOs[idCardSelected].mana);
            curCards[idCardSelected].AnimSpawmNew();
        }
        indexCellChecks.Clear();
        idCardSelected = -1;
    }

    DataCardConfigSO RandomDataCard() {
        return dataCardConfigSOs[Random.Range(0, dataCardConfigSOs.Length)];
    }

    Sprite GetBgElementCard(E_element element) {
        switch (element) {
            case E_element.Kim:
                return bgMetal;
            case E_element.Mộc:
                return bgWood;
            case E_element.Thủy:
                return bgWater;
            case E_element.Hỏa:
                return bgFire;
            case E_element.Thổ:
                return bgEarth;
        }
        return null;
    }

    public void OnClickCard(int index) {
        if (!CheckMana(curDataCardConfigSOs[index].mana))
            return;
        idCardSelected = index;
        curCards[idCardSelected].AnimSelect();
        indexCellInGridCur = -Vector3Int.one;       
    }
    #endregion
    #region MANA
    [Header("     --- MANA ---")]
    public Image progessMana;
    public TextMeshProUGUI textMana;
    public int amountManaMax;
    public float secondsPerMana;
    float countTimeMana;
    int amountManaCur;

    void InitMana() {
        amountManaCur = 0;
        UpdateUIMana();
    }

    void UpdateMana() {
        if (amountManaCur == amountManaMax)
            return;
        countTimeMana += Time.deltaTime;
        if (countTimeMana >= secondsPerMana) {
            countTimeMana = 0;
            amountManaCur++;
            if (amountManaCur > amountManaMax)
                amountManaCur = amountManaMax;
            UpdateUIMana();
        }
    }

    bool CheckMana(int amount) {
        return amountManaCur >= amount;
    }

    void UseMana(int amount) {
        if (!CheckMana(amount))
            return;
        amountManaCur -= amount;
        if (amountManaCur < 0)
            amountManaCur = 0;
        UpdateUIMana();
    }

    void UpdateUIMana() {
        textMana.text = amountManaCur.ToString();
        progessMana.fillAmount = (float)amountManaCur / amountManaMax;
    }
    #endregion
}
#region CELL
[System.Serializable]
public struct S_cell {
    public SpriteRenderer cell;
    public bool isFull;
}

[System.Serializable]
public struct S_ColCell {
    public S_cell[] cols;
}

[System.Serializable]
public struct S_GridCell {
    public S_ColCell[] rows;
}
#endregion