using System.Globalization;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleController : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystemRenderer psRenderer;

    [Header("Initial Settings")]
    [Tooltip("The initial number of particles emitted per second.")]
    [Range(10, 500)]
    public float initialParticleCount = 200f;

    [Tooltip("The initial maximum speed of the particles.")]
    [Range(0.1f, 5f)]
    public float initialMaxSpeed = 1.5f;

    [Tooltip("The initial maximum size of the particles.")]
    [Range(1f, 10f)]
    public float initialMaxSize = 4f;

    [Tooltip("The initial maximum lifetime of each particle in seconds.")]
    [Range(1f, 5f)]
    public float initialLifespan = 2f;

    [Tooltip("The initial color of the particles.")]
    public Color initialParticleColor = new Color(0.024f, 0.714f, 0.831f);

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        psRenderer = GetComponent<ParticleSystemRenderer>();

        var particleMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        psRenderer.material = particleMaterial;

        SetInitialValues();
    }

    void SetInitialValues()
    {
        if (ps == null) return;

        var main = ps.main;
        var emission = ps.emission;

        main.startSpeed = new ParticleSystem.MinMaxCurve(0, initialMaxSpeed);
        main.startSize = new ParticleSystem.MinMaxCurve(1, initialMaxSize);
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, initialLifespan);
        main.startColor = new ParticleSystem.MinMaxGradient(initialParticleColor);
        emission.rateOverTime = initialParticleCount;
    }

    public void UpdateParticleSettings(UpdateParticleParams p)
    {
        if (ps == null) return;

        var main = ps.main;
        var emission = ps.emission;

        main.maxParticles = p.particleCount;
        main.startSpeed = new ParticleSystem.MinMaxCurve(p.maxSpeed);
        main.startSize = new ParticleSystem.MinMaxCurve(p.maxSize);
        main.startLifetime = new ParticleSystem.MinMaxCurve(p.lifespan);
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(p.particleCount / p.lifespan);

        main.startColor = new ParticleSystem.MinMaxGradient(ParseHtmlColor(p.particleColor));
    }

    private Color ParseHtmlColor(string hexString)
    {
        // Remove the '#' if it's there
        if (hexString.StartsWith("#"))
        {
            hexString = hexString.Substring(1);
        }

        if (hexString.Length == 6)
        {
            byte r = byte.Parse(hexString.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hexString.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hexString.Substring(4, 2), NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }

        // Return a default color if parsing fails
        return Color.darkBlue;
    }
}
