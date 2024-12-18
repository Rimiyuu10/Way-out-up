using UnityEngine;
[CreateAssetMenu(menuName = "EnemyMovement Data")] //Tạo một đối tượng playerData mới bằng cách nhấp chuột phải trong Menu Project sau đó Create/Player/Player Data và kéo vào người chơi
public class EnemyData : ScriptableObject
{
    [Header("Trọng lực")]
    [HideInInspector] public float gravityStrength; //Lực hướng xuống (trọng lực) cần thiết cho chiều cao nhảy và thời gian tới đỉnh nhảy mong muốn.
    [HideInInspector] public float gravityScale; //Độ mạnh của trọng lực của người chơi dưới dạng bội số của trọng lực (được đặt trong ProjectSettings/Physics2D).
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
    public float runDetected;
    [HideInInspector] public float runAccelAmount; //Lực thực tế (nhân với speedDiff) áp dụng cho người chơi.
    public float runDecceleration; //Tốc độ mà người chơi của chúng tôi giảm tốc độ từ tốc độ hiện tại, có thể được đặt thành runMaxSpeed để giảm tốc độ tức thời xuống 0 cho không giảm tốc độ nào cả
    [HideInInspector] public float runDeccelAmount; //Lực thực tế (nhân với speedDiff) áp dụng cho người chơi.
    [Space(5)]
    [Range(0f, 1)] public float accelInAir; //Các hệ số nhân áp dụng cho tốc độ tăng tốc khi ở trên không.
    [Range(0f, 1)] public float deccelInAir;
    [Space(5)]
    public bool doConserveMomentum = true;
    public float waitTime;

    [Space(20)]
    [Header("Flip")]
    public float minFlipTime; // random min/max thời gian ngẫu nhiên Flip
    public float maxFlipTime;

    [Space(20)]
    [Header("Detecter")]
    public float detectionThreshold; // nếu thanh này đầy, enemy sẽ hóa chó :)))
    public float detectionCooldown; // Khi player ra khỏi phạm vị tìm kiếm, khi cái này hết thì kẻ địch không hóa chao nữa
    public float detectionDistance; // Khoảng cách phát hiện người chơi
    public float detectionDecreaseSpeed; // Thêm biến này để kiểm soát tốc độ giảm
    public float timeMoveNext; // Thêm biến này để thiết lập thời gian dừng
    public float detectionProgress = 0f;
    public float maxAnimationSpeedMultiplier = 2f;

    [HideInInspector] public float detectionTimeout = 0f;
    [Space(10)]
    public Color startColor = Color.white; // màu sắc kẻ địch hóa chao
    public Color endColor = Color.red;


    //Unity Callback, gọi khi trình kiểm tra cập nhật
    private void OnValidate()
    {
        //Tính toán trọng lực của rigidbody (tức là: độ mạnh của trọng lực tương đối với giá trị trọng lực của unity, xem project settings/Physics2D)
        gravityScale = gravityStrength / Physics2D.gravity.y;

        //Tính toán lực tăng tốc & giảm tốc độ chạy bằng công thức: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;


        #region Các Phạm Vi Biến
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion
    }
}
