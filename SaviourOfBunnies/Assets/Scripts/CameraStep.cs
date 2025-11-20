using System.Collections;
using UnityEngine;

public class CameraStep : MonoBehaviour
{

    public Transform target;   // کاراکتر
    public float offset;     

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 newPos = transform.position;       // گرفتن موقعیت فعلی
        newPos.x = target.position.x + offset;     // تغییر محور x
        transform.position = newPos;
    }
    //[SerializeField] private float speed;
    //[SerializeField] private GameObject player;


    //[Header("Follow target (optional)")]
    //public Transform target; // optional, for centering on player vertically if wanted

    //[Header("Step settings")]
    //public float blockSize = 2f;      // فاصله یک بلوک در واحد یونیتی (مقدار دلخواه متناسب با سایز بلوک‌ها)
    //public float smoothTime = 0.15f;  // زمان نرم شدن حرکت
    //public bool followX = true;       // حرکت مرحله‌ای روی محور X
    //public bool followY = false;      // اگر می‌خواهی مرحله‌ای روی Y هم باشه
    //public Vector3 baseOffset = new Vector3(0f, 0f, -10f); // پایه‌ی افست دوربین

    //[Header("Optional bounds")]
    //public float minX = -Mathf.Infinity;
    //public float maxX = Mathf.Infinity;
    //public float minY = -Mathf.Infinity;
    //public float maxY = Mathf.Infinity;

    //// internal
    //Vector3 velocity = Vector3.zero;
    //bool isMoving = false;
    //Vector3 targetPosition;

    void Start()
    {
    //    // initial position aligned to block grid (optional: snap to nearest integer multiple)
    //    Vector3 pos = transform.position;
    //    pos.z = baseOffset.z;
    //    transform.position = pos;
    //    targetPosition = transform.position;
    }

    //void LateUpdate()
    //{
    //    // smooth follow to targetPosition
    //    if (isMoving)
    //    {
    //        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    //        // stop small residuals
    //        if ((transform.position - targetPosition).sqrMagnitude < 0.0001f)
    //        {
    //            transform.position = targetPosition;
    //            isMoving = false;
    //        }
    //    }
    //    else
    //    {
    //        // if you want camera to track player's vertical continuously:
    //        if (target != null && !followY)
    //        {
    //            // keep y aligned to target's y + offset smoothly (optional)
    //            // comment out if not needed
    //            // float desiredY = Mathf.Clamp(target.position.y + baseOffset.y, minY, maxY);
    //            // Vector3 p = transform.position;
    //            // p.y = desiredY;
    //            // transform.position = p;
    //        }
    //    }
    //}

    //// call this to move the camera forward by exactly one block (or -1 for back)
    //public void StepForward(int steps = 1)
    //{
    //    StepToOffset(Vector2.right * blockSize * steps);
    //}

    //// explicit: step on Y as well if followY true
    //public void StepVertical(int steps = 1)
    //{
    //    StepToOffset(Vector2.up * blockSize * steps);
    //}

    //// general step
    //public void StepToOffset(Vector2 delta)
    //{
    //    if (isMoving)
    //    {
    //        // optional: queue moves or ignore
    //        return;
    //    }

    //    Vector3 newPos = targetPosition;

    //    if (followX)
    //        newPos.x += delta.x;
    //    if (followY)
    //        newPos.y += delta.y;

    //    // apply bounds
    //    newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
    //    newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
    //    newPos.z = baseOffset.z;

    //    targetPosition = newPos;
    //    isMoving = true;
    //}

    //// convenience: move to block index (0-based)
    //public void StepToBlockIndex(int blockIndexX, int blockIndexY = 0)
    //{
    //    Vector3 newPos = transform.position;
    //    if (followX)
    //        newPos.x = blockIndexX * blockSize;
    //    if (followY)
    //        newPos.y = blockIndexY * blockSize;
    //    newPos.z = baseOffset.z;

    //    newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
    //    newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

    //    targetPosition = newPos;
    //    isMoving = true;
    //}

    //// reset camera to player's current block (useful on respawn)
    //public void SnapToPlayerBlock(Vector3 playerPos)
    //{
    //    Vector3 newPos = transform.position;
    //    if (followX)
    //        newPos.x = Mathf.Round(playerPos.x / blockSize) * blockSize;
    //    if (followY)
    //        newPos.y = Mathf.Round(playerPos.y / blockSize) * blockSize;
    //    newPos.z = baseOffset.z;
    //    transform.position = newPos;
    //    targetPosition = newPos;
    //    isMoving = false;
    //}
}
