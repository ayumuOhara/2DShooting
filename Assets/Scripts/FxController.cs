using UnityEngine;

public class FxController : MonoBehaviour
{
    Animator animator;
    PlayerController _player;
    float _time;

    private void Start()
    {
        animator = GetComponent<Animator>();
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public void OnAnimationEnd()
    {
        if(_player._status.IsDead())
        {
            _time += Time.deltaTime;
            animator.speed = 0.01f;
            if (_time > 2.0f)
            {
                animator.speed = 1.0f;
            }
        }
        Destroy(gameObject);
    }
}
