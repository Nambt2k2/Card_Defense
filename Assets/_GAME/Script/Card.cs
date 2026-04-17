using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {
    public Image bg;
    public Image icon;
    public TextMeshProUGUI amountManaTmp;
    public TextMeshProUGUI nameTmp;
    public Image tempBlack, tempWhiteLight;
    Vector3 posOrigin, posOriginToParent;
    float timeAnimCardSelect = .3f;
    Transform parent;
    Coroutine tempWhiteLightCoroutine;

    public void InitUI(Sprite bg, Sprite icon, string name, int mana, bool isTempBlack) {
        this.bg.sprite = bg;
        this.icon.sprite = icon;
        nameTmp.text = name;
        amountManaTmp.text = mana.ToString();
        this.icon.SetNativeSize();
        SetTempBlack(isTempBlack);
        tempWhiteLight.gameObject.SetActive(false);
    }

    public void SetPosCardOrigin() {
        posOrigin = transform.localPosition;
        posOriginToParent = posOrigin + transform.parent.localPosition;
        parent = transform.parent;
    }

    public void AnimSelect(Vector3 position, AnimationCurve curve) {
        StopAllCoroutines();
        transform.SetParent(parent.parent);
        StartCoroutine(CurveSO.IELocalMove(this, transform, posOriginToParent + position, timeAnimCardSelect, curve));
        transform.rotation = Quaternion.identity;
    }

    public void AnimDeselect() {
        StopAllCoroutines();
        transform.SetParent(parent);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void Move(Vector3 offset, AnimationCurve curve) {
        StopAllCoroutines();
        StartCoroutine(CurveSO.IELocalMove(this, transform, posOrigin + offset, timeAnimCardSelect, curve));
    }

    public void MoveToDefault() {
        StopAllCoroutines();
        transform.localPosition = posOrigin;
    }

    public void AnimSpawmNew(AnimationCurve curveMove, AnimationCurve curveRotate, Vector3 posTouch, float timeOffset) {
        StopAllCoroutines();
        gameObject.SetActive(true);
        transform.localScale = new Vector3(.75f, .75f, .75f);
        StartCoroutine(CurveSO.IEScale(transform, Vector3.one, .3f + timeOffset, curveMove));
        Vector3 rotateTmp = new Vector3(GamePlayController.Remap(posTouch.y - transform.position.y, 0, 500, -1f, 1f), GamePlayController.Remap(posTouch.x - transform.position.x, -500, 500, 10f, -10f), GamePlayController.Remap(posTouch.x - transform.position.x, -500, 500, -1.2f, 1.2f));
        transform.localRotation = Quaternion.Euler(rotateTmp);
        StartCoroutine(CurveSO.IELocalRotate(transform, rotateTmp, Vector3.zero, .3f + timeOffset, curveRotate));
    }

    public void SetTempImage(bool isActive) {
        if (isActive)
            tempWhiteLightCoroutine = StartCoroutine(IESetTempWhiteLight(isActive));
        SetTempBlack(isActive);
    }

    void SetTempBlack(bool isDeactive) {
        tempBlack.gameObject.SetActive(!isDeactive);
        if (!isDeactive) {
            if (tempWhiteLightCoroutine != null)
                StopCoroutine(tempWhiteLightCoroutine);
            tempWhiteLight.gameObject.SetActive(false);
        }

    }

    IEnumerator IESetTempWhiteLight(bool isActive) {
        if (isActive && tempBlack.gameObject.activeSelf) {
            tempWhiteLight.color = Color.white;
            tempWhiteLight.gameObject.SetActive(true);
            tempWhiteLight.CrossFadeAlpha(.5f, .3f, true);
            yield return new WaitForSeconds(.3f);
            tempWhiteLight.gameObject.SetActive(false);
        } else
            tempWhiteLight.gameObject.SetActive(false);
    }
}