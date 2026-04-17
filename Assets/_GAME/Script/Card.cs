using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {
    public Image bg;
    public Image icon;
    public TextMeshProUGUI amountManaTmp;
    public TextMeshProUGUI nameTmp;
    public Image tempBlack, tempWhiteLight, tempOutlineBlur;
    Vector3 posOrigin, posOriginToParent;
    Color colorOutlineBlurDefault;
    float timeAnimCardSelect = .3f;
    Transform parent;
    Coroutine tempWhiteLightCoroutine;
    bool isAnimSpawnNew;

    public void InitUI(Sprite bg, Sprite icon, string name, int mana, bool isTempBlack) {
        this.bg.sprite = bg;
        this.icon.sprite = icon;
        nameTmp.text = name;
        amountManaTmp.text = mana.ToString();
        this.icon.SetNativeSize();
        SetTempBlack(isTempBlack);
        tempWhiteLight.gameObject.SetActive(false);
        isAnimSpawnNew = true;
    }

    public void SetPosCardOrigin() {
        posOrigin = transform.localPosition;
        posOriginToParent = posOrigin + transform.parent.localPosition;
        parent = transform.parent;
        colorOutlineBlurDefault = tempOutlineBlur.color;
    }

    public void AnimSelect(Vector3 position, AnimationCurve curve, Color color) {
        StopAllCoroutines();
        transform.SetParent(parent.parent);
        tempOutlineBlur.color = color;
        StartCoroutine(CurveSO.IELocalMove(this, transform, posOriginToParent + position, timeAnimCardSelect, curve));
        transform.rotation = Quaternion.identity;
    }

    public void AnimDeselect() {
        StopAllCoroutines();
        transform.SetParent(parent);
        tempOutlineBlur.color = colorOutlineBlurDefault;
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

    public void AnimSpawmNew(AnimationCurve curveScale, AnimationCurve curveRotate, Vector3 posTouch, float timeOffset) {
        StopAllCoroutines();
        gameObject.SetActive(true);
        transform.localScale = new Vector3(.75f, .75f, .75f);
        StartCoroutine(CurveSO.IEScale(transform, Vector3.one, .3f + timeOffset, curveScale));
        Vector3 rotateTmp = new Vector3(GamePlayController.Remap(posTouch.y - transform.position.y, 0, 500, -1f, 1f), GamePlayController.Remap(posTouch.x - transform.position.x, -500, 500, 10f, -10f), GamePlayController.Remap(posTouch.x - transform.position.x, -500, 500, -1.2f, 1.2f));
        transform.localRotation = Quaternion.Euler(rotateTmp);
        StartCoroutine(CurveSO.IELocalRotate(transform, rotateTmp, Vector3.zero, .3f + timeOffset, curveRotate, () => isAnimSpawnNew = false));
    }

    public void SetTempImage(bool isActive, AnimationCurve curve, MonoBehaviour script) {
        if (isActive && tempBlack.gameObject.activeSelf && !isAnimSpawnNew)
            SetTempWhiteLight(curve, script);
        else
            tempWhiteLight.gameObject.SetActive(false);
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

    void SetTempWhiteLight(AnimationCurve curve, MonoBehaviour script) {
        tempWhiteLight.gameObject.SetActive(true);
        tempWhiteLightCoroutine = script.StartCoroutine(CurveSO.IEFadeColorImage(tempWhiteLight, .4f, 1, .1f, curve, 
            () => tempWhiteLightCoroutine = script.StartCoroutine(CurveSO.IEFadeColorImage(tempWhiteLight, 1, .3f, .25f, curve, 
            () => tempWhiteLight.gameObject.SetActive(false)))));
    }
}