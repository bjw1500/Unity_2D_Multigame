using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{

    public static Managers _instance;

    public static Managers Instance
    {
        get
        {
            Init();
            return _instance;
        }

    }

    private NetworkManager _networkManager = new NetworkManager();
    private MapManager _map = new MapManager();
    private ObjectManager _obj = new ObjectManager();

    public static NetworkManager NetWork { get { return Instance._networkManager; } }
    public static MapManager Map { get { return Instance._map; } }

    public static ObjectManager Object { get { return Instance._obj;}}

    
   
    private ResourceManager _resourceManager = new ResourceManager();
    private SceneManagerEX _sceneManager = new SceneManagerEX();
    private PoolManager _poolManager = new PoolManager();
    private DataManager _dataManager = new DataManager();
    private GameManager _gameManager = new GameManager();

    
  


    public static ResourceManager Resource
    {
        get
        {
            return Instance._resourceManager;
        }

    }

    public static SceneManagerEX SceneManager
    {
        get
        {
            return Instance._sceneManager;
        }

    }

    public static PoolManager PoolManager
    {
        get{return Instance._poolManager;}

    }

    public static DataManager DataManager { get { return Instance._dataManager; } }

    public static GameManager GameManager { get { return Instance._gameManager; }}


    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        _networkManager.Update();

    }

    static void Init()
    {

        if (_instance == null)
        {

            GameObject go = GameObject.Find("@Manager");
            if (go == null)
            {
                go = new GameObject("@Manager");
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);

            _instance = go.GetComponent<Managers>();
            _instance._poolManager.Init();
            _instance._dataManager.Init();
            _instance._gameManager.Init();
            _instance._networkManager.Init();
        }



    }

    public static void Clear()
    {

        PoolManager.Clear();

    }



}
