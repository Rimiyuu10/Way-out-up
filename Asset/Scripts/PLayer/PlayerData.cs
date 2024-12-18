using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")] //Tạo một đối tượng playerData mới bằng cách nhấp chuột phải trong Menu Project sau đó Create/Player/Player Data và kéo vào người chơi
public class PlayerData : ScriptableObject
{
    [Header("Trọng lực")]
    public float gravityStrength; //Lực hướng xuống (trọng lực) cần thiết cho chiều cao nhảy và thời gian tới đỉnh nhảy mong muốn.
    public float gravityScale; //Độ mạnh của trọng lực của người chơi dưới dạng bội số của trọng lực (được đặt trong ProjectSettings/Physics2D).
    //Cũng là giá trị được đặt cho rigidbody2D.gravityScale của người chơi.
    [Space(5)]
    public float fallGravityMult; //Hệ số nhân cho gravityScale của người chơi khi rơi xuống.
    public float maxFallSpeed; //Tốc độ rơi tối đa (vận tốc cực đại) của người chơi khi rơi xuống.
    [Space(5)]
    public float fastFallGravityMult; //Hệ số nhân lớn hơn cho gravityScale của người chơi khi họ rơi xuống và nhấn phím điều hướng xuống.
    //Thấy trong các trò chơi như Celeste, cho phép người chơi rơi nhanh hơn nếu họ muốn.
    public float maxFastFallSpeed; //Tốc độ rơi tối đa (vận tốc cực đại) của người chơi khi thực hiện rơi nhanh hơn.

    [Space(20)]

    [Header("Chạy")]
    public float runMaxSpeed; //Tốc độ mục tiêu mà chúng tôi muốn người chơi đạt được.
    public float runAcceleration; //Tốc độ mà người chơi của chúng tôi tăng tốc tới tốc độ tối đa, có thể được đặt thành runMaxSpeed để tăng tốc tức thời xuống 0 cho không tăng tốc độ nào cả
    [HideInInspector] public float runAccelAmount; //Lực thực tế (nhân với speedDiff) áp dụng cho người chơi.
    public float runDecceleration; //Tốc độ mà người chơi của chúng tôi giảm tốc độ từ tốc độ hiện tại, có thể được đặt thành runMaxSpeed để giảm tốc độ tức thời xuống 0 cho không giảm tốc độ nào cả
    [HideInInspector] public float runDeccelAmount; //Lực thực tế (nhân với speedDiff) áp dụng cho người chơi.
    [Space(5)]
    [Range(0f, 1)] public float accelInAir; //Các hệ số nhân áp dụng cho tốc độ tăng tốc khi ở trên không.
    [Range(0f, 1)] public float deccelInAir;
    [Space(5)]
    public bool doConserveMomentum = true;
    public float _pauseTimeRemaining; // Thời gian tạm dừng còn lại, khi LoadScene, nhân vật sẽ dừng lại 1 nhịp

    [Space(20)]

    [Header("Nhảy")]
    public float jumpHeight; //Chiều cao của cú nhảy của người chơi
    public float jumpTimeToApex; //Thời gian giữa việc áp dụng lực nhảy và đạt đến chiều cao nhảy mong muốn. Những giá trị này cũng kiểm soát trọng lực và lực nhảy của người chơi.
    public int jumpAmount;
    [HideInInspector] public float jumpForce; //Lực thực tế được áp dụng (hướng lên) cho người chơi khi họ nhảy.

    [Header("Cả Hai Lần Nhảy")]
    public float jumpCutGravityMult; //Hệ số nhân để tăng trọng lực nếu người chơi nhả nút nhảy khi vẫn đang nhảy
    [Range(0f, 1)] public float jumpHangGravityMult; //Giảm trọng lực khi gần đến đỉnh (chiều cao tối đa mong muốn) của cú nhảy
    public float jumpHangTimeThreshold; //Tốc độ (gần bằng 0) nơi người chơi sẽ trải qua thêm "treo nhảy". Vận tốc.y của người chơi gần bằng 0 nhất ở đỉnh của cú nhảy (nghĩ về gradient của một parabol hoặc hàm bậc hai)
    [Space(0.5f)]
    public float jumpHangAccelerationMult;
    public float jumpHangMaxSpeedMult;

    [Header("Hỗ Trợ")]
    [Range(0f, 0.5f)] public float coyoteTime; //Khoảng thời gian ân hạn sau khi rời khỏi nền tảng, nơi bạn vẫn có thể nhảy
    [Range(0f, 0.5f)] public float jumpInputBufferTime; //Khoảng thời gian ân hạn sau khi nhấn nhảy, một cú nhảy sẽ tự động được thực hiện khi các yêu cầu (ví dụ: tiếp đất) được đáp ứng.

    //Unity Callback, gọi khi trình kiểm tra cập nhật
    private void OnValidate()
    {
        //Tính toán lực trọng lực bằng công thức (trọng lực = 2 * chiều cao nhảy / timeToJumpApex^2) 
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

        //Tính toán trọng lực của rigidbody (tức là: độ mạnh của trọng lực tương đối với giá trị trọng lực của unity, xem project settings/Physics2D)
        gravityScale = gravityStrength / Physics2D.gravity.y;

        //Tính toán lực tăng tốc & giảm tốc độ chạy bằng công thức: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        //Tính toán lực nhảy bằng công thức (initialJumpVelocity = trọng lực * timeToJumpApex)
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        #region Các Phạm Vi Biến
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion
    }
}
