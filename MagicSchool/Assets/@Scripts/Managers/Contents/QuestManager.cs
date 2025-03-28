using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class QuestManager
{
    public Dictionary<int, Quest> AllQuests = new Dictionary<int, Quest>();

    // To Do 상태에 따라 분리해서 이중으로 List 관리

    public void Init()
    {
        Managers.Game.OnBroadcastEvent -= OnHandleBroadcastEvent;
        Managers.Game.OnBroadcastEvent += OnHandleBroadcastEvent;
    }

    public void AddUnknownQuests()
    {
        foreach (QuestData questData in Managers.Data.QuestDic.Values.ToList())
        {
            if (AllQuests.ContainsKey(questData.DataId))
                continue;

            QuestSaveData questSaveData = new QuestSaveData()
            {
                TemplateId = questData.DataId,
                State = Define.EQuestState.None,
                NextResetTime = DateTime.MaxValue,
            };

            for (int i = 0; i < questData.QuestTasks.Count; i++)
                questSaveData.ProgressCount.Add(0);

            AddQuest(questSaveData);
        }
    }

    // Quest 등장 조건 Check
    public void CheckWaitingQuests()
    {
        // TODO
    }

    public void CheckProcessingQuests()
    {
        // TODO
    }

    public Quest AddQuest(QuestSaveData questInfo)
    {
        Quest quest = Quest.MakeQuest(questInfo);
        if (quest == null)
            return null;

        AllQuests.Add(quest.TemplateId, quest);

        // To Do switch case 이용해서, 이중으로 관리하는 List에 넣어주기

        return quest;
    }

    public void Clear()
    {
        AllQuests.Clear();

        // To Do 이중으로 관리하는 List들 Clear
    }

    void OnHandleBroadcastEvent(EBroadcastEventType eventType, int value)
    {
        foreach (Quest quest in AllQuests.Values)
        {
            if (quest.State == EQuestState.Processing)
                quest.OnHandleBroadcastEvent(eventType, value);
        }
    }
}
