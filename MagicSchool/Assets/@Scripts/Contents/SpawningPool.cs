using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningPool : MonoBehaviour
{
    float _spawnInterval = 0.2f;
    int _maxMonsterCount = 100;
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
        Vector3 ranPos = Utils.GenerateMonsterSpawnPosition(Managers.Game.Player.transform.position, 10, 15);
        MonsterController mc = Managers.Object.Spawn<MonsterController>(ranPos, 1 + Random.Range(0, 2));
        mc.Hp = mc.MaxHp;
    }
}
