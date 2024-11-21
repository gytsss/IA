using System.Collections.Generic;
using System.Threading.Tasks;
using ECS.Implementation;
using ECS.Patron;
using FlockingECS.Components;
using FlockingECS.Systems;
using UnityEngine;

namespace FlockingECS
{
    public class ECSFlockingManager : MonoBehaviour
    {
        private const int MAX_OBJS_PER_DRAWCALL = 1000;
        [SerializeField] private GameObject target;
        public int entityCount = 100;
        public float velocity = 0.1f;
        public GameObject prefab;
        private List<uint> entities;
        private Material prefabMaterial;
        private Mesh prefabMesh;
        private Vector3 prefabScale;
        private PositionComponent<System.Numerics.Vector3> targetPositionComponent;
        private ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = 32
        };

        private void Start()
        {
            InitializeSystems();
            InitComponents();

            entities = new List<uint>();
            for (var i = 0; i < entityCount; i++)
            {
                var entityID = ECSManager.CreateEntity();
                ECSManager.AddComponent(entityID, new PositionComponent<System.Numerics.Vector3>(new System.Numerics.Vector3(0, -i, 0)));
                ECSManager.AddComponent(entityID,
                    new FlockComponent<System.Numerics.Vector3>(System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero));
                entities.Add(entityID);
            }

            targetPositionComponent = new PositionComponent<System.Numerics.Vector3>(new System.Numerics.Vector3(target.transform.position.x,
                target.transform.position.y, target.transform.position.z));

            prefabMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
            prefabMaterial = prefab.GetComponent<MeshRenderer>().sharedMaterial;
            prefabScale = prefab.transform.localScale;
        }


        private void Update()
        {
            ECSManager.Tick(Time.deltaTime);
        }

        private void LateUpdate()
        {
            var drawMatrix = new List<Matrix4x4[]>();
            var meshes = entities.Count;
            for (var i = 0; i < entities.Count; i += MAX_OBJS_PER_DRAWCALL)
            {
                drawMatrix.Add(new Matrix4x4[meshes > MAX_OBJS_PER_DRAWCALL ? MAX_OBJS_PER_DRAWCALL : meshes]);
                meshes -= MAX_OBJS_PER_DRAWCALL;
            }

            Parallel.For(0, entities.Count, parallelOptions, i =>
            {
                var position = ECSManager.GetComponent<PositionComponent<System.Numerics.Vector3>>(entities[i]);
                var pos =
                    new Vector3(position.Position.X, position.Position.Y, position.Position.Z);

                if (float.IsNaN(pos.x) || float.IsNaN(pos.y) || float.IsNaN(pos.z) ||
                    float.IsInfinity(pos.x) || float.IsInfinity(pos.y) || float.IsInfinity(pos.z))
                {
                    Debug.LogWarning($"Invalid position for entity {entities[i]}: {pos}");
                    pos = Vector3.zero;
                }

                drawMatrix[i / MAX_OBJS_PER_DRAWCALL][i % MAX_OBJS_PER_DRAWCALL]
                    .SetTRS(pos, Quaternion.identity, prefabScale);
            });
            foreach (var t in drawMatrix) Graphics.DrawMeshInstanced(prefabMesh, 0, prefabMaterial, t);
        }

        private void InitializeSystems()
        {
            ECSManager.AddSystem(new AlignmentSystem<System.Numerics.Vector3>());
            ECSManager.AddSystem(new CohesionSystem<System.Numerics.Vector3>());
            ECSManager.AddSystem(new DirectionSystem<System.Numerics.Vector3>());
            ECSManager.AddSystem(new SeparationSystem<System.Numerics.Vector3>());
            ECSManager.AddSystem(new MoveSystem<System.Numerics.Vector3>());
            ECSManager.InitSystems();
        }

        private void InitComponents()
        {
            ECSManager.AddComponentList(typeof(PositionComponent<System.Numerics.Vector3>));
            ECSManager.AddComponentList(typeof(FlockComponent<System.Numerics.Vector3>));
        }
    }
}