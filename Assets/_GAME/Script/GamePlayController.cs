using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayController : MonoBehaviour {
    #region MAINFLOW
    public E_stateGame stateGame;
    Camera camMain;
    [Header("     --- OrderDisplay ---")]
    public int orderCellDefault;
    public int orderCellTowerDefault, orderCellTowerDrag;
    public int orderTowerDefault, orderTowerDrag;
    [Header("     --- INPUT ---")]
    public PlayerInput playerInput;
    InputAction touchPositionAction;
    InputAction touchPressAction;
    Vector3 offsetCheckToTouch;
    Vector3 offsetCheckToTouch2n;
    Vector3 posTouchCur;

    void Awake() {
        stateGame = E_stateGame.Init;
        touchPositionAction = playerInput.actions["Position"];
        touchPressAction = playerInput.actions["Press"];
        offsetCheckToTouch = new Vector3(0, sizeCell * 3, 0);
        offsetCheckToTouch2n = new Vector3(sizeCell / 2, sizeCell * 3, 0);
        StartCoroutine(IEAnimGrid());
        InitCard();
        InitHp();
        InitMana();
        camMain = Camera.main;
        if (Screen.height / Screen.width >= (16f / 9)) {
            camMain.fieldOfView = Camera.HorizontalToVerticalFieldOfView(25, (float)Screen.width / Screen.height);
        }
    }

    void Update() {
        switch (stateGame) {
            case E_stateGame.Playing:
                UpdateMana();
                if (idCardSelected >= 0 && idCardSelected < curDataCardConfigSOs.Length) {
                    if (touchPressAction.IsPressed()) {
                        posTouchCur = touchPositionAction.ReadValue<Vector2>();
                        posTouchCur.z = -camMain.transform.position.z;
                        posTouchCur = camMain.ScreenToWorldPoint(posTouchCur);
                        DragCardInGrid(posTouchCur);
                    } else if (touchPressAction.WasReleasedThisFrame()) {
                        DropCardInGrid();
                    }
                }
                break;
        }
    }

    void Lose() {
        stateGame = E_stateGame.Lose;
        for (int i = 0; i < eventTriggerslotCards.Length; i++)
            eventTriggerslotCards[i].enabled = false;
        StopAllCoroutines();
    }
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

    IEnumerator IEAnimGrid() {
        yield return null;
        for (int i = 0; i < gridCell.rows.Length; i++) {
            for (int j = 0; j < gridCell.rows[i].cols.Length; j++)
                gridCell.rows[i].cols[j].cell.enabled = true;
            yield return new WaitForSeconds(.05f);
        }
        StartCoroutine(IEAnimeMoveSlotCardStart());
    }

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
                gridCell.rows[j].cols[i].cell.sortingOrder = orderCellDefault;
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
    public Color colorCardOutlineSelected;
    public Card cardPrefab;
    public Card[] curCards;
    public Transform[] slotCards;
    public EventTrigger[] eventTriggerslotCards;
    public Transform storageCard;
    public Sprite bgWood, bgFire, bgEarth, bgMetal, bgWater;
    Tower[] towerInCardCurs;
    public HashSet<Tower> towerCurs = new HashSet<Tower>();
    public Vector3 poscardSelect;
    public Vector3 offsetMoveCardSelect;
    public Vector3 offsetMoveSlotCard;
    public float timeAnimMoveSlotCard, timeDelayAnimMoveSlotCard;
    public Vector3 rotateStartSlotCard, rotateTargetSlotCard;
    public CurveSO curveSO;
    bool isSelectedCard;

    void InitCard() {
        idCardSelected = -1;
        isSelectedCard = false;
        curDataCardConfigSOs = new DataCardConfigSO[slotCards.Length];
        towerInCardCurs = new Tower[slotCards.Length];
        towerCurs.Clear();
        curCards = new Card[slotCards.Length];
        for (int i = 0; i < curDataCardConfigSOs.Length; i++) {
            curDataCardConfigSOs[i] = RandomDataCard();
            curCards[i] = Instantiate(cardPrefab, slotCards[i]);
            curCards[i].InitUI(GetBgElementCard(curDataCardConfigSOs[i].element), curDataCardConfigSOs[i].icon, curDataCardConfigSOs[i].name, curDataCardConfigSOs[i].mana, CheckMana(curDataCardConfigSOs[i].mana));
        }
        for (int i = 0; i < curCards.Length; i++)
            curCards[i].SetPosCardOrigin();
        for (int i = 0; i < curCards.Length; i++) {
            curCards[i].transform.position = storageCard.position;
            curCards[i].gameObject.SetActive(false);
        }
        StartCoroutine(IEAnimeMoveSlotCard());
    }

    IEnumerator IEAnimeMoveSlotCardStart() {
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < curCards.Length; i++) {
            StartCoroutine(CurveSO.IELocalMove(this, curCards[i].transform, Vector3.zero, .3f + (curCards.Length - i - 2) * 0.03f, curveSO.OutQuad));
            curCards[i].AnimSpawmNew(curveSO.OutBack, curveSO.OutQuad, slotCards[i].position - storageCard.position, (curCards.Length - i - 2) * 0.03f);
            yield return new WaitForSeconds(.12f);
        }
        for (int i = 0; i < eventTriggerslotCards.Length; i++) {
            eventTriggerslotCards[i].enabled = true;
        }
        stateGame = E_stateGame.Playing;
    }

    IEnumerator IEAnimeMoveSlotCard() {
        for (int i = 0; i < slotCards.Length; i++) {
            slotCards[i].localRotation = Quaternion.Euler(rotateStartSlotCard);
        }
        for (int i = 0; i < slotCards.Length; i++) {
            StartCoroutine(CurveSO.IELocalMoveLoop(this, slotCards[i], slotCards[i].localPosition + offsetMoveSlotCard, timeAnimMoveSlotCard, curveSO.OutQuad));
            StartCoroutine(CurveSO.IELocalRotateLoop(this, slotCards[i], rotateStartSlotCard, rotateTargetSlotCard, timeAnimMoveSlotCard, curveSO.OutQuad));
            yield return new WaitForSeconds(timeDelayAnimMoveSlotCard);
        }
    }

    void DragCardInGrid(Vector3 posTouch) {
        Vector3 posTouchTmp = posTouch + (curDataCardConfigSOs[idCardSelected].size.x % 2 == 0 ? offsetCheckToTouch2n : offsetCheckToTouch);
        Vector3Int indexCellTmp = grid.WorldToCell(posTouchTmp);
        RotateCardFollowDrag(posTouchTmp);
        if (indexCellInGridCur == indexCellTmp)
            return;
        indexCellInGridCur = indexCellTmp;
        canDropCard = true;
        indexCellChecks.Clear();
        Color colorCellCheckCur = colorCellActive;
        for (int i = -curDataCardConfigSOs[idCardSelected].size.x / 2; i < curDataCardConfigSOs[idCardSelected].size.x - curDataCardConfigSOs[idCardSelected].size.x / 2; i++)
            for (int j = 0; j < curDataCardConfigSOs[idCardSelected].size.y; j++)
                if (indexCellInGridCur.x + i < size.x && indexCellInGridCur.y + j < size.y && indexCellInGridCur.x + i >= 0 && indexCellInGridCur.y + j >= 0) {
                    if (!gridCell.rows[indexCellInGridCur.y + j].cols[indexCellInGridCur.x + i].isFull)
                        indexCellChecks.Add(new Vector3Int(indexCellInGridCur.x + i, indexCellInGridCur.y + j, 0));
                    else {
                        canDropCard = false;
                        colorCellCheckCur = colorCellDeactive;
                    }
                } else {
                    canDropCard = false;
                    colorCellCheckCur = colorCellDeactive;
                    // towerInCardCurs[idCardSelected].Hide();
                    // return;
                }
        if (canDropCard) {
            canDropCard = CalcIndexCanPlaces(indexCellChecks[0]);
            if (!canDropCard)
                colorCellCheckCur = colorCellDeactive;
        }
        towerInCardCurs[idCardSelected].DisplayCellCheckOnDrag(colorCellCheckCur, grid.CellToWorld(new Vector3Int(indexCellInGridCur.x - curDataCardConfigSOs[idCardSelected].size.x / 2, indexCellInGridCur.y, 0)) + new Vector3(sizeCell * curDataCardConfigSOs[idCardSelected].size.x / 2, sizeCell * curDataCardConfigSOs[idCardSelected].size.y / 2, 0));
    }

    void RotateCardFollowDrag(Vector3 posTouch) {
        curCards[idCardSelected].transform.localRotation = Quaternion.Euler(Mathf.Clamp(Remap(posTouch.y - curCards[idCardSelected].transform.position.y, 0, 500, -1f, 1f), -1f, 1f), Mathf.Clamp(Remap(posTouch.x - curCards[idCardSelected].transform.position.x, -500, 500, 10f, -10f), -10f, 10f), Mathf.Clamp(Remap(posTouch.x - curCards[idCardSelected].transform.position.x, -500, 500, -1.2f, 1.2f), -1.2f, 1.2f));
    }

    public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax) {
        if (Mathf.Approximately(fromMax, fromMin))
            return toMin;
        float normalized = (value - fromMin) / (fromMax - fromMin);
        return toMin + normalized * (toMax - toMin);
    }

    void DropCardInGrid() {
        for (int i = 0; i < indexCellChecks.Count; i++) {
            gridCell.rows[indexCellChecks[i].y].cols[indexCellChecks[i].x].cell.color = colorCellDefault;
            gridCell.rows[indexCellChecks[i].y].cols[indexCellChecks[i].x].cell.sortingOrder = orderCellDefault;
        }
        for (int i = 0; i < curCards.Length; i++) {
            if (i < idCardSelected)
                curCards[i].MoveToDefault();
            else if (i == idCardSelected) {
                curCards[idCardSelected].AnimDeselect();
                HideUIManaUseCard();
                isSelectedCard = false;
            } else
                curCards[i].MoveToDefault();
        }
        if (canDropCard) {
            UseMana(curDataCardConfigSOs[idCardSelected].mana);
            for (int i = 0; i < indexCellChecks.Count; i++)
                gridCell.rows[indexCellChecks[i].y].cols[indexCellChecks[i].x].isFull = true;
            towerInCardCurs[idCardSelected].Drop(orderTowerDefault, true);
            towerCurs.Add(towerInCardCurs[idCardSelected]);
            towerInCardCurs[idCardSelected] = null;
            curCards[idCardSelected].gameObject.SetActive(false);
            eventTriggerslotCards[idCardSelected].enabled = false;
            curCards[idCardSelected].transform.position = storageCard.position;
            curDataCardConfigSOs[idCardSelected] = RandomDataCard();
            curCards[idCardSelected].InitUI(GetBgElementCard(curDataCardConfigSOs[idCardSelected].element), curDataCardConfigSOs[idCardSelected].icon, curDataCardConfigSOs[idCardSelected].name, curDataCardConfigSOs[idCardSelected].mana, CheckMana(curDataCardConfigSOs[idCardSelected].mana));
            int idCardTemp = idCardSelected;
            StartCoroutine(CurveSO.IELocalMove(this, curCards[idCardSelected].transform, Vector3.zero, .3f + (curCards.Length - idCardSelected - 2) * 0.03f, curveSO.OutQuad, () => eventTriggerslotCards[idCardTemp].enabled = true));
            curCards[idCardSelected].AnimSpawmNew(curveSO.OutBack, curveSO.OutQuad, slotCards[idCardSelected].position - storageCard.position, (curCards.Length - idCardSelected - 2) * 0.03f);
            slotCards[idCardSelected].SetSiblingIndex(slotCards.Length - 1);
        } else {
            towerInCardCurs[idCardSelected].Drop(orderTowerDefault, false);
        }
        foreach (Tower t in towerCurs)
            t.HideCellCheck();
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
        if (isSelectedCard) {
            DropCardInGrid();
            return;
        }
        indexCellInGridCur = -Vector3Int.one;
        idCardSelected = index;
        for (int i = 0; i < curCards.Length; i++) {
            if (i < idCardSelected)
                curCards[i].Move(offsetMoveCardSelect, curveSO.OutBack);
            else if (i == idCardSelected) {
                curCards[idCardSelected].AnimSelect(poscardSelect, curveSO.OutBack, colorCardOutlineSelected);
                ShowUIManaUseCard();
                isSelectedCard = true;
            } else
                curCards[i].Move(offsetMoveCardSelect, curveSO.OutBack);
        }
        if (towerInCardCurs[idCardSelected] == null)
            towerInCardCurs[idCardSelected] = Instantiate(curDataCardConfigSOs[idCardSelected].prefab, towerParent);
        posTouchCur = touchPositionAction.ReadValue<Vector2>();
        posTouchCur.z = -camMain.transform.position.z;
        posTouchCur = camMain.ScreenToWorldPoint(posTouchCur);
        DragCardInGrid(posTouchCur);
        towerInCardCurs[idCardSelected].StartDrag(orderTowerDrag, orderCellTowerDrag, canDropCard);
        foreach (Tower t in towerCurs)
            t.DisplayCellCheckInGrid(orderCellTowerDefault, colorCellDeactive);
    }

    void UpdateUICardFollowMana() {
        for (int i = 0; i < curCards.Length; i++)
            curCards[i].SetTempImage(CheckMana(curDataCardConfigSOs[i].mana), curveSO.OutQuad, this);
    }
    #endregion
    #region HP
    [Header("     --- HP ---")]
    public float amountHpMax;
    public Image progessHp;
    public RectTransform progessHPBound;

    [ContextMenu("TestBaseTakeDame")]
    public void TestBaseTakeDame() {
        BaseTakeDame(10);
    }

    void InitHp() {
        amountHpCur = amountHpMax;
        UpdateUIHp();
    }

    void UpdateUIHp() {
        progessHPBound.gameObject.SetActive(amountHpCur < amountHpMax && amountHpCur > 0);
        progessHp.fillAmount = amountHpCur / amountHpMax;
        progessHPBound.sizeDelta = new Vector2(progessHp.fillAmount * progessHp.rectTransform.sizeDelta.x, progessHPBound.sizeDelta.y);
    }

    void BaseTakeDame(float dame) {
        amountHpCur -= dame;
        if (amountHpCur < 0)
            amountHpCur = 0;
        if (amountHpCur == 0)
            Lose();
        UpdateUIHp();
    }
    float amountHpCur;
    #endregion
    #region MANA
    [Header("     --- MANA ---")]
    public Image progessMana;
    public RectTransform maskManaUseCard;
    public Image imgProgessManaUseCard;
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
        UpdateUIMana(true);
    }

    void UpdateUIMana(bool isUseCard = false) {
        UpdateUICardFollowMana();
        textMana.text = amountManaCur.ToString();
        progessMana.fillAmount = (float)amountManaCur / amountManaMax;
        if (idCardSelected >= 0 && !isUseCard)
            ShowUIManaUseCard();
    }

    void ShowUIManaUseCard() {
        maskManaUseCard.gameObject.SetActive(true);
        maskManaUseCard.sizeDelta = new Vector2(imgProgessManaUseCard.rectTransform.sizeDelta.x * progessMana.fillAmount, imgProgessManaUseCard.rectTransform.sizeDelta.y);
        imgProgessManaUseCard.fillAmount = 1 - (float)(amountManaCur - curDataCardConfigSOs[idCardSelected].mana) / amountManaMax;
        textMana.text = (amountManaCur - curDataCardConfigSOs[idCardSelected].mana).ToString();
    }

    void HideUIManaUseCard() {
        textMana.text = amountManaCur.ToString();
        maskManaUseCard.gameObject.SetActive(false);
    }
    #endregion
    #region EVENTONCLICK
    public void OnClickReset() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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