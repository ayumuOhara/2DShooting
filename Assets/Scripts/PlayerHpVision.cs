using UnityEngine;

public class PlayerHpVision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Renderer>().material.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ColorChange(float hp,float maxHp)
    {
        float _playerHpRate = hp / maxHp;
        if (_playerHpRate < 0.25f)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else if (_playerHpRate < 0.5f)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
    }
}
