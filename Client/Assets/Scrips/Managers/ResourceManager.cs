using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        //Ǯ���� �� ���� ������.
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
        //1. original �̹� ��� ������ �ٷ� ���.


        GameObject Original = Load<GameObject>($"Prefabs/{path}");
        if (Original == null)
        {
            Debug.Log($"Instantiate ����! {path}");
            return null;

        }

        //2.Ȥ�� Ǯ���� �ְ� ������.
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

        //1.Ǯ���� �ʿ��� ������Ʈ���, Ǯ�� �Ŵ������� �ñ��.
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
