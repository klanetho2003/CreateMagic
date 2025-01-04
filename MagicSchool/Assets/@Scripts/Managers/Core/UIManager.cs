using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager //Sort Order를 관리하기 위해 만든 클래스
{
    int _order = 10;

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    UI_Scene _sceneUI = null;
    public UI_Scene SceneUI { get { return _sceneUI; } }

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true, int sortOrder = 0) //오더를 채워달라(우선순위)
    {
        Canvas canvas = go.GetOrAddComponent<Canvas>();
        if (canvas == null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true; //Canvas가 중첩해서 있을 때, 부모가 어떤 값을 가지던 나만의 소팅오더를 가진다
        }

        CanvasScaler cs = go.GetOrAddComponent<CanvasScaler>();
        if (canvas != null)
        {
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1080, 1920);
        }

        go.GetOrAddComponent<GraphicRaycaster>();

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = sortOrder;
        }
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null, bool pooling = true) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"{name}.prefab", parent, pooling);
        go.transform.SetParent(parent);
        return go.GetOrAddComponent<T>();
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"{name}.prefab");
        T sceneUI = go.GetOrAddComponent<T>();
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    public T ShowPopUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"{name}.prefab");
        T popup = go.GetOrAddComponent<T>();
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);

        RefreshTimeScale();

        return popup;
    }

    public void ClosePopUp(UI_Popup popup) // 매게변수 popup이 삭제가 되는 건지 확인하기 위한 용도
    {
        if (_popupStack.Count == 0)
            return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Clone Popup Failed");
            return;
        }
        ClosePopUp();
    }

    public void ClosePopUp()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _order--;

        RefreshTimeScale();
    }

    public void CloseAllPopUpUI()
    {
        while (_popupStack.Count > 0)
            ClosePopUp();
    }

    public void Clear()
    {
        CloseAllPopUpUI();
        _sceneUI = null;
    }

    public void RefreshTimeScale()
    {
        if (_popupStack.Count > 0)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

}
