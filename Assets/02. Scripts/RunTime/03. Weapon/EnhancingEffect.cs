using Coffee.UIExtensions;
using UnityEngine;

public class EnhancingEffect : MonoBehaviour
{
    [SerializeField] private UIParticle PStartEffect;
    private ParticleSystem startPaticle;

    [SerializeField] private UIParticle PEndEffect;
    private ParticleSystem endPaticle;

    void Awake()
    {
        startPaticle = PStartEffect.GetComponent<ParticleSystem>();
        endPaticle = PEndEffect.GetComponent<ParticleSystem>();
    }

    public void StartEffect()
    {
        if (endPaticle.isPlaying) endPaticle.Stop();
        PStartEffect.Play();
    }

    public void EndEffect()
    {
        if (startPaticle.isPlaying) startPaticle.Stop();
        PEndEffect.Play();
    }
}
