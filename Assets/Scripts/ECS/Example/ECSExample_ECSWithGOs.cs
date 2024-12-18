using System.Collections.Generic;
using ECS.Implementation;
using UnityEngine;

public class ECSExample_ECSWithGOs : MonoBehaviour
{
    public int entityCount = 100;
    public float velocity = 10.0f;
    public float rotVelocity = 10.0f;
    public GameObject prefab;

    private Dictionary<uint, GameObject> entities;

    void Start()
    {
        ECSManager.Init();
        entities = new Dictionary<uint, GameObject>();
        for (int i = 0; i < entityCount; i++)
        {
            uint entityID = ECSManager.CreateEntity();
            ECSManager.AddComponent<PositionComponent>(entityID,
                new PositionComponent(0, -i, 0));
            ECSManager.AddComponent<VelocityComponent>(entityID,
                new VelocityComponent(velocity, Vector3.right.x, Vector3.right.y, Vector3.right.z));

            ECSManager.AddComponent<RotationComponent>(entityID, new RotationComponent(0, 0, 0));
            
            ECSManager.AddComponent<VelocityRotComponent>(entityID, new VelocityRotComponent(rotVelocity, Vector3.right.x, Vector3.right.y, Vector3.right.z));

            entities.Add(entityID, Instantiate(prefab, new Vector3(0, -i, 0), Quaternion.identity));
        }
    }

    void Update()
    {
        ECSManager.Tick(Time.deltaTime);
    }

    void LateUpdate()
    {
        foreach (KeyValuePair<uint, GameObject> entity in entities)
        {
            PositionComponent position = ECSManager.GetComponent<PositionComponent>(entity.Key);
            entity.Value.transform.SetPositionAndRotation(new Vector3(position.X, position.Y, position.Z),
                Quaternion.identity);
            
            RotationComponent rotation = ECSManager.GetComponent<RotationComponent>(entity.Key);
            entity.Value.transform.rotation = Quaternion.Euler(rotation.X, rotation.Y, rotation.Z);
        }
    }
}