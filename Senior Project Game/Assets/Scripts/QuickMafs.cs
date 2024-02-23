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
}
