using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public bool isIdle = false;
    public bool isWalking = false;
    public bool isRunning = false;
    public bool isWaiting = false;
    public bool isDying = false;
    public Animator animator = null;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    public void Walking()
    {
        if (!isWalking)
        {
            animator.SetTrigger("Walk");
            isWalking = true;
        }
    }

    public void Running()
    {
        if (!isRunning)
        {
            animator.SetTrigger("Run");
            isRunning = true;
        }

    }
    public void Waiting()
    {
        if (!isWaiting)
        {
            animator.SetTrigger("Wait");
            isWaiting = true;
        }
    }
    public void Dying()
    {
        if (!isDying)
        {
            animator.SetTrigger("Die");
            isDying = true;
        }
    }
}
