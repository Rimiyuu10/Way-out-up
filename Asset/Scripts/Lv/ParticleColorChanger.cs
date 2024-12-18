using UnityEngine;

public class ParticleColorChanger : MonoBehaviour
{
    /*
    [SerializeField] private ParticleSystem[] particleSystems;

    private void Start()
    {
        // Load the platform color from PlayerPrefs
        Color platformColor = LoadColor("PlatformColor");

        // Apply the color to all ParticleSystems
        foreach (var particleSystem in particleSystems)
        {
            var main = particleSystem.main;
            main.startColor = platformColor;
        }
    }

    private Color LoadColor(string colorKeyPrefix)
    {
        float r = PlayerPrefs.GetFloat(colorKeyPrefix + "R", 1f);
        float g = PlayerPrefs.GetFloat(colorKeyPrefix + "G", 1f);
        float b = PlayerPrefs.GetFloat(colorKeyPrefix + "B", 1f);
        float a = PlayerPrefs.GetFloat(colorKeyPrefix + "A", 1f);
        return new Color(r, g, b, a);
    }
    */
}
