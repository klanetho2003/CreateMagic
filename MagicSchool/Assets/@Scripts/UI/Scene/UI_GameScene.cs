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
        MpBar,
    }
    enum Texts
    {
        SkillValueText,
    }

    List<UI_GameScene_WaveItem> _waveItems = new List<UI_GameScene_WaveItem>();
    List<UI_GameScene_NavSkillItem> _navSkillItems = new List<UI_GameScene_NavSkillItem>();
    const int MAX_ITEM_COUNT = 30;

    PlayerController _playerCache;
    List<SkillBase> _cachePlayerActivateSkills { get; set; }

    List<UI_GameScene_EachMpBar> _mpBarItems = new List<UI_GameScene_EachMpBar>();
    public UI_GameScene_EachMpBar FillMpBar = null;

    public Stack<UI_GameScene_EachMpBar> FullMpBars { get; } = new Stack<UI_GameScene_EachMpBar>();
    public Stack<UI_GameScene_EachMpBar> NoneMpBars { get; } = new Stack<UI_GameScene_EachMpBar>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _playerCache = Managers.Game.Player;
        _cachePlayerActivateSkills = Managers.Game.Player.PlayerSkills.ActivateSkills;

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

        // Mp Bar
        _mpBarItems.Clear();
        GameObject _mpBarsParent = GetObject((int)GameObjects.MpBar);
        for (int i = 0; i < _playerCache.MaxMp.Value; i++)
        {
            UI_GameScene_EachMpBar item = Managers.UI.MakeSubItem<UI_GameScene_EachMpBar>(_mpBarsParent.transform);
            _mpBarItems.Add(item);
        }

        #endregion

        #region Refresh
        RefreshWave();
        RefreshNavi();
        RefreshMp();
        #endregion

        #region Event
        _playerCache.PlayerSkills.OnSkillValueChanged -= HandleOnSkillValueChanged;
        _playerCache.PlayerSkills.OnSkillValueChanged += HandleOnSkillValueChanged;

        _playerCache.OnMpGaugeUpStart -= HandleOnMpGaugeUpStart;
        _playerCache.OnMpGaugeUpStart += HandleOnMpGaugeUpStart;
        _playerCache.OnMpGaugeFill -= HandleOnMpGaugeUp;
        _playerCache.OnMpGaugeFill += HandleOnMpGaugeUp;
        _playerCache.OnDecreaseMpGauge -= HandleOnDecreaseMpGauge;
        _playerCache.OnDecreaseMpGauge += HandleOnDecreaseMpGauge;
        #endregion

        return true;
    }

    public void SetInfo()
    {
        RefreshWave();
        RefreshNavi();
        RefreshMp();
    }

    #region Wave
    
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

    #endregion 

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
                ui.SetInfo(_cachePlayerActivateSkills[i]); // Skill�� �Ű��� SetInfo �ʿ�
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

    void RefreshMp()
    {
        if (_init == false)
            return;

        foreach (var item in _mpBarItems)
        {
            item.SetInfo(this);
            NoneMpBars.Push(item);
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < _playerCache.MaxMp.Value; i++)
        {
            _mpBarItems[i].gameObject.SetActive(true);
        }
    }

    void HandleOnMpGaugeUpStart()
    {
        if (FillMpBar != null)
        {
            if (NoneMpBars.Count < 1)
            {
                FullMpBars.Push(FillMpBar);
                FillMpBar = null;
                return;
            }

            FullMpBars.Push(FillMpBar);
        }

        UI_GameScene_EachMpBar mpBar = NoneMpBars.Pop();
        FillMpBar = mpBar;
    }

    void HandleOnMpGaugeUp()
    {
        if (FillMpBar != null)
            FillMpBar.RefreshSlider();
    }

    void HandleOnDecreaseMpGauge()
    {
        while (_playerCache.Mp < FullMpBars.Count)
        {
            UI_GameScene_EachMpBar mpBar = FullMpBars.Pop();

            if (FillMpBar != null)
            {
                mpBar.Slider.value = FillMpBar.Slider.value;
                FillMpBar.ResetValue();
                NoneMpBars.Push(FillMpBar);

                FillMpBar = mpBar;
            }
            else
            {
                mpBar.ResetValue();
                NoneMpBars.Push(mpBar);
            }
        }
    }

    #endregion


}
