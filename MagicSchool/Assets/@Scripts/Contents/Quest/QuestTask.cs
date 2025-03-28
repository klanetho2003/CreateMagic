using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class QuestTask
{
    public QuestTaskData _questTaskData;
    public int Count { get; set; }

    public QuestTask(QuestTaskData questTaskData)
    {
        _questTaskData = questTaskData;
    }

    public bool IsCompleted()
    {
        // TODO
        return false;
    }

    public void OnHandleBroadcastEvent(EBroadcastEventType eventType, int value)
    {
        // _questTaskData.ObjectiveType¿Í eventType ºñ±³
    }
}