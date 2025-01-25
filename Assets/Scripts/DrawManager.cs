using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawManager : MonoBehaviour
{
    private Camera _cam;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Line _linePrefab;
    [SerializeField] private Bezier _bezier;
    [SerializeField] private Transform ball; // ������ �� ������ ����

    public const float RESOLUTION = .1f;

    private Line _currentLine;

    void Start()
    {
        _cam = Camera.main;
        Debug.Log(_canvas.GetComponent<Canvas>().pixelRect);
        _bezier = GetComponent<Bezier>();
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            if (_currentLine != null)
            {
                Destroy(_currentLine.gameObject);
            }

            _currentLine = Instantiate(_linePrefab, _canvas.transform);
        }

        if (Input.GetMouseButton(0))
        {
            _currentLine.SetPosition(mousePos, _canvas.GetComponent<Canvas>());
        }

        if (Input.GetMouseButtonUp(0))
        {
            // �������� ����� ������
            List<Vector2> swipePoints2D = _currentLine.GetComponent<Line>().GetPoints();
            // ����������� �� � 3D
            List<Vector3> swipePoints3D = ConvertTo3DPoints(swipePoints2D, ball.position);
            // �������� � ������ Bezier
            _bezier.swipePoints = swipePoints3D;
            _bezier.ApproxBezier();
        }
    }

    // �������������� ������ 2D ����� � 3D
    private List<Vector3> ConvertTo3DPoints(List<Vector2> points2D, Vector3 initialPosition)
    {
        List<Vector3> points3D = new List<Vector3>();
        float zOffset = 0f; // ��������� �������� �� ��� Z

        for (int i = 0; i < points2D.Count; i++)
        {
            Vector2 point2D = points2D[i];

            // ����������� ���������� ���� � ������� ����������
            Vector3 worldPoint = _cam.ScreenToWorldPoint(new Vector3(point2D.x, point2D.y, _cam.nearClipPlane));
            // ��������� �������� Z ��������������� ������� �����
            zOffset += 0.1f; // ������ ��� �� Z (������������)
            Vector3 point3D = new Vector3(worldPoint.x, worldPoint.y, initialPosition.z + zOffset);

            points3D.Add(point3D);
        }

        return points3D;
    }
}