using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_GameScene : UI_Scene
{
    enum GameObjects
    {
        WaveList,
        NavSkillItemList,
        MpBar,
    }

    enum Buttons
    {
        ItemsListButton,
    }
    enum Texts
    {
        SkillValueText,
        FPSViewerText,
    }

    List<UI_GameScene_WaveItem> _waveItems = new List<UI_GameScene_WaveItem>();
    List<UI_GameScene_NavSkillItem> _navSkillItems = new List<UI_GameScene_NavSkillItem>();
    const int MAX_ITEM_COUNT = 30;

    PlayerController _playerCache;
    List<SkillBase> _cachePlayerActivateSkills { get; set; }

    TMP_Text _cacheFPSViewerText;

    List<UI_GameScene_EachMpBar> _mpBarItems = new List<UI_GameScene_EachMpBar>();
    public UI_GameScene_EachMpBar FillMpBar { get; private set; }

    public Stack<UI_GameScene_EachMpBar> FullMpBars { get; } = new Stack<UI_GameScene_EachMpBar>();
    public Stack<UI_GameScene_EachMpBar> NoneMpBars { get; } = new Stack<UI_GameScene_EachMpBar>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        #region Bind
        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        #endregion

        #region Event Binding
        GetButton((int)Buttons.ItemsListButton).gameObject.BindEvent(OnClickItemsListButton);
        #endregion

        #region Cache
        _playerCache = Managers.Game.Player;
        _cachePlayerActivateSkills = Managers.Game.Player.PlayerSkills.ActivateSkills;
        _cacheFPSViewerText = GetText((int)Texts.FPSViewerText);
        #endregion

        #region Instantiate Pre & Cache

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

    private float _elapsedTime = 0.0f;
    private float _updateInterval = 1.0f;

    private void Update()
    {
        #region FPS Viewer
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _updateInterval)
        {
            float fps = 1.0f / Time.deltaTime;
            float ms = Time.deltaTime * 1000.0f;
            string text = string.Format("{0:N1} FPS ({1:N1}ms)", fps, ms);
            _cacheFPSViewerText.text = text;

            _elapsedTime = 0;
        }
        #endregion
    }

    void OnClickItemsListButton(PointerEventData evt)
    {
        Debug.Log("OnClickItemsListButton");
        UI_ItemsListPopup popup = Managers.UI.ShowPopupUI<UI_ItemsListPopup>();
        popup.SetInfo();
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

    #region Navi_삭제 예정
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
    #endregion

    #region Navi
    void HandleOnSkillValueChanged()
    {
        // RefreshNavi();

        GetText((int)Texts.SkillValueText).text = _playerCache.PlayerSkills.InputMemorizer.GetCombinedInputToString();
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

    // FullBars.Count + NoneBars.Count + FillBar 개수 == MaxMp
    // Max 9 + 0 + 1    ,   6 + 3 + 1   ,  0 + 9 + 1  ,   First 0 + 10 + 0
    void HandleOnMpGaugeUpStart()
    {
        if (FillMpBar != null && FullMpBars.Count + 1 < _playerCache.MaxMp.Value)
            FullMpBars.Push(FillMpBar);

        if (NoneMpBars.Count < 1) // null check
            return;
        UI_GameScene_EachMpBar mpBar = NoneMpBars.Pop();
        FillMpBar = mpBar;
    }

    void HandleOnMpGaugeUp(float currentMpGaugeAmount)
    {
        if (FillMpBar != null)
            FillMpBar.Refresh(currentMpGaugeAmount);
    }

    void HandleOnDecreaseMpGauge()
    {
        if (FillMpBar == null)
        {
            Debug.LogWarning("FillBar None in UI_GameScene");
            return;
        }

        int sumLoopCount = FullMpBars.Count - _playerCache.Mp;

        // "mp가 가득 차있는 상황일 때 casting을 한 경우
        if (sumLoopCount == 0)
        {
            FillMpBar.Refresh(0); // full
        }

        // Mp가 감소한 경우
        else if (sumLoopCount > 0)
        {
            for (int i = 0; i < sumLoopCount; i++)
            {
                UI_GameScene_EachMpBar mpBar = FullMpBars.Pop();
                mpBar.Slider.value = FillMpBar.Slider.value;
                FillMpBar.Refresh(0); // reset
                NoneMpBars.Push(FillMpBar);

                FillMpBar = mpBar;
            }
        }

        // Mp가 증가한 경우
        else if (sumLoopCount < 0)
        {
            sumLoopCount = Mathf.Abs(sumLoopCount);

            for (int i = 0; i < sumLoopCount; i++)
            {
                if (FullMpBars.Count + 1 >= (int)_playerCache.MaxMp.Value)
                {
                    FillMpBar.Refresh(1); // full
                    return;
                }

                UI_GameScene_EachMpBar mpBar = NoneMpBars.Pop();
                mpBar.Slider.value = FillMpBar.Slider.value;
                FillMpBar.Refresh(1); // full
                FullMpBars.Push(FillMpBar);

                FillMpBar = mpBar;
            }
        }
    }

    #endregion
}
