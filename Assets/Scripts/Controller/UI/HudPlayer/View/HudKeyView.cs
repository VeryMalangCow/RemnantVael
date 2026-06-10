using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudKeyView : MonoBehaviour
{
    [SerializeField] private Image keyItemVfxImg;
    [SerializeField] private List<Image> keyItemImgList;
    [SerializeField] private List<TMP_Text> keyItemAmountTxtList;

    // Init
    public void Init()
    {
        // Key
        for (int i = 0; i < keyItemImgList.Count; i++)
            keyItemImgList[i].gameObject.SetActive(false);

        gameObject.SetActive(true);
    }

    public void SetKeyItem(Dictionary<int, int> keyItemDict)
    {
        for (int i = 0; i < keyItemImgList.Count; i++)
            keyItemImgList[i].gameObject.SetActive(false);
        
        int index = 0;

        foreach (var keyItem in keyItemDict)
        {
            if (keyItem.Value == 0) continue;

            keyItemImgList[index].sprite = ResourceManager.instance.Get_KeyCardSprite(keyItem.Key);
            keyItemAmountTxtList[index].text = keyItem.Value.ToString();
            keyItemImgList[index].gameObject.SetActive(true);

            index++;
        }
    }

    public void EffectKeyIcon(int id)
    {
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < keyItemImgList.Count; i++)
        {
            if (keyItemImgList[i].gameObject.activeSelf && keyItemImgList[i].sprite == ResourceManager.instance.Get_KeyCardSprite(id))
            {
                UnityEngine.Debug.Log("¤·¤·");
                seq.Append(keyItemImgList[i].transform.DOScale(1.3f, 0.1f));
                seq.Append(keyItemImgList[i].transform.DOScale(1f, 0.3f));

                if (keyItemVfxImg.TryGetComponent(out RectTransform vfxRt) &&
                    keyItemImgList[i].TryGetComponent(out RectTransform imgRt))
                {
                    DevTool.SetKillTween(vfxRt);
                    vfxRt.anchoredPosition = imgRt.anchoredPosition;
                    vfxRt.rotation = Quaternion.identity;
                    vfxRt.DORotate(Vector3.forward * 360, 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear);

                    vfxRt.transform.localScale = Vector3.one;
                    vfxRt.DOScale(0.5f, 1f);
                }
                DevTool.SetKillTween(keyItemVfxImg);
                keyItemVfxImg.color = new Color(1, 1, 1, 0.7f);
                keyItemVfxImg.DOFade(0f, 1f);
            }
        }
    }
}
