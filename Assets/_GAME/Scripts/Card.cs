using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour {
    public Image bg;
    public Image icon;
    public TextMeshProUGUI amountManaTmp;
    public TextMeshProUGUI nameTmp;
    public CurveSO curveScale;


    public void InitUI(Sprite bg, Sprite icon, string name, int mana) {
        this.bg.sprite = bg;
        this.icon.sprite = icon;
        nameTmp.text = name;
        amountManaTmp.text = mana.ToString();
        this.icon.SetNativeSize();
    }

    public void AnimSelect() {
        StopAllCoroutines();
        transform.localScale = new Vector3(1.06f, 1.06f, 1.06f);
    }

    public void AnimDeselect() {
        StopAllCoroutines();
        transform.localScale = Vector3.one;
    }

    public void AnimSpawmNew() {
        StopAllCoroutines();
        gameObject.SetActive(true);
        StartCoroutine(IEScale());
    }

    IEnumerator IEScale() {
        float elapsed = 0f;
        float duration = .25f;
        Vector3 startScale = new Vector3(.75f, .75f, .75f);
        Vector3 targerScale = Vector3.one;
        transform.localScale = startScale;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, targerScale, curveScale.curve.Evaluate(t));
            yield return null;
        }
        transform.localScale = targerScale;
    }
}