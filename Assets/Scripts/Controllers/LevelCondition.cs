using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCondition : MonoBehaviour
{
    
    // true mean win, false otherwise
    public event Action<bool> ConditionCompleteEvent = delegate { };

    protected Text m_txt;

    protected bool m_conditionCompleted = false;

    public virtual void Setup(float value, Text txt)
    {
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt, GameManager mngr)
    {
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt, BoardController board)
    {
        m_txt = txt;
    }
    
    public virtual void Setup(float value, Text txt, BoardController boardController, BottomBarController bottomBarController)
    {
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt, GameManager mngr, BoardController boardController, 
        BottomBarController bottomBarController)
    {
        m_txt = txt;
    }

    protected virtual void UpdateText() { }

    protected void OnConditionComplete()
    {
        m_conditionCompleted = true;

        ConditionCompleteEvent(true);
    }
    
    protected void OnConditionComplete(bool isWin)
    {
        m_conditionCompleted = true;

        ConditionCompleteEvent(isWin);
    }

    protected virtual void OnDestroy()
    {

    }
}
