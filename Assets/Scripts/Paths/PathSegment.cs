using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
public class PathSegment : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public Transform curvePoint;
    public float width;
    public CameraPosition cameraPosition = CameraPosition.ForwardBackward;
    public bool bezier => curvePoint != null;

    public OptimalCameraParam cameraParam = new OptimalCameraParam { distance = 4f, followVantage = VantageType.Free };
#if UNITY_EDITOR
    public int segmentCount;
#endif
    public Line Value => new Line
    {
        pointA = startPoint.position,
        pointB = endPoint.position,
        width = width,
        curvePoint = bezier ? curvePoint.position : (startPoint.position +endPoint.position)/2,
        cameraPosition = cameraPosition,
        cameraParam = cameraParam
    };
#if UNITY_EDITOR
    public void OnDrawGizmosSelected(){
        var line = Value;
        Gizmos.color = Color.red; 
        float3 currPosition;
        float3 nextPosition = line.pointA;
        float3 prevNormal = line.NormalAt(0) ?? math.normalize(math.cross(line.pointB - line.pointA, math.up()));
        Gizmos.DrawLine(line.pointA + prevNormal * width , line.pointA);
        Gizmos.DrawLine(line.pointA - prevNormal * width, line.pointA);
        for (var i = 0; i < segmentCount; i++){
            currPosition = nextPosition;
            var nextT = (float)(i + 1)/ (float)segmentCount;
            nextPosition = line.At(nextT);
            Gizmos.DrawLine(currPosition, nextPosition);
            var nextNormal = line.NormalAt(nextT) ?? math.normalize(math.cross(line.pointB - line.pointA, math.up()));
            Gizmos.DrawLine(nextPosition + nextNormal * width, nextPosition);
            Gizmos.DrawLine(nextPosition - nextNormal * width, nextPosition);
        }
    }
    #endif
}