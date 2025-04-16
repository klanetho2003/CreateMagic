using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ItemSelectPopup : UI_Popup
{
    Transform _grid;

    List<UI_ItemSelect_CardItem> _items = new List<UI_ItemSelect_CardItem> ();

    enum GameObjects
    {
        SkillCardSelectListObject
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        _grid = GetObject((int)GameObjects.SkillCardSelectListObject).transform;

        return true;
    }

    public void SetInfo(List<Data.ItemData> rewards)
    {
        PopulateGrid(rewards);//그리드 채우기
    }

    void PopulateGrid(List<Data.ItemData> rewards)
    {
        foreach (Transform t in _grid.transform)
            Managers.Resource.Destroy(t.gameObject);

        for (int i = 0; i < rewards.Count; i++)
        {
            UI_ItemSelect_CardItem selectCard = Managers.UI.MakeSubItem<UI_ItemSelect_CardItem>(_grid.transform);
            selectCard.SetInfo(rewards[i]);

            _items.Add(selectCard);
        }
    }
}
