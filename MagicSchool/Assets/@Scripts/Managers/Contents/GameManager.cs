using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;

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

            if (value > 0) // 0일 때는 Wave가 전환될 때
                OnKillCountChanged?.Invoke(value);
        }
    }
    #endregion

    #region Teleport
    public void TeleportPlayer(Vector3 position)
    {
        TeleportPlayer(Managers.Map.World2Cell(position));
    }

    public void TeleportPlayer(Vector3Int cellPos)
    {
        Vector3Int randCellPos = Managers.Game.GetNearbyPosition(Player, cellPos);

        Player.Cam.enabled = false;
        Managers.Map.MoveTo(Player, randCellPos, forceMove: true);
        Player.Cam.enabled = true;


        // Pet이 있을 경우 아래 코드로 변환
        /*foreach (var hero in Managers.Object.Heroes)
        {
            Vector3Int randCellPos = Managers.Game.GetNearbyPosition(hero, cellPos);
            Managers.Map.MoveTo(hero, randCellPos, forceMove: true);
        }*/

        /*Vector3 worldPos = Managers.Map.Cell2World(cellPos);
        Managers.Object.Camp.ForceMove(worldPos);
        Camera.main.transform.position = worldPos;*/
    }
    #endregion

    #region Helper
    public Vector3Int GetNearbyPosition(BaseController student, Vector3Int pivot, int range = 5)
    {
        int x = Random.Range(-range, range);
        int y = Random.Range(-range, range);

        for (int i = 0; i < 100; i++)
        {
            Vector3Int randCellPos = pivot + new Vector3Int(x, y, 0);
            if (Managers.Map.CanGo(student, randCellPos))
                return randCellPos;
        }

        Debug.LogError($"GetNearbyPosition Failed");

        return Vector3Int.zero;
    }
    #endregion
}
