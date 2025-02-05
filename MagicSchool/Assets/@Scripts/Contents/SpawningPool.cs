using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SpawningPool : MonoBehaviour
{
    float _spawnInterval = 0.2f;
    int _maxMonsterCount = 2;
    Coroutine _coUpdateSpawningPool;

    public bool Stopped { get; set; } = false;

    void Start()
    {
        // TEMP : DataID ?
        /*Vector3 ranPos = Utils.GenerateMonsterSpawnPosition(Managers.Game.Player.transform.position, 5, 10);
        MonsterController mc = Managers.Object.Spawn<MonsterController>(ranPos, Random.Range(MONSTER_SKELETON_ID, MONSTER_SKELETON_ID + 1));

        mc.Hp = mc.MaxHp;
        mc.CreatureState = Define.CreatureState.Moving;*/
        _coUpdateSpawningPool = StartCoroutine(CoUpdateSpawningPool());
    }

    IEnumerator CoUpdateSpawningPool()
    {
        while (true)
        {
            TrySpawn();
            yield return new WaitForSeconds(_spawnInterval);
        }
        
    }

    void TrySpawn()
    {
        if (Stopped)
            return;

        int monsterCount = Managers.Object.Monsters.Count;
        if (monsterCount >= _maxMonsterCount)
            return;

        // TEMP : DataID ?
        //Vector3 ranPos = Utils.GenerateMonsterSpawnPosition(Managers.Game.Player.transform.position, 5, 10);
        Vector3Int randCellPos = new Vector3Int(0 + Random.Range(-3,3), 0 + Random.Range(-3, 3), 0);
        if (Managers.Map.CanGo(randCellPos) == false)
            return;

        MonsterController mc = Managers.Object.Spawn<MonsterController>(Vector3.zero, Random.Range(MONSTER_ASSASIN_ID, MONSTER_ASSASIN_ID + 1));
        mc.SetCellPos(randCellPos, true);

        // Value
        mc.Hp = mc.MaxHp;
        mc.CreatureState = Define.CreatureState.Moving;
    }
}
