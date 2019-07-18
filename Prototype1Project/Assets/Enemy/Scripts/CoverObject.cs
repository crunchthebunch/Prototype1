using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[ExecuteInEditMode]
public class CoverObject : MonoBehaviour
{
    public int hp = 0;
    public bool isDestructible;

    Vector3 extent;
    Vector3 dimensions;
    Vector3[] frontCovers, backCovers, leftCovers, rightCovers;
    public Vector3[] coverPoints;
    public bool[] pointTakenList;
    public int depthPoints, widthPoints;
    public bool isFullCover;
    MeshRenderer render;

    private void Awake()
    {
        Quaternion initialRotation = transform.rotation;
        transform.rotation = Quaternion.identity;

        render = GetComponent<MeshRenderer>();
        extent = render.bounds.extents;

        transform.rotation = initialRotation;

        //Calculate number of cover points available
        depthPoints = Mathf.FloorToInt(extent.x * 2.0f); //Front & Back
        widthPoints = Mathf.FloorToInt(extent.z * 2.0f); //Left & Right

        //Calculate if this is full-cover or not (half-cover)
        if (Mathf.CeilToInt(extent.y * 2.0f) >= 2)
        {
            isFullCover = true;
        }
        else
        {
            isFullCover = false;
        }

        frontCovers = new Vector3[depthPoints];
        backCovers = new Vector3[depthPoints];
        leftCovers = new Vector3[widthPoints];
        rightCovers = new Vector3[widthPoints];

        coverPoints = new Vector3[(depthPoints * 2) + (widthPoints * 2)];
        pointTakenList = new bool[(depthPoints * 2) + (widthPoints * 2)];

        PositionCoverPoints();

        for (int i = 0; i < pointTakenList.Length; i++)
        {
            pointTakenList[i] = false;
        }
    }

    private void Update()
    {
        PositionCoverPoints();
    }

    void PositionCoverPoints() //Moves cover points to the correct positions
    {
        int coverToUpdate = 0;

        for (int i = 0; i < frontCovers.Length; i++)
        {
            float fraction = 1.0f / (frontCovers.Length + 1.0f);
            frontCovers[i] = transform.position + (transform.forward * (extent.z * 1.0f));

            if (frontCovers.Length > 1)
            {
                frontCovers[i] -= transform.right * extent.x;
                frontCovers[i] += transform.right * extent.x * 2.0f * fraction * (i + 1);
            }

            coverPoints[coverToUpdate] = frontCovers[i];
            coverToUpdate++;
        }

        for (int i = 0; i < backCovers.Length; i++)
        {
            float fraction = 1.0f / (backCovers.Length + 1.0f);
            backCovers[i] = transform.position + (transform.forward * (-extent.z * 1.0f));

            if (backCovers.Length > 1)
            {
                backCovers[i] -= transform.right * extent.x;
                backCovers[i] += transform.right * extent.x * 2.0f * fraction * (i + 1);
            }

            coverPoints[coverToUpdate] = backCovers[i];
            coverToUpdate++;
        }

        for (int i = 0; i < leftCovers.Length; i++)
        {
            float fraction = 1.0f / (leftCovers.Length + 1.0f);
            leftCovers[i] = transform.position + (transform.right * (-extent.x * 1.0f));

            if (leftCovers.Length > 1)
            {
                leftCovers[i] -= transform.forward * extent.z;
                leftCovers[i] += transform.forward * extent.z * 2.0f * fraction * (i + 1);
            }

            coverPoints[coverToUpdate] = leftCovers[i];
            coverToUpdate++;
        }

        for (int i = 0; i < rightCovers.Length; i++)
        {
            float fraction = 1.0f / (rightCovers.Length + 1.0f);
            rightCovers[i] = transform.position + (transform.right * (extent.x * 1.0f));

            if (rightCovers.Length > 1)
            {
                rightCovers[i] -= transform.forward * extent.z;
                rightCovers[i] += transform.forward * extent.z * 2.0f * fraction * (i + 1);
            }

            coverPoints[coverToUpdate] = rightCovers[i];
            coverToUpdate++;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        for (int i = 0; i < frontCovers.Length; i++)
        {
            Gizmos.DrawSphere(frontCovers[i], 0.2f);
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < backCovers.Length; i++)
        {
            Gizmos.DrawSphere(backCovers[i], 0.2f);
        }
        Gizmos.color = Color.blue;
        for (int i = 0; i < leftCovers.Length; i++)
        {
            Gizmos.DrawSphere(leftCovers[i], 0.2f);
        }
        Gizmos.color = Color.red;
        for (int i = 0; i < rightCovers.Length; i++)
        {
            Gizmos.DrawSphere(rightCovers[i], 0.2f);
        }
    }
}
