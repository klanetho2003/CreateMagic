using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Quest
{
    public QuestSaveData SaveData { get; set; }
    public QuestData QuestData { get; private set; }
    public List<QuestTask> _questTasks = new List<QuestTask>();

    public int TemplateId
    {
        get { return SaveData.TemplateId; }
        set { SaveData.TemplateId = value; }
    }

    public EQuestState State
    {
        get { return SaveData.State; }
        set { SaveData.State = value; }
    }

    public QuestTask GetCurrentTask()
    {
        foreach (QuestTask task in _questTasks)
        {
            if (task.IsCompleted() == false)
                return task;
        }

        return null;
    }

    public bool IsCompleted()
    {
        for (int i = 0; i < QuestData.QuestTasks.Count; i++)
        {
            if (i >= SaveData.ProgressCount.Count)
                return false;

            QuestTaskData questTaskData = QuestData.QuestTasks[i];

            int progressCount = SaveData.ProgressCount[i];
            if (progressCount < questTaskData.ObjectiveCount)
                return false;
        }

        return true;
    }

    public Quest(QuestSaveData saveData)
    {
        SaveData = saveData;
        State = EQuestState.None;
        QuestData = Managers.Data.QuestDic[TemplateId];

        _questTasks.Clear();

        for (int i = 0; i < QuestData.QuestTasks.Count; i++)
        {
            _questTasks.Add(new QuestTask(QuestData.QuestTasks[i], saveData.ProgressCount[i]));
        }
    }

    public void GiveReward()
    {
        if (SaveData.State == EQuestState.Rewarded)
            return;

        if (IsCompleted() == false)
            return;

        SaveData.State = EQuestState.Rewarded;

        foreach (var reward in QuestData.Rewards)
        {
            switch (reward.RewardType)
            {
                case EQuestRewardType.Gold:
                    Managers.Game.EarnResource(EResourceType.Gold, reward.RewardCount);
                    break;
                case EQuestRewardType.Skill:
                    int skillId = reward.RewardDataId;
                    Managers.Game.Player.PlayerSkills.AddSkill(skillId, Define.ESkillSlot.CastingSkill);
                    break;
                case EQuestRewardType.Meat:
                    Managers.Game.EarnResource(EResourceType.Meat, reward.RewardCount);
                    break;
                case EQuestRewardType.Mineral:
                    Managers.Game.EarnResource(EResourceType.Mineral, reward.RewardCount);
                    break;
                case EQuestRewardType.Wood:
                    Managers.Game.EarnResource(EResourceType.Wood, reward.RewardCount);
                    break;
                case EQuestRewardType.Item:
                    break;
            }
        }
    }

    public static Quest MakeQuest(QuestSaveData saveData)
    {
        if (Managers.Data.QuestDic.TryGetValue(saveData.TemplateId, out QuestData questData) == false)
            return null;

        Quest quest = new Quest(saveData);
        return quest;
    }

    public void OnHandleBroadcastEvent(EBroadcastEventType eventType, int value)
    {
        // 무한 루프 방지
        if (eventType == EBroadcastEventType.QuestClear)
            return;

        GetCurrentTask().OnHandleBroadcastEvent(eventType, value);

        for (int i = 0; i < _questTasks.Count; i++)
        {
            // To Do Save를 진행하는 타이밍 정의 필요
            SaveData.ProgressCount[i] = _questTasks[i].Count;
        }

        if (IsCompleted() && State != EQuestState.Rewarded)
        {
            State = EQuestState.Completed;
            GiveReward(); // Rewarded State
            Managers.Game.BroadcastEvent(EBroadcastEventType.QuestClear, QuestData.DataId);
        }
    }
}

