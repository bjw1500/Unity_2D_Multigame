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


    //코드 분석하기.
    public static T FindChild<T>(GameObject go, string name = null, bool rescusive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;
        //where T : UnityEngine.Object를 안해주면 NUll 리턴이 안 된다.

        if (rescusive == false)
        {
            //직속 자식만 찾는 경우.
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
            //자식의 자식까지 전부 깊이 우선 탐색으로 찾 음.


            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                //name == enum 안에 있는 이름들.
                {
                    return component;
                }
            }
        }

        return null;
    }
  
}




