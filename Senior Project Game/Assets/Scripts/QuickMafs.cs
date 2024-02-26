using System.Collections;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;

namespace Cyrcadian
{
    public static class QuickMafs 
    {
        public static float DistanceLength(Vector3 position_2,  Vector3 position_1)
        {
            NativeArray<float3> transformArray = new NativeArray<float3>(2, Allocator.TempJob);
            transformArray[0] = position_1;
            transformArray[1] = position_2;

            NativeArray<float> distanceArray = new NativeArray<float>(1, Allocator.TempJob);

            CalculateDistanceLength job = new CalculateDistanceLength()
            {
                transformArray = transformArray,
                distanceArray = distanceArray
            };

            JobHandle jobHandle = job.Schedule();
            jobHandle.Complete();

            float distance;
            distance = distanceArray[0];

            transformArray.Dispose();
            distanceArray.Dispose();
           
            return distance;
        }

        public static float DistanceLength(Vector3 position_2,  Vector3 position_1, out float distance)
        {
            NativeArray<float3> transformArray = new NativeArray<float3>(2, Allocator.TempJob);
            transformArray[0] = position_1;
            transformArray[1] = position_2;

            NativeArray<float> distanceArray = new NativeArray<float>(1, Allocator.TempJob);

            CalculateDistanceLength job = new CalculateDistanceLength()
            {
                transformArray = transformArray,
                distanceArray = distanceArray
            };

            JobHandle jobHandle = job.Schedule();
            jobHandle.Complete();

            distance = distanceArray[0];

            transformArray.Dispose();
            distanceArray.Dispose();

           
            return distance;
        }

        public static Vector3 Distance(Transform position_2, Transform position_1, out Vector3 distance)
        {
            NativeArray<float3> transformArray = new NativeArray<float3>(2, Allocator.TempJob);
            transformArray[0] = position_1.position;
            transformArray[1] = position_2.position;

            NativeArray<float3> distanceArray = new NativeArray<float3>(1, Allocator.TempJob);

            CalculateDistance job = new CalculateDistance()
            {
                transformArray = transformArray,
                distanceArray = distanceArray
            };

            JobHandle jobHandle = job.Schedule();
            jobHandle.Complete();

            distance = distanceArray[0];

            transformArray.Dispose();
            distanceArray.Dispose();

           
            return distance;
        }


        public static bool IsTargetCloser(Transform referencePoint, Transform target_1, Transform target_2)
        {
            JobHandle rangeJobHandle;
            NativeArray<bool> tempBoolArray = new NativeArray<bool>(1, Allocator.TempJob); 

            CalculateRangeDifference _rangeCalculationJob = new CalculateRangeDifference(){
                referencePosition = referencePoint.position,
                target_1 = target_1.position,
                target_2 = target_2.position,
                isCloser = tempBoolArray
            };
            rangeJobHandle = _rangeCalculationJob.Schedule();

            // Execute JOB on multi-thread
            rangeJobHandle.Complete();

            bool isInRange;
            isInRange = _rangeCalculationJob.isCloser[0];

            tempBoolArray.Dispose();      

            return isInRange;
        }
    }


        [BurstCompile]
        public struct CalculateRangeDifference : IJob
        {
            public float3 referencePosition;
            public float3 target_1;
            public float3 target_2;

            public NativeArray<bool> isCloser;

            public void Execute()
            {
                isCloser[0] = math.lengthsq(target_1 - referencePosition) < math.lengthsq(target_2 - referencePosition);
            }
        }


        [BurstCompile]
        public struct CalculateDistanceLength : IJob
        {
            [ReadOnly] public NativeArray<float3> transformArray;
            public NativeArray<float> distanceArray;

            public void Execute()
            {
                distanceArray[0] = math.distance(transformArray[1],transformArray[0]);
            }
        }

        [BurstCompile]
        public struct CalculateDistance : IJob
        {
            [ReadOnly] public NativeArray<float3> transformArray;
            public NativeArray<float3> distanceArray;

            public void Execute()
            {
                distanceArray[0] = transformArray[1] - transformArray[0];
            }
        }
}
