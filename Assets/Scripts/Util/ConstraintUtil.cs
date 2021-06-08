using UnityEngine;
using Unity.Mathematics;
using AdaptiveCamera.Data;

namespace AdaptiveCamera.Util{
    public static class ConstraintUtil{
        public static ConstraintData MakeDistanceConstraint(float idealValue, float cost, float scale, float3 position, quaternion rotation){
            var output = new ConstraintData();
            output.type = ConstraintType.DISTANCE;
            output.position = position;
            output.rotation = rotation;
            output.cost = cost;
            output.idealFloat = idealValue;
            output.scale = scale;
            return output;
        }
        public static ConstraintData MakeDistanceConstraint(this Transform transform, float idealValue, float cost, float scale) => MakeDistanceConstraint(idealValue, cost, scale, transform.position, transform.rotation);

        public static ConstraintData MakeViewpointConstraint(float idealValue, float cost, float3 position, quaternion rotation){
            var output = new ConstraintData();
            output.type = ConstraintType.VIEWPOINT;
            output.position = position;
            output.rotation = rotation;
            output.cost = cost;
            output.idealFloat = idealValue;
            output.scale = math.PI;
            return output;
        }
        public static ConstraintData MakeViewpointConstraint(this Transform transform, float idealValue, float cost) => MakeViewpointConstraint(idealValue, cost, transform.position, transform.rotation);
        public static ConstraintData MakeVantageConstraint(float idealValue, float cost, float3 position, quaternion rotation)
        {
            var output = new ConstraintData();
            output.type = ConstraintType.VANTAGE;
            output.position = position;
            output.rotation = rotation;
            output.cost = cost;
            output.idealFloat = idealValue;
            output.scale = math.PI;
            return output;
        }
        public static ConstraintData MakeVantageConstraint(this Transform transform, float idealValue, float cost) => MakeVantageConstraint(idealValue, cost, transform.position, transform.rotation);
        public static ConstraintData MakeFOVConstraint(float idealValue, float cost, float3 position, quaternion rotation){
            var output = new ConstraintData();
            output.type = ConstraintType.FOV;
            output.position = position;
            output.rotation = rotation;
            output.cost = cost;
            output.idealFloat = idealValue;
            output.scale = 180;
            return output;
        }
        public static ConstraintData MakeFOVConstraint(this Transform transform, float idealValue, float cost) => MakeFOVConstraint(idealValue, cost, transform.position, transform.rotation);
        public static ConstraintData MakeCameraAngleConstraint(float idealValue, float cost, float3 position, quaternion rotation){
            var output = new ConstraintData();
            output.type = ConstraintType.CAMERA_ANGLE;
            output.position = position;
            output.rotation = rotation;
            output.cost = cost;
            output.idealFloat = idealValue;
            output.scale = math.PI;
            return output;
        }
        public static ConstraintData MakeCameraAngleConstraint(float cost, float2 targetForward, float maxChange, float2 cameraForward)
        {
            var result = MakeCameraAngleConstraint(0, cost, new float3(0, 0, 0), Quaternion.identity);
            var targetAngle = math.atan2(targetForward.x, targetForward.y);
            var cameraAngle = math.atan2(cameraForward.x, cameraForward.y);
            var testAngle = targetAngle - cameraAngle;
            if(testAngle > math.PI) testAngle -= 2 * math.PI;
            if(testAngle < -math.PI) testAngle += 2 * math.PI;
            maxChange = math.min(maxChange, math.abs(testAngle));
            if(testAngle > 0) cameraAngle+= maxChange;
            else cameraAngle -= maxChange;
            result.rotation = Quaternion.LookRotation(MathUtil.FromCameraAngle(new float2(cameraAngle, 0)), math.up());
            return result;
        }
        public static ConstraintData MakeCameraAngleConstraint(this Transform transform, float idealValue, float cost) => MakeCameraAngleConstraint(idealValue, cost, transform.position, transform.rotation);
        public static CameraConfiguration GetConfiguration(this Camera camera) => new CameraConfiguration
        {
            center = camera.transform.position,
        };
    }
}