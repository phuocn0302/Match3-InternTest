using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIPanelMain : MonoBehaviour, IMenu
{
    [SerializeField] private Button btnTimeAttack;

    [SerializeField] private Button btnNormal;
    [SerializeField] private Button btnNormalWin;
    [SerializeField] private Button btnNormalLose;

    private UIMainManager m_mngr;

    private void Awake()
    {
        btnNormal.onClick.AddListener(OnClickNormal);
        btnNormalWin.onClick.AddListener(() => { OnClickNormalAuto(true);});
        btnNormalLose.onClick.AddListener(() => { OnClickNormalAuto(false);});
        btnTimeAttack.onClick.AddListener(OnClickTimeAttack);
    }

    private void OnDestroy()
    {
        if (btnNormal) btnNormal.onClick.RemoveAllListeners();
        if (btnNormalWin) btnNormalWin.onClick.RemoveAllListeners();
        if (btnNormalLose) btnNormalLose.onClick.RemoveAllListeners();
        if (btnTimeAttack) btnTimeAttack.onClick.RemoveAllListeners();
    }

    public void Setup(UIMainManager mngr)
    {
        m_mngr = mngr;
    }

    private void OnClickTimer()
    {
        m_mngr.LoadLevelTimer();
    }

    private void OnClickMoves()
    {
        m_mngr.LoadLevelMoves();
    }
    
    private void OnClickNormal()
    {
        m_mngr.LoadLevelNormal();
    }
    
    private void OnClickNormalAuto(bool doAutoWin)
    {
        m_mngr.LoadLevelNormalAuto(doAutoWin);
    }
    
    private void OnClickTimeAttack()
    {
        m_mngr.LoadLevelTimeAttack();
    }
    
    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
