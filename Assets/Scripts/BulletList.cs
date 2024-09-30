using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class BulletData
{
    public string _bulletName;
    public float _pow;
    public float _speed;
    public bool _through;
    public bool _homing;
    public bool _drain;
}

[CreateAssetMenu(fileName = "BulletList", menuName = "Scriptable Object/BulletList")]
public class BulletList : ScriptableObject
{
    public List<BulletData> _bulletData;
}
