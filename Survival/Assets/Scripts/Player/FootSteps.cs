using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public AudioClip[] footStepClips;
    private AudioSource audioSource;
    private Rigidbody _rigidbody;
    public float footStepThreshold;
    public float footStepRate;
    private float lastFootStepTime;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(Mathf.Abs(_rigidbody.velocity.y) < 0.1f)
        {
            if(_rigidbody.velocity.magnitude > footStepThreshold) 
            {
                if(Time.time - lastFootStepTime > footStepRate) 
                {
                    lastFootStepTime = Time.time;
                    audioSource.PlayOneShot(footStepClips[UnityEngine.Random.Range(0, footStepClips.Length)]);
                }
            }
        }
    }
}
