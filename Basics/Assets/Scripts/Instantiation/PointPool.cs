using UnityEngine;
using UnityEngine.Pool;

namespace Basics.Instantiation
{
    public class PointPool
    {
        private readonly Transform prefab;
        private readonly Transform parent;
        private readonly ObjectPool<Transform> pool;

        public PointPool(Transform prefab, Transform parent)
        {
            this.prefab = prefab;
            this.parent = parent;
            pool = new ObjectPool<Transform>(CreatePoint, GetPoint, ReleasePoint, DestroyPoint, true, 10000, 100000);
        }

        /// <summary>
        /// Get a point from the pool
        /// </summary>
        public Transform Get() => pool.Get();

        /// <summary>
        /// Release a point back into the pool
        /// </summary>
        public void Release(Transform point) => pool.Release(point);

        /// <summary>
        /// Create a point within the pool
        /// </summary>
        private Transform CreatePoint()
        {
            Transform point = Object.Instantiate(prefab, parent);
            return point;
        }

        /// <summary>
        /// Manipulate the point object upon retrieval
        /// </summary>
        private void GetPoint(Transform point)
        {
            point.gameObject.SetActive(true);
        }

        /// <summary>
        /// Manipulate the point object upon release
        /// </summary>
        private void ReleasePoint(Transform point)
        {
            point.gameObject.SetActive(false);
        }

        /// <summary>
        /// Destroy a point within the pool
        /// </summary>
        private void DestroyPoint(Transform point)
        {
            Object.Destroy(point.gameObject);
        }
    }
}
