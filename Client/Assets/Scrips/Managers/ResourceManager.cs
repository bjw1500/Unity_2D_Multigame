using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        //풀링할 때 래핑 해주자.
        if(typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index > 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.PoolManager.GetOriginal(name);
            if (go != null)
                return go as T;

        }
        //


        return Resources.Load<T>(path);

    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        //1. original 이미 들고 있으면 바로 사용.


        GameObject Original = Load<GameObject>($"Prefabs/{path}");
        if (Original == null)
        {
            Debug.Log($"Instantiate 실패! {path}");
            return null;

        }

        //2.혹시 풀링된 애가 있을까.
        if (Original.GetComponent<Poolable>() != null)
        {
            return Managers.PoolManager.Pop(Original, parent).gameObject;
        }



        GameObject go = GameObject.Instantiate(Original, parent);
        go.name = Original.name;

        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        //1.풀링이 필요한 오브젝트라면, 풀링 매니저한테 맡긴다.
        Poolable poolable = go.GetComponent<Poolable>();
        if(poolable != null)
        {
            Managers.PoolManager.Push(poolable);
            return;
        }

        Debug.Log($"Destroy{go.name}");
        Object.Destroy(go);
    }


}
