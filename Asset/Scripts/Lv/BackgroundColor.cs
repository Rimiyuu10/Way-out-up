using UnityEngine;

public class BackgroundColor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background;


    private void Update()
    {
        background.color = ColorManager.instance.backgroundColor;
    }
}
