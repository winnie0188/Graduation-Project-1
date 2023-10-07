using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadePanel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Image image;
    public static FadePanel fadePanel;

    private void Awake()
    {
        fadePanel = this;
    }

    public bool isFinsh()
    {
        return !image.gameObject.activeSelf;
    }

    public void playAni()
    {
        image.gameObject.SetActive(true);
        image.color = new Color(0, 0, 0, 1);
        Sequence s = DOTween.Sequence();
        s.Append(image.DOFade(1, 1f));
        s.Append(image.DOFade(0, 0.7f).SetEase(Ease.Flash));
        s.Play();

        s.OnComplete(() =>
        {
            image.gameObject.SetActive(false);
        });
    }

}
