using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimPlayer : MonoBehaviour
{
    public List<string> PropertyNames = new List<string>() { "grounded", "moving", "grinding", "jumping" };

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_animator.GetBool("jumping"))
        {
            _animator.SetBool("jumping", false);
        }
        
        if (Input.GetKeyDown(KeyCode.A))
            SetAnimatorProp(PropertyNames[0]);
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetAnimatorProp(PropertyNames[1]);
            _animator.SetBool("grounded", true);

        }
        if (Input.GetKeyDown(KeyCode.D))
            SetAnimatorProp(PropertyNames[2]);
        if (Input.GetKeyDown(KeyCode.F))
            SetAnimatorProp(PropertyNames[3]);
        
    }

    void SetAnimatorProp(string name)
    {
        foreach (var propName in PropertyNames)
        {
            if(propName == name)
                _animator.SetBool(propName, true);
            else
                _animator.SetBool(propName, false);
        }
    }
}
