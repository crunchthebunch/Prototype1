using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverObject : MonoBehaviour
{
    public int hp;

    Vector3 meshSize;
    Vector3 dimensions;
    Vector3 frontCover, backCover, leftCover, rightCover;
    public Vector3[] coverPoints;

    private void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        meshSize = renderer.bounds.size;
        coverPoints = new Vector3[4];
        PositionCoverPoints();
        coverPoints[0] = frontCover;
        coverPoints[1] = backCover;
        coverPoints[2] = leftCover;
        coverPoints[3] = rightCover;
    }

    private void Update()
    {
    }

    void PositionCoverPoints() //Moves cover points to the correct positions
    {

        dimensions = meshSize; // new Vector3(meshSize.x, meshSize.y, meshSize.z);
        frontCover = transform.position + (transform.forward * (dimensions.z * 0.5f * transform.localScale.z));
        backCover = transform.position + (transform.forward * (-dimensions.z * 0.5f * transform.localScale.z));
        leftCover = transform.position + (transform.right * (-dimensions.x * 0.5f * transform.localScale.x));
        rightCover = transform.position + (transform.right * (dimensions.x * 0.5f * transform.localScale.x));
    }

    void OnDrawGizmos()
    {
        PositionCoverPoints();
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(frontCover, 0.2f);
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(backCover, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(leftCover, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(rightCover, 0.2f);
    }
}
