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

            //Ǯ���� ���������� �غ��Ѵ�.
            //Ǯ���� ������Ʈ���� ��Ƶ� Root�� �ϳ� ������ش�.
            //Create�� ������ �Ŀ�, Root�� �о� �־��ش�.

//            �⺻������ ����Ƽ���� Component�� ���������� �����ϴ°� �ƴ϶�
//GameObject�� ���(?)�ؼ� ��ư��ϴ�.

//���� GameObject�� �Ѱ��ֳ� Ư�� Component�� �Ѱ��ֳ� �� ���� �����ϴ�.
//GameObject�� �Ѱܹ��� ��� go.GetComponent<T> �� Component�� ã���� �� �ְ�
//�ݴ��� ��쿡�� gameObject�� �̿��� Component�� �پ��ִ� ������Ʈ�� ã���� �� �ֽ��ϴ�.
//transform�� GameObject�� ��� �ִ� ���̰�,
//��� Component������ transform�� ���� Transform�� ������ �� �ֽ��ϴ�.

//���� poolable.transform~�� �ϴ� ����
//[poolable Component�� �پ��ִ� GameObject�� transform]�� �ǹ̿� �����մϴ�.


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
            //���� �� �θ�� ������ �� ���ΰ�?
            //Null�� �ϸ� �׳� �Ǹ����� ���̶�Ű�� ǥ�õ�.
            Poolable poolable;

            if(_poolStack.Count > 0)
            {
                poolable = _poolStack.Pop();
                //pool.Init()���� Dont destroy�� �� �� ���⿡, Ȱ��ȭ �Ǿ Dont destroy�ؿ��� ��.

            }
            else
            {
                poolable = Create();
                //���� �� �ٷ� Ȱ��ȭ �Ǿ Don't Destroy�� ���� ����.

            }

            poolable.gameObject.SetActive(true);



            //DonEstroyOnLoad ���� �뵵
            if (parent == null)
               poolable.transform.parent = Managers.SceneManager.CurrentScene.transform;

            poolable.transform.parent = parent;
            //�θ� null �̱⿡ �� � �θ� ���� �ʰ� �Ǹ����� ����.

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

        //PoolManager ���Ͽ� pool_Root�� ���ܳ���.


    }


    public void Push(Poolable poolable)
    {
        //��ȯ. 
        string name = poolable.gameObject.name;

        if(_pool.ContainsKey(name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }

        _pool[name].Push(poolable);

        //������� Ǯ�� �ٽ� ���� �ִ´�.

    }

    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if (_pool.ContainsKey(original.name) == false)
        {
            //pool�� ������ 
            CreatePool(original);


        }
        
        return _pool[original.name].Pop(parent);
    }

    public GameObject GetOriginal(string name)
    {
        //���ҽ� �Ŵ����� ���� ���� �Լ�.
        if (_pool.ContainsKey(name) == false)
            return null;


        return _pool[name].Original;
    }

    public void Clear()
    {
        //Ǯ���� ������ ���� ���� ������, �ʹ� ������ ���� ��. 
        //�� �̵��Ҷ�?

        foreach (Transform child in _root)
            GameObject.Destroy(child.gameObject);

        //Ǯ���� �ı��Ѵ�.

        _pool.Clear();

    }

}
