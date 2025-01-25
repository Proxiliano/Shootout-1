using UnityEngine;
using System.Collections.Generic;

public class Bezier : MonoBehaviour
{
    // �������� ����� ������ (������� � ���������� ��� ����������� ��������������)
    public List<Vector3> swipePoints;

    // ��������� � 4 ����������� ����� �����
    private Vector3 P0, P1, P2, P3;

    [SerializeField] private int iterations = 1000;     // ���������� �������� ������������ ������
    [SerializeField] private float learningRate = 0.01f; // �������� �������� (���)

    public void ApproxBezier()
    {
        if (swipePoints == null || swipePoints.Count < 2)
        {
            Debug.LogError("������������ ����� ��� �������������.");
            return;
        }

        // 1) ��������� t
        int n = swipePoints.Count;
        List<float> tList = new List<float>();
        for (int i = 0; i < n; i++)
        {
            tList.Add((float)i / (n - 1));
        }

        // 2) ������ P0 � P3
        P0 = swipePoints[0];
        P3 = swipePoints[n - 1];

        // �������������� ��������� P1 � P2 ��� ����� ����� �� ������ (P0,P3).
        // ��������, ����� P1 ���� ������� �� 1/3, P2 �� 2/3 ������� [P0,P3]
        P1 = Vector3.Lerp(P0, P3, 1f / 3f);
        P2 = Vector3.Lerp(P0, P3, 2f / 3f);

        // 3) ��������� ����������� �����
        for (int iter = 0; iter < iterations; iter++)
        {
            // �������� ��������� ��� P1 � P2
            Vector3 gradP1 = Vector3.zero;
            Vector3 gradP2 = Vector3.zero;

            for (int i = 0; i < n; i++)
            {
                float t = tList[i];
                Vector3 Q = swipePoints[i];

                // ������� B(t)
                Vector3 B = BezierPoint(P0, P1, P2, P3, t);
                // ������ = B - Q
                Vector3 diff = B - Q;

                // ����������� B(t) �� P1 � P2 (�������)
                // dB/dP1 = 3(1 - t)^2 * t
                // dB/dP2 = 3(1 - t) * t^2
                float dB1 = 3 * Mathf.Pow((1 - t), 2) * t;
                float dB2 = 3 * (1 - t) * Mathf.Pow(t, 2);

                // �������� (����� d(Error)/dP1 ��� dP2)
                // d(Error)/dP1 = 2 * diff * dB/dP1 (�� ������� i)
                gradP1 += 2f * diff * dB1;
                gradP2 += 2f * diff * dB2;
            }

            // ��������� P1 � P2 (������ ������ ���������)
            P1 -= gradP1 * learningRate;
            P2 -= gradP2 * learningRate;
        }

        // �� ������ �������� P0, P1, P2, P3 � ���� ������� ����������� �����
        Debug.Log($"P0 = {P0}, P1 = {P1}, P2 = {P2}, P3 = {P3}");
    }

    // ������� ��� ���������� ����� �� ������ ����� �� t
    private Vector3 BezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1f - t;
        float u2 = u * u;
        float t2 = t * t;

        return (u2 * u) * p0 +
               (3f * u2 * t) * p1 +
               (3f * u * t2) * p2 +
               (t2 * t) * p3;
    }

    
}

