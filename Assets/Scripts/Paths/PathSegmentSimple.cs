using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
public class PathSegmentSimple : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 endPoint;
    public Vector3 curvePoint;
    public float width;
    public CameraPosition cameraPosition = CameraPosition.ForwardBackward;
    public OptimalCameraParam cameraParam = new OptimalCameraParam { distance = 4f, followVantage = VantageType.Free };
    public Line Value => new Line
    {
        pointA = transform.TransformPoint(startPoint),
        pointB = transform.TransformPoint(endPoint),
        width = width,
        curvePoint = transform.TransformPoint(curvePoint),
        cameraPosition = cameraPosition,
        cameraParam = cameraParam
    };
#if UNITY_EDITOR
    public int segmentCount=10;
    public void OnDrawGizmosSelected(){
        if(startPoint == null || endPoint == null) return;
        var line = Value;
        Gizmos.color = Color.red; 
        float3 currPosition;
        float3 nextPosition = line.pointA;
        float3 prevNormal = line.NormalAt(0) ?? math.normalize(math.cross(line.pointB - line.pointA, transform.up));
        Gizmos.DrawLine(line.pointA + prevNormal * width , line.pointA);
        Gizmos.DrawLine(line.pointA - prevNormal * width, line.pointA);
        for (var i = 0; i < segmentCount; i++){
            currPosition = nextPosition;
            var nextT = (float)(i + 1)/ (float)segmentCount;
            nextPosition = line.At(nextT);
            Gizmos.DrawLine(currPosition, nextPosition);
            var nextNormal = line.NormalAt(nextT) ?? math.normalize(math.cross(line.pointB - line.pointA, transform.up));
            Gizmos.DrawLine(nextPosition + nextNormal * width, nextPosition);
            Gizmos.DrawLine(nextPosition - nextNormal * width, nextPosition);
        }
    }
#endif
}
