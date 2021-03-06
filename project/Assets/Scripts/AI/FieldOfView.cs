﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField]
    public float viewRadius;
    [Range(0, 360)]

    [SerializeField]
    public float viewAngle;

    float oldViewRadius = 0f;
    float oldViewAngle = 0f;

    public float detectedViewAngle = 4f;


    [SerializeField]
    private Color32 _undetectedColor; // color of the fov cone when player is not in sight

    [SerializeField]
    private Color32 _detectedColor; // color of the fov cone when player is in sight

    public LayerMask _obstacleMask;

    float _meshResolution = 10;
    int _edgeResolveIterations = 10;
    float _edgeDstThreshold = 10;

    [HideInInspector]
    public bool _targetFound; // Has player been found
    [HideInInspector]
    public bool _distractionFound; // Has butterfly been detected

    [HideInInspector] 
    bool _drawCone = true;

    [HideInInspector]
    public GameObject player;

    [HideInInspector]
    public GameObject _butterfly;

    PlayerRestart restart;

    float _maskCutawayDst = .1f;

    public MeshFilter viewMeshFilter;
    public MeshRenderer _viewMeshRenderer;

    Mesh viewMesh; //The mesh that is created for th npc
    [HideInInspector] public GameObject teaImage;
    bool activeOnce = false;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        viewMesh = new Mesh(); //Creates Mesh on start
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        _targetFound = false;

        oldViewRadius = viewRadius;  //stores the initialised value
        oldViewAngle = viewAngle; //stores the initialised value

        restart = player.GetComponent<PlayerRestart>();
        StartCoroutine("FindTargetsWithDelay", .2f);
        DrawFieldOfView();
    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
            DistractionInRange();
        }
    }

    void LateUpdate()
    {
        _drawCone = false;
        float dstToTarget = Vector3.Distance(transform.position, player.transform.position);

        if (dstToTarget < 20)
        {
            _drawCone = true;
        }
        if (_drawCone == true)
        {
            DrawFieldOfView();
        }
    }

    public void FindVisibleTargets()
    {
        Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;

        if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
        {
            float dstToTarget = Vector3.Distance(transform.position, player.transform.position);

            if (!Physics.Raycast(transform.position, dirToPlayer, dstToTarget, _obstacleMask) && dstToTarget <= viewRadius)
            {
                _targetFound = true;
                TeaPlaceMechanic._teaCanBePlaced = false;
            }
            else
            {
                restart._playerPosrestart = false;
                _targetFound = false;
                TeaPlaceMechanic._teaCanBePlaced = true;

            }
        }
    }

    public void DistractionInRange() //Checks if there is any distractions in the fov and will set bools for each distraction that is found to true
    {
        _butterfly = GameObject.FindGameObjectWithTag("FairyBull");
        if (_butterfly != null)
        {

            Vector3 dirToButterfly = (_butterfly.transform.position - transform.position);
            if (Vector3.Distance(transform.forward, dirToButterfly) <= viewRadius)
            {
                _targetFound = false;
                _distractionFound = true;
            }
            else
            {
                _distractionFound = false;
            }
        }


    }
    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * _meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        float dstToTarget = Vector3.Distance(transform.position, player.transform.position);

        if (dstToTarget < 40)
        {
            _drawCone = true;
        }

        if (_drawCone == true)
        {
            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(angle);

                if (i > 0)
                {
                    bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > _edgeDstThreshold;
                    if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                    {
                        EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                        if (edge.pointA != Vector3.zero)
                        {
                            viewPoints.Add(edge.pointA);
                        }
                        if (edge.pointB != Vector3.zero)
                        {
                            viewPoints.Add(edge.pointB);
                        }
                    }

                }
                viewPoints.Add(newViewCast.point);
                oldViewCast = newViewCast;
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;
            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + Vector3.forward * _maskCutawayDst;

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            if (_targetFound) //if the target is found inside fov chnage the color to detected
            {
                _viewMeshRenderer.material.color = _detectedColor;
                viewAngle = 360;
                viewRadius = detectedViewAngle;
            }
            else
            {
                _viewMeshRenderer.enabled = true;
                viewAngle = oldViewAngle;
                viewRadius = oldViewRadius;
                _viewMeshRenderer.material.color = _undetectedColor;
            }
            viewMesh.Clear();


            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        }
    }


    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < _edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > _edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }


    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, _obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
