using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Panel : MonoBehaviour
{
    public List<Button> buttons;

    public virtual void PanelOpen()
    {
        this.gameObject.SetActive(true);
    }

    public void PanelClose(bool soundPlay)
    {
        if (soundPlay)
        {
            StartCoroutine(SoundCoroutine());
        }
        this.gameObject.SetActive(false);
    }

    protected IEnumerator SoundCoroutine()
    {
        SoundManager.Instance.Play("SFX_UI_Button_Keyboard_Enter_Thick_1");
        yield return null;

    }
}
