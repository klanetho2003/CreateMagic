using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemsList_Item : UI_Base
{
	enum Buttons
	{
        ItemButton,
	}

	enum Images
	{
        ItemImage,
	}

	enum Texts
	{
		ExpText,
		LevelText,
	}

	enum Sliders
	{
        ItemExpSlider,
	}

	int _itemTemplateId = -1;
	int _itemDataId = -1;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        BindImages(typeof(Images));

        GetButton((int)Buttons.ItemButton).gameObject.BindEvent(OnClickItemButton);

        Refresh();

        return true;
	}

	public void SetInfo(int itemTemplateId, int itemInstanceId)
	{
        transform.localScale = Vector3.one;

        _itemTemplateId = itemTemplateId;
        _itemDataId = itemInstanceId;

		Refresh();
	}

	void Refresh()
	{
		if (_init == false)
			return;

		if (_itemDataId < 0)
			return;

		// GetImage((int)Images.HeroImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.SkillDic[_heroDataId].IconImage);
	}

	void OnClickItemButton(PointerEventData evt)
	{
		//UI_HeroInfoPopup popup = Managers.UI.ShowPopupUI<UI_HeroInfoPopup>();
		//popup.SetInfo(_heroDataId);
	}
}
