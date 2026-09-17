using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public sealed class SkillPresentationData
{
    [SerializeField] private SkillAnimationParameterType animationParameterType;
    [FormerlySerializedAs("animationTrigger")]
    [SerializeField] private string animationParameter;
    [SerializeField] private string animationStartTrigger;
    [SerializeField] private AudioClip castSound;
    [SerializeField] private GameObject castEffectPrefab;
    [SerializeField] private GameObject impactEffectPrefab;

    public SkillAnimationParameterType AnimationParameterType =>
        animationParameterType;
    public string AnimationParameter => animationParameter;
    public string AnimationStartTrigger => animationStartTrigger;
    public AudioClip CastSound => castSound;
    public GameObject CastEffectPrefab => castEffectPrefab;
    public GameObject ImpactEffectPrefab => impactEffectPrefab;
}
