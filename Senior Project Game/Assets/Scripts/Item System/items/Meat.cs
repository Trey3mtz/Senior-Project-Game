using UnityEngine;


using Cyrcadian;


    [CreateAssetMenu(fileName = "Meat", menuName = "2D Survial/Items/Meat")]
    public class Meat : Item
    {
        public override bool CanUse(Vector3Int target)
        {
            //  return GameManager.Instance?.Terrain != null && GameManager.Instance.Terrain.IsTillable(target);
            return true;
        }

        public override bool Use(Vector3Int target)
        {
            //  GameManager.Instance.Terrain.TillAt(target);
            return true;
        }
    }
