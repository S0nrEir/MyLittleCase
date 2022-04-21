using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DynamicScrollView : MonoBehaviour
{

    #region PUBLIC_VARIABLES
    public int NoOfItems;

    public GameObject Item;

    public GridLayoutGroup GridLayout;

    public RectTransform ScrollContent;

    public ScrollRect ScrollRect;
    #endregion

    #region PRIVATE_VARIABLES
    #endregion

    #region UNITY_CALLBACKS
    void OnEnable()
    {
        InitializeList();
    }
    #endregion

    #region PUBLIC_METHODS
    #endregion

    #region PRIVATE_METHODS
    private void ClearOldElement()
    {
        for (int i = 0; i < GridLayout.transform.childCount; i++)
        {
            Destroy(GridLayout.transform.GetChild(i).gameObject);
        }
    }



    public void SetContentHeight()
    {
        float scrollContentHeight = (GridLayout.transform.childCount * GridLayout.cellSize.y) + ((GridLayout.transform.childCount - 1) * GridLayout.spacing.y);
        ScrollContent.sizeDelta = new Vector2(400, scrollContentHeight);
    }

    private void InitializeList()
    {
        ClearOldElement();
        for (int i = 0; i < NoOfItems; i++)
            InitializeNewItem("" + (i + 1));
        SetContentHeight();
    }

    private void InitializeNewItem(string name)
    {
        GameObject newItem = Instantiate(Item) as GameObject;
        newItem.name = name;
        newItem.transform.parent = GridLayout.transform;
        newItem.transform.localScale = Vector3.one;
        newItem.SetActive(true);
    }
    #endregion

    #region PRIVATE_COROUTINES
    private IEnumerator MoveTowardsTarget(float time,float from,float target) {
        float i = 0;
        float rate = 1 / time;
        while(i<1){
            i += rate * Time.deltaTime;
            ScrollRect.verticalNormalizedPosition = Mathf.Lerp(from,target,i);
            yield return 0;
        }
    }
    #endregion

    #region DELEGATES_CALLBACKS
    #endregion

    #region UI_CALLBACKS
    public void AddNewElement()
    {
        InitializeNewItem("" + (GridLayout.transform.childCount + 1));
        SetContentHeight();
        StartCoroutine(MoveTowardsTarget(0.2f, ScrollRect.verticalNormalizedPosition, 0));
    }
    #endregion


}
