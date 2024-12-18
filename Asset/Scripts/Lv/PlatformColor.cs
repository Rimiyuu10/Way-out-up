using UnityEngine;

public class PlatformColor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] headerSr;
    [SerializeField] private SpriteRenderer[] sr;

    private void Start()
    {
        foreach (var header in headerSr)
        {
            header.transform.parent = transform.parent;
            header.color = ColorManager.instance.platformColor;
        }
    }

    private void Update()
    {
        foreach (var header in headerSr)
        {
            header.color = ColorManager.instance.platformColor;
        }
    }
}
