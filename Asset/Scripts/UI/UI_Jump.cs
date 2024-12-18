using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Jump : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.instance.player._pauseTimeRemaining > 0)
        {
            return;
        }
        GameManager.instance.player.OnJumpInput();

        // Lặp qua tất cả các crystal và kích hoạt crystal nếu người chơi đang trong vùng
        foreach (Crystal crystal in GameManager.instance.crystals)
        {
            if (crystal != null && crystal.PlayerInZone)
            {
                crystal.PerformJump();
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameManager.instance.player._pauseTimeRemaining > 0)
        {
            return;
        }
        GameManager.instance.player.OnJumpUpInput();
    }
}


