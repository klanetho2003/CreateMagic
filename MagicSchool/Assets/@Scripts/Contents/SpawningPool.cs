using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SpawningPool : MonoBehaviour
{
    float _spawnInterval = 0.2f;
    int _maxMonsterCount = 6;
    Coroutine _coUpdateSpawningPool;

    public bool Stopped { get; set; } = false;

    void Start()
    {
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
        Vector3 ranPos = Utils.GenerateMonsterSpawnPosition(Managers.Game.Player.transform.position, 5, 10);
        MonsterController mc = Managers.Object.Spawn<MonsterController>(ranPos, Random.Range(MONSTER_ASSASIN_ID, MONSTER_MAGESKELETON_SHIELD_ID + 1));

        // Value
        mc.Hp = mc.MaxHp;
        mc.CreatureState = Define.CreatureState.Moving;
    }
}
