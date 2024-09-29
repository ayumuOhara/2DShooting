using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class BulletData
{
    public string _bulletName;
    public float _pow;
    public float _speed;
}

[CreateAssetMenu(fileName = "BulletList", menuName = "Scriptable Object/BulletList")]
public class BulletList : ScriptableObject
{
    public List<BulletData> _bulletData;
}
