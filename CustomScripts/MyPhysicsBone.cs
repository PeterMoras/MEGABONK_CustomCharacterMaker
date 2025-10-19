using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyPhysicsBone : MonoBehaviour
{
    [Tooltip("How much resistance the bones have to movement")]
    [Range(0,1)] public float damping = 0.5f;
    [Tooltip("How damping changes down the bone chain")]
    public AnimationCurve dampingCurve= new AnimationCurve(new Keyframe(0f,0f),new Keyframe(1f,1f));
    [Tooltip("How much the bones include the original animation's movement")]
    [Range(0,1)]public float stiffness = 1f;
    [Tooltip("How stiffness changes down the bone chain")]
    public AnimationCurve stiffnessCurve= new AnimationCurve(new Keyframe(0f,0f),new Keyframe(1f,1f));
    [Tooltip("How quickly the bone tries to move back to its original position")]
    [Range(0,1)]public float elasticity = 0.1f;
    [Tooltip("How elasticity changes down the bone chain")]
    public AnimationCurve elasticityCurve = new AnimationCurve(new Keyframe(0f,0f),new Keyframe(1f,1f));

    [Tooltip("How much the bones are locked to the original animation. 1 means no physics sim")]
    [Range(0, 1)] public float inert = 0.5f;
    public AnimationCurve inertCurve = new AnimationCurve(new Keyframe(0f,1f),new Keyframe(1f,0.5f));
    //[Range(0,1)]public float weight = 0.1f;

    //[Range(1,20)]public int iterations = 4;
    public Vector3 gravity = Vector3.down * 0.001f;


    private Vector3[] particlePositions; // Positions of each segment as of last frame
    private Vector3[] previousBonePositions;
    private Vector3[] previousParticlePositions;

    private Transform[] bones = new Transform[0];
    private float[] segmentLengths;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeBones();
    }

    void LateUpdate()
    {
        SimulateBonePhysics();
    }


    // Initialize rope positions
    void InitializeBones()
    {
        
        var children = this.GetComponentsInChildren<Transform>();
        bones = new[] { this.transform.parent }.Concat(children).ToArray();
        
        var segmentCount = bones.Length;
        segmentLengths = new float[segmentCount-1];
        particlePositions = new Vector3[segmentCount];
        previousBonePositions = new Vector3[segmentCount];
        previousParticlePositions = new Vector3[segmentCount];
        // Initialize positions 
        for (int i = 0; i < segmentCount; i++)
        {
            particlePositions[i] = bones[i].position;
            previousBonePositions[i] = bones[i].position;
            previousParticlePositions[i] = particlePositions[i];
            if (i > 0)
            {
                segmentLengths[i-1] = (particlePositions[i] - particlePositions[i-1]).magnitude;
            }
        }
    }
    
    // Physics simulation loop
    void SimulateBonePhysics()
    {
        var dt = 1;//* Time.deltaTime;
        
        for (int iter = 0; iter < 1; iter++)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                var boneTime = i / (bones.Length-1);
                var c_damping = damping * dampingCurve.Evaluate(boneTime);
                var c_elasticity = elasticity * elasticityCurve.Evaluate(boneTime);
                var c_stiffness = stiffness * stiffnessCurve.Evaluate(boneTime);
                var c_inert = inert * inertCurve.Evaluate(boneTime);
                Vector3 targetPosition = bones[i].position;
                Vector3 particlePosition = particlePositions[i];
                Vector3 particleVelocity = particlePosition - previousParticlePositions[i];
                //the movement the bones would normally perform over this timestep, assuming no active particles
                Vector3 boneVelocity = bones[i].position - previousBonePositions[i];
                
                //all movement that is being forced upon the bones
                Vector3 externalMovement = gravity * (dt * dt) + (particleVelocity * (dt * (1 - c_damping))) + (boneVelocity * (dt* (1-c_stiffness)));
                
                Vector3 errorVector = (targetPosition - particlePosition);
                //float error = errorVector.magnitude;
                Vector3 correction = errorVector * (c_elasticity);
                
                previousParticlePositions[i] = particlePositions[i];
                particlePositions[i] = particlePosition + correction + externalMovement;
                particlePositions[i] = Vector3.Lerp(particlePositions[i], targetPosition, c_inert);

            }
        }

        particlePositions[0] = bones[0].position;

        for (int i = 0; i < particlePositions.Length; i++)
        {
            previousBonePositions[i] = bones[i].position;
            bones[i].position = particlePositions[i];
        }
        
        

        
    }
    
    private void OnDrawGizmos()
    {
        if (bones.Length > 0)
        {
            // Visualize the bone chain connections in the scene view
            Gizmos.color = Color.cyan;
            for (int i = 0; i < bones.Length - 1; i++)
            {
                Gizmos.DrawLine(bones[i].position, bones[i + 1].position);
            }
            Gizmos.color = Color.blue;
            for (int i = 0; i < particlePositions.Length - 1; i++)
            {
                Gizmos.DrawLine(previousParticlePositions[i], previousParticlePositions[i + 1]);
            }
        }
    }

}
