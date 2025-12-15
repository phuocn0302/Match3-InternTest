
using UnityEngine.UI;

public class LevelNormal : LevelCondition
{
    protected BoardController m_boardController;
    protected BottomBarController m_bottomBarController;
    
    public override void Setup(float value, Text txt, BoardController boardController, BottomBarController bottomBarController)
    {
        base.Setup(value, txt, boardController, bottomBarController);
        
        m_boardController = boardController;
        m_bottomBarController = bottomBarController;
        
        m_bottomBarController.OnBarFilled += HandleOnBarFilled;
        m_boardController.OnBoardCleared += HandleOnBoardCleared;
    }

    protected override void OnDestroy()
    {
        m_bottomBarController.OnBarFilled -= HandleOnBarFilled;
        m_boardController.OnBoardCleared -= HandleOnBoardCleared;
        
        base.OnDestroy();
    }

    private void HandleOnBarFilled()
    {
        OnConditionComplete(false);
    }

    private void HandleOnBoardCleared()
    {
        OnConditionComplete(true);
    }
    
}
