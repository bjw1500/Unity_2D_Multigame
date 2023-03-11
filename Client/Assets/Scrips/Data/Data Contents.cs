using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Json ���Ͽ��� �Ľ��� ������ ����ü�� �����ϴ� ��.



[Serializable]
public class StatData : ILoader<string, StatInfo>
{
    public List<StatInfo> stats = new List<StatInfo>();

    public Dictionary<string, StatInfo> MakeDict()
    {
        Dictionary<string, StatInfo> dict = new Dictionary<string, StatInfo>();
        foreach (StatInfo stat in stats)
            dict.Add(stat.Name, stat);
        return dict;
    }
}


[Serializable]
public class Skill
{
    public int id;
    public string name;
    public float cooldown;
    public int damage;
    public AttackType attackType;

}

public class SkillData : ILoader<int, Skill>
{
    public List<Skill> Skills = new List<Skill>();

    public Dictionary<int, Skill> MakeDict()
    {
        Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
        foreach (Skill skill in Skills)
            dict.Add(skill.id, skill);
        return dict;
    }
}