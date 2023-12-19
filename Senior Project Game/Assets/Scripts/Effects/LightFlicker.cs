
using UnityEngine;
using UnityEngine.Rendering.Universal;


/********************************************************************
        Useful for things like campfires and torches!
*/ 

  public class LightFlicker : MonoBehaviour
    {
        [SerializeField] public float m_PositionJitterScale = 0.09f;
    
        [SerializeField] public float m_RotationJitterScale = 2.3f;

        [SerializeField] public float m_IntensityJitterScale = 2f;

        [SerializeField] public float m_Timescale = 3f;
    
        private Vector3 m_InitialPosition;
        private float m_InitialIntensity;
        private Quaternion m_InitialRotation;

        private float m_XSeed;
        private float m_YSeed;
        private float m_ZSeed;

        private Light2D m_Light;

        private float m_flickerIntensityOffset = 1f;
        public float flickerIntensityOffset => m_flickerIntensityOffset;
        public float modifiedIntensity => m_InitialIntensity + m_flickerIntensityOffset;


        void Start()
        {
            Random.InitState(gameObject.GetInstanceID());
            m_XSeed = Random.value*248;
            m_YSeed = Random.value*248;
            m_ZSeed = Random.value*248;

            m_Light = GetComponent<Light2D>();
            m_InitialIntensity = m_Light.intensity;
            m_InitialPosition = transform.localPosition;
            m_InitialRotation = transform.localRotation;
        }

        void Update()
        {
            float x = Time.time * m_Timescale + m_XSeed;
            float y = Time.time * m_Timescale + m_YSeed;
            float z = Time.time * m_Timescale + m_ZSeed;

            Vector3 Noise = PerlinNoise3D(new Vector3(x, y, z), 2, 1);
            Noise = Noise * 2 - Vector3.one;

            transform.SetLocalPositionAndRotation(m_InitialPosition + Noise * m_PositionJitterScale, m_InitialRotation * Quaternion.Euler(Noise * m_RotationJitterScale)); // Neat

            m_flickerIntensityOffset = Noise.x * m_IntensityJitterScale;
            m_Light.intensity = modifiedIntensity;
        }

        private Vector3 PerlinNoise3D(Vector3 uv, int Octaves, float freq)
        {
            Vector3 output = Vector3.zero;
            for (int i = 0; i < Octaves; i++)
            {
                output.x += Mathf.PerlinNoise1D(uv.x * freq * (i + 1));
                output.y += Mathf.PerlinNoise1D(uv.y * freq * (i + 1));
                output.z += Mathf.PerlinNoise1D(uv.z * freq * (i + 1));
            
            }

            return output;
        }
    }