using UnityEngine;

public class FxController : MonoBehaviour
{
    public void OnAnimationEnd()
    {
        Destroy(gameObject);
    }
}
