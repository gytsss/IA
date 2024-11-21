using UnityEngine;

namespace ECS.Example
{
    public class MovementMonoBehaviour : MonoBehaviour
    {
        public float velocity;

        void LateUpdate()
        {
            transform.position += Vector3.right * velocity * Time.deltaTime;
        }
    }
}
