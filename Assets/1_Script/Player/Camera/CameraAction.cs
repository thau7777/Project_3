using UnityEngine;
using UnityEngine.TextCore.Text;

public class CameraAction : MonoBehaviour
{
    public static CameraAction instance { get; private set; }

    [Header("Chế độ Target + Offset")]
    public Transform targetPoint;
    public Vector3 offsetPosition;
    public Vector3 offsetRotation;
    public float smoothSpeed = 5f;

    [Header("Điểm Neo Cố Định")]
    public Transform TargetAllPlayer;
    public Transform TargetAllEnemy;

    private Transform currentAnchor;

    private bool shouldTeleport = false;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void LookAtAnchorTransform(Transform anchor, bool teleportImmediately = false)
    {
        targetPoint = null;

        currentAnchor = anchor;
        shouldTeleport = teleportImmediately;

        if (anchor == null)
        {
            Debug.LogWarning("Anchor mục tiêu bị Null. Camera sẽ dừng cập nhật.");
        }
    }

    private void SetTargetAndOffset(Character character, Vector3 posOffset, Vector3 rotOffset, bool teleport = false)
    {
        currentAnchor = null;
        shouldTeleport = teleport; 

        if (character != null)
        {
            targetPoint = character.transform.Find("CameraTarget");

        }
        else
        {
            targetPoint = null;
        }

        offsetPosition = posOffset;
        offsetRotation = rotOffset;
    }

    public void TargetAllTeam()
    {
        LookAtAnchorTransform(TargetAllPlayer, true);
    }

    public void TargetAllEnemies()
    {
        LookAtAnchorTransform(TargetAllEnemy, true);
    }

    public void ResetCamera()
    {
        currentAnchor = null;
        shouldTeleport = false;

        if (targetPoint != null)
        {
            offsetPosition = Vector3.zero;
            offsetRotation = Vector3.zero;
        }
        else
        {
            LookAtAnchorTransform(TargetAllPlayer, false);
        }
    }

    public void LookCameraAtTarget(Character character)
    {
        SetTargetAndOffset(character, Vector3.zero, Vector3.zero);
    }


    public void NormalAttack(Character attacker, bool teleportImmediately = false) 
    {
        Vector3 pos = new Vector3(-0.5f, 0f, -0.77f);
        Vector3 rot = new Vector3(0f, 0f, 0f);

        SetTargetAndOffset(attacker, pos, rot, teleportImmediately);
    }

    public void ReadySkill(Character character)
    {
        Vector3 pos = new Vector3(-0.3f, 0.05f, -0.9f);
        Vector3 rot = new Vector3(8f, 20f, 5f);
        SetTargetAndOffset(character, pos, rot);
    }


    private void LateUpdate()
    {
        Vector3 desiredPos;
        Quaternion desiredRot;

        if (currentAnchor != null)
        {
            desiredPos = currentAnchor.position;
            desiredRot = currentAnchor.rotation;
        }
        else if (targetPoint != null)
        {
            desiredPos = targetPoint.position + offsetPosition;
            desiredRot = targetPoint.rotation * Quaternion.Euler(offsetRotation);
        }
        else
        {
            return;
        }

        if (shouldTeleport)
        {
            transform.position = desiredPos;
            transform.rotation = desiredRot;
            shouldTeleport = false;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * smoothSpeed);
        }
    }
}