using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<eStateGame> StateChangedAction = delegate { };

    public enum eLevelMode
    {
        TIMER,
        MOVES,
        TIME_ATTACK,
        NORMAL,
    }

    public enum eStateGame
    {
        SETUP,
        MAIN_MENU,
        GAME_STARTED,
        PAUSE,
        GAME_OVER,
        GAME_WIN,
    }

    private eStateGame m_state;
    public eStateGame State
    {
        get { return m_state; }
        private set
        {
            m_state = value;

            StateChangedAction(m_state);
        }
    }


    private GameSettings m_gameSettings;


    private BoardController m_boardController;
    private BottomBarController m_bottomBarController;

    private UIMainManager m_uiMenu;

    private LevelCondition m_levelCondition;

    private void Awake()
    {
        State = eStateGame.SETUP;

        m_gameSettings = Resources.Load<GameSettings>(Constants.GAME_SETTINGS_PATH);

        m_uiMenu = FindObjectOfType<UIMainManager>();
        m_uiMenu.Setup(this);
    }

    void Start()
    {
        State = eStateGame.MAIN_MENU;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_boardController != null) m_boardController.Update();
        if (m_bottomBarController != null) m_bottomBarController.Update();
    }


    internal void SetState(eStateGame state)
    {
        State = state;

        if(State == eStateGame.PAUSE)
        {
            DOTween.PauseAll();
        }
        else
        {
            DOTween.PlayAll();
        }
    }

    public void LoadLevel(eLevelMode mode)
    {
        m_boardController = new GameObject("BoardController").AddComponent<BoardController>();
        m_boardController.StartGame(this, m_gameSettings);

        m_bottomBarController = new GameObject("BottomBarController").AddComponent<BottomBarController>();
        m_bottomBarController.StartGame(this, m_gameSettings);
        
        m_boardController.OnCellSelected += HandleOnCellSelected;
        m_bottomBarController.OnItemAdded += HandleOnItemAdded;
        m_bottomBarController.OnBarChecked += m_boardController.CheckBoardEmpty;
        
        if (mode == eLevelMode.NORMAL)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelNormal>();
            m_levelCondition.Setup(m_gameSettings.LevelNormal,m_uiMenu.GetLevelConditionView(), m_boardController, m_bottomBarController);
            
            
            
        }
        else if (mode == eLevelMode.TIME_ATTACK)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelTimeAttack>();
            m_levelCondition.Setup(m_gameSettings.LevelTimeAttack, m_uiMenu.GetLevelConditionView(),this, m_boardController, m_bottomBarController);
            
        }

        m_levelCondition.ConditionCompleteEvent += GameOver;
        
        State = eStateGame.GAME_STARTED;
    }

    public void SetAutoWin()
    {
        m_boardController.StartAutoWin();
    }

    public void SetAutoLose()
    {
        m_boardController.StartAutoLose();
    }

    public void GameOver(bool isWin)
    {
        StartCoroutine(WaitBoardController(isWin));
    }

    internal void ClearLevel()
    {
        if (m_boardController)
        {
            m_boardController.Clear();
            Destroy(m_boardController.gameObject);
            m_boardController = null;
        }

        if (m_bottomBarController)
        {
            Destroy(m_bottomBarController.gameObject);
            m_bottomBarController = null;
        }
    }

    private IEnumerator WaitBoardController(bool isWin)
    {
        while (m_boardController.IsBusy)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);

        State = isWin ? eStateGame.GAME_WIN : eStateGame.GAME_OVER;
        
        if (m_levelCondition != null)
        {
            m_levelCondition.ConditionCompleteEvent -= GameOver;
            
            m_boardController.OnCellSelected -= HandleOnCellSelected;
            m_bottomBarController.OnItemAdded -= HandleOnItemAdded;
            m_bottomBarController.OnBarChecked -= m_boardController.CheckBoardEmpty;
            
            Destroy(m_levelCondition);
            m_levelCondition = null;
        }
    }
    
    private void HandleOnCellSelected(Cell cell)
    {
        if (cell != null && cell.Item is NormalItem)
        {
            m_bottomBarController.AddItem(cell.Item as NormalItem);
        }
    }

    private void HandleOnItemAdded(Item item)
    {
        if (item.Cell != null)
        {
            item.Cell.Clickable = false;
        }
    }


}
