using UnityEngine;

// 특정 상태(예: Walk/Run) 클립에 이 SMB를 붙이면 단계별 이벤트 제어가 쉬워짐
public class FootstepSMB : StateMachineBehaviour
{
    [Range(0f, 1f)] public float firstStep = 0.2f; // 첫 발자국 타이밍(정규화 시간)
    [Range(0f, 1f)] public float secondStep = 0.7f;

    private bool firstDone;
    private bool secondDone;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        firstDone = false;
        secondDone = false;
        // 상태 입장 시 1회 처리 로직
        // Debug.Log("Enter Walk/Run");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float t = stateInfo.normalizedTime % 1f;

        if (!firstDone && t >= firstStep)
        {
            firstDone = true;
            // 예: 발소리 / 먼지 파티클
            // AudioSource.PlayClipAtPoint(footSfx, animator.transform.position);
            // SpawnDust(animator.transform.position);
            // Debug.Log("Footstep 1");
        }
        if (!secondDone && t >= secondStep)
        {
            secondDone = true;
            // Debug.Log("Footstep 2");
        }

        // 루프 상태면 한 바퀴 돌 때마다 리셋
        if (stateInfo.loop && t < 0.05f)
        {
            firstDone = false;
            secondDone = false;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 상태 종료 시 처리(예: 이동 잔여값 클리어)
        // Debug.Log("Exit Walk/Run");
    }
}
