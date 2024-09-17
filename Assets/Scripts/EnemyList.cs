using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class EnemyData
{
    public string _name;
    public float _hp;
    public float _pow;
    public float _rate;
    public float _speed;
}

[CreateAssetMenu(menuName = "ScriptableObject/Enemy Data", fileName = "EnemyData")]
public class EnemyList : ScriptableObject
{
    public List<EnemyData> _data;
}
