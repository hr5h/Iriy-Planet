using UnityEngine;

public class BlueForest : MonoBehaviour
{
    public GameObject treePref;
    public int treeCount;
    public float radius;
    public CircleCollider2D questArea;
    public Color treeCol;
    private WorldController worldController;

    public void TreeCreate()
    {
        Vector3 vect = transform.position + Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(radius * Random.Range(0f, 1f), 0, 0);
        if (!worldController.IsQuestArea(vect))
        {
            //TODO TreeCreate(). связать созданное дерево с чанком
            Sprite spr = worldController.plants[Random.Range(0, worldController.plants.Count)];
            var rot = Random.Range(0, 360f);
            Color col = treeCol + new Color(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 1);
            worldController.poolStorage.plantData.pool.Get().Init(vect, rot, spr, col);
            //plantsPosition.Add((vect, rot, spr, col));
            //myPlants.Push(worldController.CreatePlant(vect, rot, spr, col));
        }
    }

    public void Start()
    {
        worldController = WorldController.Instance;
        for (int i = 0; i < treeCount; i++)
        {
            TreeCreate();
        }
        questArea.radius = radius;
    }
}
