using UnityEngine;

public static class AnimParams
{
    // Animator Parameter 해시 (성능 + 오타 방지)
    public static readonly int Speed = Animator.StringToHash("Speed");      // float
    public static readonly int IsGrounded = Animator.StringToHash("IsGrounded"); // bool
    public static readonly int Jump = Animator.StringToHash("Jump");       // trigger
    public static readonly int Attack = Animator.StringToHash("Attack");     // trigger
}

[RequireComponent(typeof(CharacterController))]
public class Basic1 : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float accel = 10f;

    [Header("Ground")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask groundMask = ~0;

    private float currentSpeed;

    private void Reset()
    {
        controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // --- 1) 이동 입력
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(h, 0f, v).normalized;

        // 방향이 있으면 앞으로 이동
        if (dir.sqrMagnitude > 0.0001f)
        {
            // 카메라 기준 회전 (간단 구현)
            var cam = Camera.main ? Camera.main.transform : transform;
            Vector3 forward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 right = cam.right;
            Vector3 worldDir = (forward * v + right * h).normalized;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(worldDir), Time.deltaTime * 12f);
            controller.Move(worldDir * moveSpeed * Time.deltaTime);
        }

        // --- 2) 지면 체크
        bool isGrounded = Physics.CheckSphere(groundCheck ? groundCheck.position : transform.position, groundRadius, groundMask);

        // --- 3) 속도 보간(애니메이터용 Speed 파라미터)
        float targetSpeed = dir.magnitude * moveSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.deltaTime);

        // --- 4) Animator 파라미터 세팅
        animator.SetBool(AnimParams.IsGrounded, isGrounded);
        animator.SetFloat(AnimParams.Speed, currentSpeed);

        // --- 5) 점프 / 공격 트리거
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger(AnimParams.Jump); // 상태머신에서 Jump 상태로 전환되도록 설정해두기
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger(AnimParams.Attack); // Attack 상태 또는 상위 레이어에서 재생
        }

        // --- 6) 특수 전환을 코드로 직접 CrossFade (예: 구르기)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // "Roll" 이라는 상태명으로 0.1초 블렌딩하며 즉시 전환
            animator.CrossFadeInFixedTime("Base Layer.Roll", 0.1f);
        }
    }
}
