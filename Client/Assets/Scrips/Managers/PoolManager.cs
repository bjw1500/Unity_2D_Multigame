using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{

    #region pool
    class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        Stack<Poolable> _poolStack = new Stack<Poolable>();

        public void Init(GameObject orignal, int count = 5)
        {
            Original = orignal;
            Root = new GameObject().transform;
            Root.name = $"{orignal.name}_Root";

            //풀링할 오리지널을 준비한다.
            //풀링할 오브젝트들을 모아둘 Root를 하나 만들어준다.
            //Create로 생성한 후에, Root에 밀어 넣어준다.

//            기본적으로 유니티에서 Component는 독단적으로 존재하는게 아니라
//GameObject에 기생(?)해서 살아갑니다.

//따라서 GameObject를 넘겨주나 특정 Component를 넘겨주나 별 차이 없습니다.
//GameObject를 넘겨받을 경우 go.GetComponent<T> 로 Component를 찾아줄 수 있고
//반대의 경우에도 gameObject를 이용해 Component가 붙어있는 오브젝트를 찾아줄 수 있습니다.
//transform은 GameObject가 들고 있는 것이고,
//모든 Component에서는 transform을 통해 Transform에 접근할 수 있습니다.

//따라서 poolable.transform~을 하는 순간
//[poolable Component가 붙어있는 GameObject의 transform]의 의미와 동일합니다.


            for (int i = 0; i< count; i++)
            {

                Push(Create());
            }


        }

        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;
            return go.GetOrAddComponent<Poolable>();

        }

        public void Push(Poolable poolable)
        {
            if (poolable == null)
                return;

            poolable.transform.parent = Root;
            poolable.gameObject.SetActive(false);
            poolable.IsUsing = false;

            _poolStack.Push(poolable);

        }

        public Poolable Pop(Transform parent)
        {
            //꺼낼 때 부모는 누구로 할 것인가?
            //Null로 하면 그냥 맨몸으로 하이라키에 표시됨.
            Poolable poolable;

            if(_poolStack.Count > 0)
            {
                poolable = _poolStack.Pop();
                //pool.Init()에서 Dont destroy에 한 번 들어갔기에, 활성화 되어도 Dont destroy밑에서 됨.

            }
            else
            {
                poolable = Create();
                //생성 후 바로 활성화 되어서 Don't Destroy에 들어가지 않음.

            }

            poolable.gameObject.SetActive(true);



            //DonEstroyOnLoad 해제 용도
            if (parent == null)
               poolable.transform.parent = Managers.SceneManager.CurrentScene.transform;

            poolable.transform.parent = parent;
            //부모가 null 이기에 그 어떤 부모도 두지 않고 맨몸으로 나옴.

            poolable.IsUsing = true;

            return poolable;


        }
    }

    #endregion

    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();

    Transform _root;

    public void Init()
    {
       if(_root ==null)
        {
            _root = new GameObject { name = "@PoolManager" }.transform;
            Object.DontDestroyOnLoad(_root);
        }

    }

    public void CreatePool(GameObject original, int count = 5)
    {
        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.parent = _root.transform;

        _pool.Add(original.name, pool);

        //PoolManager 산하에 pool_Root가 생겨난다.


    }


    public void Push(Poolable poolable)
    {
        //반환. 
        string name = poolable.gameObject.name;

        if(_pool.ContainsKey(name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }

        _pool[name].Push(poolable);

        //만들어진 풀에 다시 집어 넣는다.

    }

    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if (_pool.ContainsKey(original.name) == false)
        {
            //pool에 없으면 
            CreatePool(original);


        }
        
        return _pool[original.name].Pop(parent);
    }

    public GameObject GetOriginal(string name)
    {
        //리소스 매니저에 낑겨 넣을 함수.
        if (_pool.ContainsKey(name) == false)
            return null;


        return _pool[name].Original;
    }

    public void Clear()
    {
        //풀링을 날리는 경우는 많이 없지만, 너무 사용되지 않을 떄. 
        //맵 이동할때?

        foreach (Transform child in _root)
            GameObject.Destroy(child.gameObject);

        //풀들을 파괴한다.

        _pool.Clear();

    }

}
