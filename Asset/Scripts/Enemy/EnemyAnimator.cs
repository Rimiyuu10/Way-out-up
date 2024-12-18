using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    public float down;

    private EnemyMovement mov;
    private Animator anim;
    private SpriteRenderer spriteRend;

    [Header("Movement Tilt")]
    [SerializeField] private float maxTilt;
    [SerializeField][Range(0, 1)] private float tiltSpeed;

    [Header("Particle FX")]
    [SerializeField] private GameObject flipFX;
    private ParticleSystem _flipParticle;

    public bool startedFlipping { private get; set; }

    private void Start()
    {
        mov = GetComponent<EnemyMovement>();
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        anim = spriteRend.GetComponent<Animator>();
        _flipParticle = flipFX.GetComponent<ParticleSystem>();
    }

    private void LateUpdate()
    {
        #region Tilt
        float tiltProgress;

        int mult = -1;

        tiltProgress = Mathf.InverseLerp(-mov.enemyData.runMaxSpeed, mov.enemyData.runMaxSpeed, mov.RB.velocity.x);
        mult = (mov.isMovingRight) ? 1 : -1;

        float newRot = ((tiltProgress * maxTilt * 2) - maxTilt);
        float rot = Mathf.LerpAngle(spriteRend.transform.localRotation.eulerAngles.z * mult, newRot, tiltSpeed);
        spriteRend.transform.localRotation = Quaternion.Euler(0, 0, rot * mult);
        #endregion

        CheckAnimationState();

        ParticleSystem.MainModule flipPSettings = _flipParticle.main;
        flipPSettings.startColor = new ParticleSystem.MinMaxGradient(ColorManager.instance.platformColor);
    }

    private void CheckAnimationState()
    {
        if (startedFlipping)
        {
            anim.SetTrigger("Land");
            GameObject obj = Instantiate(flipFX, transform.position - (Vector3.up * transform.localScale.y / 1.5f), Quaternion.Euler(-90, 0, 0));
            Destroy(obj, 1);
            startedFlipping = false;
            return;
        }

        anim.SetFloat("Vel Y", mov.RB.velocity.y);

        anim.SetBool("Detecter", mov.playerDetected);

        anim.SetFloat("ReadyDetect", mov.enemyData.detectionProgress);

        // Điều chỉnh tốc độ animation dựa trên detectionProgress
        float animationSpeed = 1 + mov.enemyData.detectionProgress * (mov.enemyData.maxAnimationSpeedMultiplier - 1);
        anim.speed = animationSpeed;
    }
}
