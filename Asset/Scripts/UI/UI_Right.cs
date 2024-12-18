using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Right : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.instance.player._pauseTimeRemaining > 0)
        {
            return;
        }
        GameManager.instance.player.MoveRight();
    }
            
    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameManager.instance.player._pauseTimeRemaining > 0)
        {
            return;
        }
        GameManager.instance.player.StopMoving();
    }
}
