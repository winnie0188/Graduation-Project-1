using System;
using UnityEngine;

[Serializable]
public class MouseLook
{
    public float XSensitivity;
    public float YSensitivity;


    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;


    public bool m_cursorIsLocked;

    public float MinimumX;
    public float MaximumX;


    public void Init(Transform character, Transform camera)
    {
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;

        // 設定靈敏度
        XSensitivity = 1;
        YSensitivity = 1;


        MaximumX = 80F;
        MinimumX = -30F;
    }


    public void LookRotation(Transform character, Transform camera)
    {

        float yRot = Input.GetAxis("Mouse X") * XSensitivity;
        float xRot = Input.GetAxis("Mouse Y") * YSensitivity;


        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);


        character.localRotation = m_CharacterTargetRot;




        m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);
        camera.localRotation = m_CameraTargetRot;


        InternalLockUpdate();
    }

    public void InternalLockUpdate()
    {
        if (m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
        else if (!m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    public void openLock()
    {
        m_cursorIsLocked = true;
    }
    public void closeLock()
    {
        m_cursorIsLocked = false;
    }


}

