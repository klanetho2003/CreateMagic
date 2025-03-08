using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_SkillsListPopup : UI_Popup
{
	enum GameObjects
	{
		CloseArea,
		EquippedHeroesList,
		WaitingHeroesList,
		UnownedHeroesList,
	}

	enum Texts
	{
		EquippedHeroesCountText,
		WaitingHeroesCountText,
		UnownedHeroesCountText,
	}

	enum Buttons
	{
		CloseButton,
	}

	List<UI_SkillsList_SkillItem> _equippedHeroes = new List<UI_SkillsList_SkillItem>();
	List<UI_SkillsList_SkillItem> _waitingHeroes = new List<UI_SkillsList_SkillItem>();
	List<UI_SkillsList_SkillItem> _unownedHeroes = new List<UI_SkillsList_SkillItem>();

	const int MAX_ITEM_COUNT = 100;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		/*BindObjects(typeof(GameObjects));
		BindTexts(typeof(Texts));
		BindButtons(typeof(Buttons));

		GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
		GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

		{
			var parent = GetObject((int)GameObjects.EquippedHeroesList).transform;
			for (int i = 0; i < MAX_ITEM_COUNT; i++)
			{
				UI_HeroesList_HeroItem item = Managers.UI.MakeSubItem<UI_HeroesList_HeroItem>(parent);
				_equippedHeroes.Add(item);
			}
		}
		{
			var parent = GetObject((int)GameObjects.WaitingHeroesList).transform;
			for (int i = 0; i < MAX_ITEM_COUNT; i++)
			{
				UI_HeroesList_HeroItem item = Managers.UI.MakeSubItem<UI_HeroesList_HeroItem>(parent);
				_waitingHeroes.Add(item);
			}
		}
		{
			var parent = GetObject((int)GameObjects.UnownedHeroesList).transform;
			for (int i = 0; i < MAX_ITEM_COUNT; i++)
			{
				UI_HeroesList_HeroItem item = Managers.UI.MakeSubItem<UI_HeroesList_HeroItem>(parent);
				_unownedHeroes.Add(item);
			}
		}

		Refresh();*/

		return true;
	}

	public void SetInfo()
	{
		Refresh();
	}

	void Refresh()
	{
		if (_init == false)
			return;

		GetText((int)Texts.EquippedHeroesCountText).text = $"{Managers.Game.PickedSkillCount} / ??";
		GetText((int)Texts.WaitingHeroesCountText).text = $"{Managers.Game.OwnedSkillCount} / ??";
		GetText((int)Texts.UnownedHeroesCountText).text = $"{Managers.Game.UnownedSkillCount} / ??";

		Refresh_Hero(_equippedHeroes, SkillOwningState.Picked);
		Refresh_Hero(_waitingHeroes, SkillOwningState.Owned);
		Refresh_Hero(_unownedHeroes, SkillOwningState.Unowned);
	}

	void Refresh_Hero(List<UI_SkillsList_SkillItem> list, SkillOwningState owningState)
	{
		List<SkillSaveData> heroes = Managers.Game.AllSkills.Where(h => h.OwningState == owningState).ToList();

		for (int i = 0; i < list.Count; i++)
		{
			if (i < heroes.Count)
			{
                SkillSaveData hero = heroes[i];
				list[i].SetInfo(hero.DataId);
				list[i].gameObject.SetActive(true);
			}
			else
			{
				list[i].gameObject.SetActive(false);
			}
		}
	}

	void OnClickCloseArea(PointerEventData evt)
	{
		Managers.UI.ClosePopupUI(this);
	}

	void OnClickCloseButton(PointerEventData evt)
	{
		Managers.UI.ClosePopupUI(this);
	}
}
