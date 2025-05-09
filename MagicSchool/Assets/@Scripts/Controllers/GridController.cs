using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CellEX
{
    public HashSet<GameObject> Objects { get; } = new HashSet<GameObject>();
}

public class GridController : BaseController
{
    UnityEngine.Grid _grid;

    Dictionary<Vector3Int, CellEX> _cells = new Dictionary<Vector3Int, CellEX>();

    public override bool Init()
    {
        base.Init();

        _grid = gameObject.GetOrAddComponent<UnityEngine.Grid>();

        return true;
    }

    public void Add(GameObject go)
    {
        Vector3Int cellPos = _grid.WorldToCell(go.transform.position);

        CellEX cell = GetCell(cellPos);
        if (cell == null)
            return;

        cell.Objects.Add(go);
    }

    public void Remove(GameObject go)
    {
        Vector3Int cellPos = _grid.WorldToCell(go.transform.position);

        CellEX cell = GetCell(cellPos);
        if (cell == null)
            return;

        cell.Objects.Remove(go);
    }

    CellEX GetCell(Vector3Int cellPos)
    {
        CellEX cell = null;

        if (_cells.TryGetValue(cellPos, out cell) == false)
        {
            cell = new CellEX();
            _cells.Add(cellPos, cell);
        }

        return cell;   
    }

    public List<GameObject> GatherObjects(Vector3 pos, float range)
    {
        List<GameObject> objects = new List<GameObject>();

        Vector3Int left = _grid.WorldToCell(pos + new Vector3(-range, 0));
        Vector3Int right = _grid.WorldToCell(pos + new Vector3(+range, 0));
        Vector3Int bottom = _grid.WorldToCell(pos + new Vector3(0, -range));
        Vector3Int top = _grid.WorldToCell(pos + new Vector3(0, +range));

        int minX = left.x;
        int maxX = right.x;
        int minY = bottom.y;
        int maxY = top.y;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (_cells.ContainsKey(new Vector3Int(x, y, 0)) == false)
                    continue;

                objects.AddRange(_cells[new Vector3Int(x, y, 0)].Objects);
            }
        }

        return objects;
    }
}
