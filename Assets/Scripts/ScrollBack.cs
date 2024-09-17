using UnityEngine;

public class ScrollBack : MonoBehaviour
{
    private float speed = 4.0f;

    void Update()
    {
        transform.Translate(new Vector3(0, Time.deltaTime * -speed));

        if (transform.position.y <= -19.2)
        {
            transform.position = new Vector3(0, 19.0f);
        }
    }
}
