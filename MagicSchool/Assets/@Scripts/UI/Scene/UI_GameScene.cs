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
    enum Sliders
    {
        MpBar,
    }

    List<UI_GameScene_WaveItem> _waveItems = new List<UI_GameScene_WaveItem>();
    List<UI_GameScene_NavSkillItem> _navSkillItems = new List<UI_GameScene_NavSkillItem>();
    const int MAX_ITEM_COUNT = 30;

    PlayerController _playerCache;
    //Dictionary<SkillBase, GameObject> _navigationSkillDic = new Dictionary<SkillBase, GameObject>();
    List<SkillBase> _cachePlayerActivateSkills { get; set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _playerCache = Managers.Game.Player;
        _cachePlayerActivateSkills = Managers.Game.Player.PlayerSkills.ActivateSkills;
        #region Bind
        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
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
        _playerCache.OnMpGageChange -= HandleOnMpGageChange;
        _playerCache.OnMpGageChange += HandleOnMpGageChange;

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

    #region Navi

    void RefreshNavi()
    {
        if (_init == false)
            return;

        if (_cachePlayerActivateSkills.Count < 1)
        {
            foreach (var ui in _navSkillItems)
                ui.gameObject.SetActive(false);

            return;
        }

        for (int i = 0; i < _navSkillItems.Count; i++)
        {
            if (_cachePlayerActivateSkills.Count > i)
            {
                UI_GameScene_NavSkillItem ui = _navSkillItems[i];
                ui.gameObject.SetActive(true);
                ui.SetInfo(_cachePlayerActivateSkills[i]); // Skill을 매개로 SetInfo 필요
            }
            else
            {
                _navSkillItems[i].gameObject.SetActive(false);
            }
        }
    }

    void HandleOnSkillValueChanged(List<SkillBase> skillList)
    {
        RefreshNavi();

        GetText((int)Texts.SkillValueText).text = _playerCache.PlayerSkills.InputTransformer.GetCombinedInputToString();
    }

    #endregion

    #region Mp

    void HandleOnMpGageChange(float currentGageAmout, float oneGaugeAmount)
    {
        // Count를 곱하자
        float sum = currentGageAmout / oneGaugeAmount;
        GetSliders((int)Sliders.MpBar).value = (sum + _playerCache.Mp * oneGaugeAmount) / _playerCache.MaxMp.Value;
    }
    // PlayerMP(Queue Count)에 맞춰서 UI 보여주기

    #endregion


}
