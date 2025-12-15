using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTimeAttack : LevelCondition
{
    private float m_time;

    private GameManager m_mngr;
    private BoardController m_boardController;
    private BottomBarController m_bottomBarController;
    
    public override void Setup(float value, Text txt, GameManager mngr, BoardController boardController, BottomBarController bottomBarController)
    {
        base.Setup(value, txt, mngr);

        m_mngr = mngr;

        m_time = value;

        m_boardController = boardController;
        m_bottomBarController = bottomBarController;
        
        m_bottomBarController.SetReturnable(true);
        
        m_boardController.OnBoardCleared += HandleOnBoardCleared;
        UpdateText();
    }

    private void Update()
    {
        if (m_conditionCompleted) return;

        if (m_mngr.State != GameManager.eStateGame.GAME_STARTED) return;

        m_time -= Time.deltaTime;

        UpdateText();

        if (m_time <= -1f)
        {
            OnConditionComplete(false);
        }
    }

    protected override void UpdateText()
    {
        if (m_time < 0f) return;

        m_txt.text = string.Format("TIME:\n{0:00}", m_time);
    }
    
    private void HandleOnBoardCleared()
    {
        OnConditionComplete(true);
    }
}