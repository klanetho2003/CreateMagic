using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SkillSelectPopup : UI_Popup
{
    Transform _grid;

    List<UI_SkillCardItem> _items = new List<UI_SkillCardItem> ();

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

        PopulateGrid();//그리드 채우기

        return true;
    }

    void PopulateGrid()
    {
        foreach (Transform t in _grid.transform)
        {
            Managers.Resource.Destroy(t.gameObject);
        }

        for (int i = 0; i < 1; i++)
        {
            UI_SkillCardItem item = Managers.UI.MakeSubItem<UI_SkillCardItem>(_grid.transform);

            _items.Add(item);
        }
    }
}
