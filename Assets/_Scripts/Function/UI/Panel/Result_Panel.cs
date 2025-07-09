using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result_Panel : Panel
{

    public Major_Panel major_Panel;
    public Particular_Panel particular_Panel;

    private void OnEnable()
    {
        buttons[0].interactable = false;
        buttons[1].onClick.AddListener(Title_BTN);
        Time.timeScale = 0;
    }
    private void OnDisable()
    {
        buttons[0].interactable = true;
    }
    private void Title_BTN()
    {
        Time.timeScale = 1;
        UnitManager.Instance.GetPlayer().GetComponent<AugmentSelector>().RemoveAllAugments();
        Destroy(GameManager.Instance.gameObject);
        Destroy(UnitManager.Instance.gameObject);
        Destroy(ProjectileManager.Instance.gameObject);
        Destroy(TimeManager.Instance.gameObject);
        Destroy(SoundManager.Instance.gameObject);
        GameSceneManager.SceneLoad("Title");
    }

}
