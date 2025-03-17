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
        _playerCache.OnChangeTotalMpGauge -= HandleOnDecreaseMpGauge;
        _playerCache.OnChangeTotalMpGauge += HandleOnDecreaseMpGauge;
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

    void RefreshMp()
    {
        if (_init == false)
            return;

        foreach (var item in _mpBarItems)
        {
            item.SetInfo(this, 0); // init
            NoneMpBars.Push(item);
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
            FillMpBar.Refresh(_playerCache.CurrentMpGaugeAmount);
    }

    void HandleOnDecreaseMpGauge()
    {
        int sumLoopCount = FullMpBars.Count - _playerCache.Mp;

        // 변화 X
        if (sumLoopCount == 0)
            return;

        // Mp가 감소한 경우
        else if (sumLoopCount > 0)
        {
            for (int i = 0; i < sumLoopCount; i++)
            {
                UI_GameScene_EachMpBar mpBar = FullMpBars.Pop();

                if (FillMpBar != null)
                {
                    mpBar.Slider.value = FillMpBar.Slider.value;
                    FillMpBar.Refresh(0); // reset
                    NoneMpBars.Push(FillMpBar);

                    FillMpBar = mpBar;
                }
                else
                {
                    mpBar.Refresh(0); // reset
                    NoneMpBars.Push(mpBar);
                }
            }
        }
        // Mp가 증가한 경우
        else if (sumLoopCount < 0)
        {
            sumLoopCount = Mathf.Abs(sumLoopCount);

            for (int i = 0; i < sumLoopCount; i++)
            {
                if (NoneMpBars.Count < 1)
                    return;

                UI_GameScene_EachMpBar mpBar = NoneMpBars.Pop();

                if (FillMpBar != null)
                {
                    mpBar.Slider.value = FillMpBar.Slider.value;
                    FillMpBar.Refresh(1); // full
                    FullMpBars.Push(FillMpBar);

                    FillMpBar = mpBar;
                }
                else
                {
                    Debug.LogWarning("Fill Bar None in UI_GameScene");

                    mpBar.Refresh(1); // reset
                    FullMpBars.Push(mpBar);
                }
            }
        }

        
    }

    #endregion


}
