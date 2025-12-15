using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public event Action OnMoveEvent = delegate { };
    public event Action<Cell> OnCellSelected;
    public event Action OnBoardCleared;

    public bool IsBusy { get; private set; }

    private Board m_board;

    private GameManager m_gameManager;

    private bool m_isDragging;

    private Camera m_cam;

    private Collider2D m_hitCollider;

    private GameSettings m_gameSettings;

    private List<Cell> m_potentialMatch;

    private Cell m_lastHoveredCell;

    private float m_timeAfterFill;

    private bool m_hintIsShown;

    private bool m_gameOver;
    private bool m_isAuto = false;

    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;

        m_gameSettings = gameSettings;

        m_gameManager.StateChangedAction += OnGameStateChange;

        m_cam = Camera.main;

        m_board = new Board(this.transform, gameSettings);

        Fill();
    }

    private void Fill()
    {
        m_board.Fill();
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_OVER:
                m_gameOver = true;
                break;
        }
    }


    public void Update()
    {
        if (m_gameOver) return;
        if (IsBusy) return;
        if (m_isAuto) return;

        var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        Cell currentCell = null;

        if (hit.collider)
        {
            Cell hitCell = hit.collider.GetComponent<Cell>();

            if (hitCell != null && hitCell.Item != null)
            {
                currentCell = hitCell;
            }
        }

        AnimateHover(currentCell);

        if (Input.GetMouseButtonDown(0))
        {
            if (currentCell != null && currentCell.Clickable && currentCell.Item != null)
            {
                currentCell.Item.View.DOKill();
                currentCell.Item.View.localScale = Vector3.one;
                m_lastHoveredCell = null;
                OnCellSelected?.Invoke(currentCell);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ResetRayCast();
        }
    }

    private void AnimateHover(Cell currentCell)
    {
        if (currentCell != m_lastHoveredCell)
        {
            if (m_lastHoveredCell != null && m_lastHoveredCell.Item != null && m_lastHoveredCell.Item.View != null)
            {
                m_lastHoveredCell.Item.View.DOKill();
                m_lastHoveredCell.Item?.View.DOScale(1.0f, 0.2f);
            }

            if (currentCell != null && currentCell.Item != null && currentCell.Item.View != null)
            {
                currentCell.Item.View.DOKill();
                currentCell.Item.View.DOScale(1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            }

            m_lastHoveredCell = currentCell;
        }
    }

    private void ResetRayCast()
    {
        m_isDragging = false;
        m_hitCollider = null;
    }
    
    [ContextMenu("StartAutoLose")]
    public void StartAutoLose()
    {
        m_isAuto = true;
        StartCoroutine(AutoLoseCoroutine());
    }

    private IEnumerator AutoLoseCoroutine()
    {
        List<NormalItem.eNormalType> pickedTypes = new List<NormalItem.eNormalType>();

        while (!m_gameOver)
        {
            var allCells = m_board.GetAllCellsWithItems();

            if (allCells.Count == 0) yield break;

            Cell targetCell = null;

            targetCell = allCells.FirstOrDefault(c =>
                c.Item is NormalItem item && !pickedTypes.Contains(item.ItemType));

            if (targetCell == null)
            {
                targetCell = allCells[UnityEngine.Random.Range(0, allCells.Count)];
            }

            if (targetCell != null && targetCell.Clickable)
            {
                if (targetCell.Item != null && targetCell.Item.View != null)
                {
                    targetCell.Item.View.DOKill();
                    targetCell.Item.View.localScale = Vector3.one;

                    pickedTypes.Add((targetCell.Item as NormalItem).ItemType);
                }

                if (m_lastHoveredCell == targetCell) m_lastHoveredCell = null;

                OnCellSelected?.Invoke(targetCell);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
    
    [ContextMenu("StartAutoWin")]
    public void StartAutoWin()
    {
        m_isAuto = true;
        StartCoroutine(AutoWinCoroutine());
    }

    private IEnumerator AutoWinCoroutine()
    {
        List<Cell> remainingCells = m_board.GetAllCellsWithItems();

        while (!m_gameOver)
        {
            if (remainingCells.Count == 0)
            {
                yield break;
            }

            var matchGroup = remainingCells
                .Where(c => c.Item != null) 
                .GroupBy(c => (c.Item as NormalItem).ItemType)
                .FirstOrDefault(g => g.Count() >= 3);

            if (matchGroup != null)
            {
                var cellsToClick = matchGroup.Take(3).ToList();

                foreach (var cell in cellsToClick)
                {
                    if (m_gameOver) yield break;

                    if (cell.Item != null && cell.Item.View != null)
                    {
                        cell.Item.View.DOKill();
                        cell.Item.View.localScale = Vector3.one;
                    }
                
                    if (m_lastHoveredCell == cell) m_lastHoveredCell = null;

                    OnCellSelected?.Invoke(cell);

                    remainingCells.Remove(cell);

                    yield return new WaitForSeconds(0.5f);
                }
            }
            else
            {
                yield break;
            }
        }
    }

    public void CheckBoardEmpty()
    {
        if (m_board.IsEmpty())
        {
            OnBoardCleared?.Invoke();
        }
    }

    private void FindMatchesAndCollapse(Cell cell1, Cell cell2)
    {
        if (cell1.Item is BonusItem)
        {
            cell1.ExplodeItem();
            StartCoroutine(ShiftDownItemsCoroutine());
        }
        else if (cell2.Item is BonusItem)
        {
            cell2.ExplodeItem();
            StartCoroutine(ShiftDownItemsCoroutine());
        }
        else
        {
            List<Cell> cells1 = GetMatches(cell1);
            List<Cell> cells2 = GetMatches(cell2);

            List<Cell> matches = new List<Cell>();
            matches.AddRange(cells1);
            matches.AddRange(cells2);
            matches = matches.Distinct().ToList();

            if (matches.Count < m_gameSettings.MatchesMin)
            {
                m_board.Swap(cell1, cell2, () => { IsBusy = false; });
            }
            else
            {
                OnMoveEvent();

                CollapseMatches(matches, cell2);
            }
        }
    }

    private void FindMatchesAndCollapse()
    {
        List<Cell> matches = m_board.FindFirstMatch();

        if (matches.Count > 0)
        {
            CollapseMatches(matches, null);
        }
        else
        {
            m_potentialMatch = m_board.GetPotentialMatches();
            if (m_potentialMatch.Count > 0)
            {
                IsBusy = false;

                m_timeAfterFill = 0f;
            }
            else
            {
                //StartCoroutine(RefillBoardCoroutine());
                StartCoroutine(ShuffleBoardCoroutine());
            }
        }
    }

    private List<Cell> GetMatches(Cell cell)
    {
        List<Cell> listHor = m_board.GetHorizontalMatches(cell);
        if (listHor.Count < m_gameSettings.MatchesMin)
        {
            listHor.Clear();
        }

        List<Cell> listVert = m_board.GetVerticalMatches(cell);
        if (listVert.Count < m_gameSettings.MatchesMin)
        {
            listVert.Clear();
        }

        return listHor.Concat(listVert).Distinct().ToList();
    }

    private void CollapseMatches(List<Cell> matches, Cell cellEnd)
    {
        for (int i = 0; i < matches.Count; i++)
        {
            matches[i].ExplodeItem();
        }

        if (matches.Count > m_gameSettings.MatchesMin)
        {
            m_board.ConvertNormalToBonus(matches, cellEnd);
        }

        StartCoroutine(ShiftDownItemsCoroutine());
    }

    private IEnumerator ShiftDownItemsCoroutine()
    {
        m_board.ShiftDownItems();

        yield return new WaitForSeconds(0.2f);

        m_board.FillGapsWithNewItems();

        yield return new WaitForSeconds(0.2f);

        FindMatchesAndCollapse();
    }

    private IEnumerator RefillBoardCoroutine()
    {
        m_board.ExplodeAllItems();

        yield return new WaitForSeconds(0.2f);

        m_board.Fill();

        yield return new WaitForSeconds(0.2f);

        FindMatchesAndCollapse();
    }

    private IEnumerator ShuffleBoardCoroutine()
    {
        m_board.Shuffle();

        yield return new WaitForSeconds(0.3f);

        FindMatchesAndCollapse();
    }


    private void SetSortingLayer(Cell cell1, Cell cell2)
    {
        if (cell1.Item != null) cell1.Item.SetSortingLayerHigher();
        if (cell2.Item != null) cell2.Item.SetSortingLayerLower();
    }

    private bool AreItemsNeighbor(Cell cell1, Cell cell2)
    {
        return cell1.IsNeighbour(cell2);
    }

    internal void Clear()
    {
        m_board.Clear();
    }

    private void ShowHint()
    {
        m_hintIsShown = true;
        foreach (var cell in m_potentialMatch)
        {
            cell.AnimateItemForHint();
        }
    }

    private void StopHints()
    {
        m_hintIsShown = false;
        foreach (var cell in m_potentialMatch)
        {
            cell.StopHintAnimation();
        }

        m_potentialMatch.Clear();
    }
}