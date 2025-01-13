using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public PlayerController Player { get { return Managers.Object?.Player; } }

    List<Transform> _wayPoints = new List<Transform>();
    public List<Transform> WayPoints { get { return _wayPoints; }}

    #region 재화
    public int Gold { get; set; }

    public Action<int> OnJamCountChanged;

    private int _jam;
    public int Jam
    {
        get { return _jam; }
        set
        {
            _jam = value;
            OnJamCountChanged?.Invoke(value);
        }
    }
    #endregion

    #region 이동
    Vector2 _moveDir;

    //delegate void ExFunc(int a, int b);
    public event Action<Vector2> OnMoveDirChanged;

    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set
        {
            _moveDir = value;
            OnMoveDirChanged?.Invoke(_moveDir);
        }
    }
    #endregion

    #region 전투
    int _killCount;
    public event Action<int> OnKillCountChanged;

    public int KillCount
    {
        get { return _killCount; }
        set
        {
            _killCount = value;
            OnKillCountChanged?.Invoke(value);
        }
    }
    #endregion
}
