using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T evt = go.GetComponent<T>();
        if(evt == null)
        {
            go.AddComponent<T>();
            evt = go.GetComponent<T>();

        }
        return evt;

    }

    public static GameObject FindChild(GameObject go, string name = null, bool recusive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recusive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }


    //�ڵ� �м��ϱ�.
    public static T FindChild<T>(GameObject go, string name = null, bool rescusive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;
        //where T : UnityEngine.Object�� �����ָ� NUll ������ �� �ȴ�.

        if (rescusive == false)
        {
            //���� �ڽĸ� ã�� ���.
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }

            }


        }
        else
        {
            //�ڽ��� �ڽı��� ���� ���� �켱 Ž������ ã ��.


            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                //name == enum �ȿ� �ִ� �̸���.
                {
                    return component;
                }
            }
        }

        return null;
    }
  
}




