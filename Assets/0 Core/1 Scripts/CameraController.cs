using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 11.98111f, -14.10971f);
    public float smoothing = 2;
    public Vector3 targetRotate;

    public PostEffectsBase postEffect1, postEffect2;

    private Vector3 lastMousePosition;
    void Update()
    {
        if (target == null)
            return;

       

        // �������Ҽ�����
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }
        // �������Ҽ�������ס
        else if (Input.GetMouseButton(1))
        {
            // ��������ƶ��ľ���
            Vector3 delta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            // ��������ƶ���xzƽ����
            delta.z = delta.y;
            delta.y = 0;

            // ���ƶ�Ӧ�����������λ��
            transform.Translate(-delta * Time.deltaTime,Space.World);
        }
        else
        {
            Follow();
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        enabled = true;
        postEffect1.enabled = false;
        postEffect2.enabled = false;
    }

    void Follow()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotate), smoothing * Time.deltaTime);
        //transform.LookAt(target);
    }
}
