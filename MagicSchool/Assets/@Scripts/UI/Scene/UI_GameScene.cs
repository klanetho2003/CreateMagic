using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_GameScene : UI_Scene
{
    enum GameObjects
    {
        WaveList,
        NavSkillItemList,
    }
    enum Texts
    {
        SkillValueText,
    }

    List<UI_GameScene_WaveItem> _waveItems = new List<UI_GameScene_WaveItem>();
    List<UI_GameScene_NavSkillItem> _navSkillItems = new List<UI_GameScene_NavSkillItem>();
    const int MAX_ITEM_COUNT = 30;

    PlayerController _playerCache;
    //Dictionary<SkillBase, GameObject> _navigationSkillDic = new Dictionary<SkillBase, GameObject>();
    List<SkillBase> _usableSkillList = new List<SkillBase>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _playerCache = Managers.Game.Player;
        #region Bind
        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));
        #endregion

        #region Instantiate Pre

        // Wave Cheat Button
        _waveItems.Clear();
        GameObject wavesParent = GetObject((int)GameObjects.WaveList);
        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            UI_GameScene_WaveItem item = Managers.UI.MakeSubItem<UI_GameScene_WaveItem>(wavesParent.transform);
            _waveItems.Add(item);
        }

        // Skill Navigation
        _navSkillItems.Clear();
        GameObject skillsParent = GetObject((int)GameObjects.NavSkillItemList);
        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            UI_GameScene_NavSkillItem item = Managers.UI.MakeSubItem<UI_GameScene_NavSkillItem>(skillsParent.transform);
            _navSkillItems.Add(item);
        }

        #endregion

        RefreshWave();
        RefreshNavi();

        //Event
        _playerCache.PlayerSkills.OnSkillValueChanged -= HandleOnSkillValueChanged;
        _playerCache.PlayerSkills.OnSkillValueChanged += HandleOnSkillValueChanged;

        return true;
    }

    public void SetInfo()
    {
        RefreshWave();
        RefreshNavi();
    }

    void RefreshWave()
    {
        if (_init == false)
            return;

        GameObject parent = GetObject((int)GameObjects.WaveList);
        List<EMonsterWaveType> waves = Managers.Map.StageTransition.CurrentStage.GetWaveTypesAll();

        for (int i = 0; i < _waveItems.Count; i++)
        {
            if (i < waves.Count)
            {
                EMonsterWaveType wave = waves[i];
                _waveItems[i].SetInfo(wave);
                _waveItems[i].gameObject.SetActive(true);
            }
            else
            {
                _waveItems[i].gameObject.SetActive(false);
            }
        }
    }

    void RefreshNavi()
    {
        if (_init == false)
            return;

        if (_usableSkillList.Count < 1)
        {
            foreach (var ui in _navSkillItems)
                ui.gameObject.SetActive(false);

            return;
        }

        for (int i = 0; i < _navSkillItems.Count; i++)
        {
            if (_usableSkillList.Count > i)
            {
                GameObject ui = _navSkillItems[i].gameObject;
                ui.SetActive(true);
                // ui.SetInfo(_usableSkillList[i]); // Skill을 매개로 SetInfo 필요
            }
            else
            {
                _navSkillItems[i].gameObject.SetActive(false);
            }
        }
    }

    void HandleOnSkillValueChanged(List<SkillBase> skillList)
    {
        _usableSkillList.Clear();
        _usableSkillList = skillList;
        RefreshNavi();

        GetText((int)Texts.SkillValueText).text = _playerCache.PlayerSkills.InputTransformer.GetCombinedInputToString();
    }

    // To Do
    // "__Input List<int>__"과 "__PlayerSkillBook 내부 skill List 내부 Skill Input List__" 비교 후, Skill을 반환받게 해야함. 
    // 반환 받은 Skill과 UI GameObject가 매핑되도록 Dictionary를 하나를 파서 UI를 On한다.
}
