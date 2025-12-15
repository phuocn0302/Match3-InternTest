using System.Linq; // Needed for Sum() and SelectMany()
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BottomBarController : MonoBehaviour
{
    public event Action<Item> OnItemAdded;
    public event Action OnBarFilled;
    public event Action OnBarChecked;

    private Camera m_cam;

    public float BottomPadding = 0.2f;
    public float ClickRadius = 0.5f;

    private int m_size;

    private int m_numOfItems;

    // Item pos
    private float m_posX;
    private float m_posY;

    private bool m_isBarFilled = false;
    private bool m_isReturnable = false;
    private bool m_isInAnim = false;


    private Dictionary<NormalItem.eNormalType, List<NormalItem>> m_items =
        new Dictionary<NormalItem.eNormalType, List<NormalItem>>();

    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_size = gameSettings.BottomBarSize;
        m_numOfItems = 0;
        m_cam = Camera.main;

        m_posX = -m_size * 0.5f + 0.5f;
        m_posY = -Camera.main.orthographicSize + 0.5f + BottomPadding;

        CreateBar();
    }

    public void Update()
    {
        if (!m_isReturnable || m_isInAnim) return;
    
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);
        
            NormalItem clickedItem = FindItemNearPosition(mousePos);

            if (clickedItem != null)
            {
                ReturnItemToBoard(clickedItem);
            }
        }
    }

    private void CreateBar()
    {
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);

        for (int i = 0; i < m_size; i++)
        {
            GameObject go = GameObject.Instantiate(prefabBG, this.transform);
            go.transform.position = new Vector3(m_posX + i, m_posY, 0f);
        }
    }

    public void SetReturnable(bool v)
    {
        m_isReturnable = v;
    }

    public void AddItem(NormalItem item)
    {
        if (m_numOfItems >= m_size) return;

        if (m_items.ContainsKey(item.ItemType))
        {
            m_items[item.ItemType].Add(item);
        }
        else
        {
            List<NormalItem> newList = new List<NormalItem>();
            newList.Add(item);
            m_items.Add(item.ItemType, newList);
        }

        m_numOfItems++;
        OnItemAdded?.Invoke(item);

        CheckAndUpdateVisual();
    }

    private void CheckAndUpdateVisual(bool isCheckMate = true)
    {
        if (m_isBarFilled) return;

        Sequence sequence = DOTween.Sequence();

        int index = 0;
        foreach (var item in m_items)
        {
            foreach (var i in item.Value)
            {
                Vector3 targetPos = new Vector3(m_posX + index, m_posY, 0f);
                i.View.DOKill();
                sequence.Join(i.View.DOMove(targetPos, 0.5f));

                m_isInAnim = true;

                index++;
            }
        }

        if (isCheckMate)
        {
            sequence.AppendInterval(1f);
            sequence.OnComplete(() =>
            {
                m_isInAnim = false;
                CheckMate();
            });
        }
    }

    private void CheckMate()
    {
        if (m_isBarFilled) return;
        Sequence sequence = DOTween.Sequence();

        var matches = m_items.Where(x => x.Value.Count >= 3).ToList();

        if (matches.Count == 0)
        {
            if (m_numOfItems >= m_size)
            {
                OnBarFilled?.Invoke();
                m_isBarFilled = true;
            }

            return;
        }


        foreach (var match in matches)
        {
            var itemType = match.Key;
            var allItemsOfType = match.Value;

            List<NormalItem> itemsToRemove = allItemsOfType.Take(3).ToList();

            foreach (var item in itemsToRemove)
            {
                allItemsOfType.Remove(item);
            }

            m_numOfItems -= 3;

            if (allItemsOfType.Count == 0)
            {
                m_items.Remove(itemType);
            }

            foreach (var item in itemsToRemove)
            {
                m_isInAnim = true;
                sequence.Join(item.View.DOScale(0f, 0.2f).OnComplete(() => { item.Clear(); }));
            }
        }

        sequence.OnComplete(() =>
        {
            m_isBarFilled = false;
            CheckAndUpdateVisual(false);
            OnBarChecked?.Invoke();
        });
    }
    
    private void ReturnItemToBoard(NormalItem item)
    {
        m_isInAnim = true;

        if (m_items.ContainsKey(item.ItemType))
        {
            m_items[item.ItemType].Remove(item);
            if (m_items[item.ItemType].Count == 0) m_items.Remove(item.ItemType);
        }

        m_numOfItems--;
        
        // In case bar is filled result in visual does not change
        m_isBarFilled = false;

        CheckAndUpdateVisual(false);

        item.View.DOKill();
        Sequence seq = DOTween.Sequence();

        seq.Join(item.View.DOMove(item.Cell.transform.position, 0.4f));
        seq.Join(item.View.DOScale(1.0f, 0.4f));

        seq.OnComplete(() =>
        {
            item.Cell.Clickable = true;
            m_isInAnim = false;
        });
    }
    
    private NormalItem FindItemNearPosition(Vector2 position)
    {
        foreach (var list in m_items.Values)
        {
            foreach (var item in list)
            {
                // Check distance between Mouse and Item
                if (Vector2.Distance(position, item.View.position) <= ClickRadius)
                {
                    return item;
                }
            }
        }
        return null;
    }
}