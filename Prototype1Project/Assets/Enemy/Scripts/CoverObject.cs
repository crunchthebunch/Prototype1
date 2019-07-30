using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[ExecuteInEditMode]
public struct CoverPoint
{
    public Vector3 pos;
    public int id;
    public int side; //0 = front, 1 = back, 2 = left, 3 = right
    public CoverObject parent;
    public bool isTaken;
};

[ExecuteInEditMode]
public class CoverObject : MonoBehaviour
{
    public int hp = 0;
    public bool isDestructible;

    Vector3 extent;
    Vector3 dimensions;
    public bool[] isTakenList;
    public CoverPoint[] frontPoints, backPoints, leftPoints, rightPoints;
    public Vector3[] frontGizmos, backGizmos, leftGizmos, rightGizmos;
    public CoverPoint[] coverPoints;
    public int depthPoints, widthPoints;
    public bool isFullCover;
    MeshRenderer render;

    float originOffset = 0.2f;

    private void Awake()
    {
        Quaternion initialRotation = transform.rotation;
        transform.rotation = Quaternion.identity;

        render = GetComponent<MeshRenderer>();
        extent = render.bounds.extents;

        transform.rotation = initialRotation;

        //Calculate number of cover points available
        depthPoints = Mathf.CeilToInt(extent.x * 2.0f); //Front & Back
        widthPoints = Mathf.CeilToInt(extent.z * 2.0f); //Left & Right

        //Calculate if this is full-cover or not (half-cover)
        if (Mathf.CeilToInt(extent.y * 2.0f) >= 2)
        {
            isFullCover = true;
        }
        else
        {
            isFullCover = false;
        }

        frontPoints = new CoverPoint[depthPoints];
        backPoints = new CoverPoint[depthPoints];
        leftPoints = new CoverPoint[widthPoints];
        rightPoints = new CoverPoint[widthPoints];

        frontGizmos = new Vector3[depthPoints];
        backGizmos = new Vector3[depthPoints];
        leftGizmos = new Vector3[widthPoints];
        rightGizmos = new Vector3[widthPoints];


        coverPoints = new CoverPoint[(depthPoints * 2) + (widthPoints * 2)];
        isTakenList = new bool[(depthPoints * 2) + (widthPoints * 2)];

        //Set all coverpoint IDs, and also parent to this
        for (int c = 0; c < coverPoints.Length; c++)
        {
            coverPoints[c].pos = transform.position;
            coverPoints[c].isTaken = false;
            coverPoints[c].side = 0;
            coverPoints[c].id = c;
            coverPoints[c].parent = this;
        }

        PositionCoverPoints();
    }

    private void Update()
    {
        PositionCoverPoints();
    }

    public int FurthestSide(Vector3 checkPosition) //Find the futherest side of the cover object to the checkPosition
    {
        int closest = 0;

        Vector3 checkFront, checkBack, checkLeft, checkRight;
        checkFront = transform.position + (transform.forward * extent.z);
        checkBack = transform.position + (transform.forward * -extent.z);
        checkLeft = transform.position + (transform.right * -extent.z);
        checkRight = transform.position + (transform.right * extent.z);

        float[] dists = new float[4];
        dists[0] = Vector3.Distance(checkPosition, checkFront);
        dists[1] = Vector3.Distance(checkPosition, checkBack);
        dists[2] = Vector3.Distance(checkPosition, checkLeft);
        dists[3] = Vector3.Distance(checkPosition, checkRight);

        float bestDist = -1000.0f;
        int bestID = 0;

        for (int id = 0; id < dists.Length; id++)
        {
            if (dists[id] > bestDist)
            {
                bestDist = dists[id];
                bestID = id;
            }
        }

        switch (bestID)
        {
            case 0:
                closest = 0;
                break;
            case 1:
                closest = 1;
                break;
            case 2:
                closest = 2;
                break;
            case 3:
                closest = 3;
                break;
            default:
                break;
        }

        return closest;
    }

    void PositionCoverPoints() //Moves cover points to the correct positions
    {
        int coverToUpdate = 0;

        for (int i = 0; i < frontPoints.Length; i++)
        {
            float fraction = 1.0f / (frontPoints.Length + 1.0f);
            frontPoints[i].id = coverToUpdate;
            frontPoints[i].parent = this;
            frontPoints[i].pos = transform.position + (transform.forward * (extent.z * 1.0f));
            frontPoints[i].pos += transform.forward * originOffset;
            frontPoints[i].side = 0;

            if (frontPoints.Length > 1)
            {
                frontPoints[i].pos -= transform.right * extent.x;
                frontPoints[i].pos += transform.right * extent.x * 2.0f * fraction * (i + 1);
            }

            frontGizmos[i] = frontPoints[i].pos;

            coverPoints[coverToUpdate] = frontPoints[i];
            coverToUpdate++;
        }

        for (int i = 0; i < backPoints.Length; i++)
        {
            float fraction = 1.0f / (backPoints.Length + 1.0f);
            backPoints[i].id = coverToUpdate;
            backPoints[i].parent = this;
            backPoints[i].pos = transform.position + (transform.forward * (-extent.z * 1.0f));
            backPoints[i].pos += -transform.forward * originOffset;
            backPoints[i].side = 1;

            if (backPoints.Length > 1)
            {
                backPoints[i].pos -= transform.right * extent.x;
                backPoints[i].pos += transform.right * extent.x * 2.0f * fraction * (i + 1);
            }

            backGizmos[i] = backPoints[i].pos;

            coverPoints[coverToUpdate] = backPoints[i];
            coverToUpdate++;
        }

        for (int i = 0; i < leftPoints.Length; i++)
        {
            float fraction = 1.0f / (leftPoints.Length + 1.0f);
            leftPoints[i].id = coverToUpdate;
            leftPoints[i].parent = this;
            leftPoints[i].pos = transform.position + (transform.right * (-extent.x * 1.0f));
            leftPoints[i].pos += -transform.right * originOffset;
            leftPoints[i].side = 2;

            if (leftPoints.Length > 1)
            {
                leftPoints[i].pos -= transform.forward * extent.z;
                leftPoints[i].pos += transform.forward * extent.z * 2.0f * fraction * (i + 1);
            }

            leftGizmos[i] = leftPoints[i].pos;

            coverPoints[coverToUpdate] = leftPoints[i];
            coverToUpdate++;
        }

        for (int i = 0; i < rightPoints.Length; i++)
        {
            float fraction = 1.0f / (rightPoints.Length + 1.0f);
            rightPoints[i].id = coverToUpdate;
            rightPoints[i].parent = this;
            rightPoints[i].pos = transform.position + (transform.right * (extent.x * 1.0f));
            rightPoints[i].pos += transform.right * originOffset;
            rightPoints[i].side = 3;

            if (rightPoints.Length > 1)
            {
                rightPoints[i].pos -= transform.forward * extent.z;
                rightPoints[i].pos += transform.forward * extent.z * 2.0f * fraction * (i + 1);
            }

            rightGizmos[i] = rightPoints[i].pos;

            coverPoints[coverToUpdate] = rightPoints[i];
            coverToUpdate++;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        for (int i = 0; i < frontGizmos.Length; i++)
        {
            Gizmos.DrawSphere(frontGizmos[i], 0.2f);
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < backGizmos.Length; i++)
        {
            Gizmos.DrawSphere(backGizmos[i], 0.2f);
        }
        Gizmos.color = Color.blue;
        for (int i = 0; i < leftGizmos.Length; i++)
        {
            Gizmos.DrawSphere(leftGizmos[i], 0.2f);
        }
        Gizmos.color = Color.red;
        for (int i = 0; i < rightGizmos.Length; i++)
        {
            Gizmos.DrawSphere(rightGizmos[i], 0.2f);
        }
    }
}
