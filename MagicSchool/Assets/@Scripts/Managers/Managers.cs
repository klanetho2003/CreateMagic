using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static bool Initialized { get; private set; } = false;

    private static Managers s_instance;
    private static Managers Instance { get { Init(); return s_instance; } }


    #region Contents
    GameManager _game = new GameManager();
    InputManager _input = new InputManager();
    InventoryManager _inventory = new InventoryManager();
    MapManager _map = new MapManager();
    ObjectManager _object = new ObjectManager();
    PoolManager _pool = new PoolManager();
    QuestManager _quest = new QuestManager();
    public static GameManager Game { get { return Instance?._game; } }
    public static InputManager Input { get { return Instance?._input; } }
    public static InventoryManager Inventory { get { return Instance?._inventory; } }
    public static MapManager Map { get { return Instance?._map; } }
    public static ObjectManager Object { get { return Instance?._object; } }
    public static PoolManager Pool { get { return Instance?._pool; } }
    public static QuestManager Quest { get { return Instance?._quest; } }
    #endregion

    #region Core
    DataManager _data = new DataManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();
    public static DataManager Data { get { return Instance?._data; } }
    public static ResourceManager Resource { get { return Instance?._resource; } }
    public static SceneManagerEx Scene { get { return Instance?._scene; } }
    public static SoundManager Sound { get { return Instance?._sound; } }
    public static UIManager UI { get { return Instance?._ui; } }
    #endregion

    #region Language

    private static Define.ELanguage _language = Define.ELanguage.Korean;
    public static Define.ELanguage Language
    {
        get { return _language; }
        set
        {
            _language = value;

            // runTime �� ������ �ʿ��� �� CallBack �Լ� �߰�
        }
    }

    public static string GetText(string textId)
    {
        switch (_language)
        {
            case Define.ELanguage.Korean:
                return Managers.Data.TextDic[textId].KOR;
            case Define.ELanguage.English:
                break;
            case Define.ELanguage.Japanese:
                break;
        }

        return "";
    }

    #endregion

    private void Start()
    {
        Init();
    }

    static void Init()
    {
        if (s_instance == null && Initialized == false)
        {
            Initialized = true;

            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);

            // �ʱ�ȭ
            s_instance = go.GetComponent<Managers>();

            s_instance._quest.Init();
        }
    }

    private void Update()
    {
        s_instance._input.OnUpdate();
    }
}
