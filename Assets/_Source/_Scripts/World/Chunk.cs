using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chunk
{
    public WorldController worldController;
    public Vector2Int coord; //Координаты чанка в сетке
    public Vector3 pos; //Координаты чанка на сцене
    public float size; //Размер чанка
    public Chunk[] adjacentChunks = new Chunk[8]; //Соседние чанки

    private static Vector2Int[] neighborsGrid = new Vector2Int[8]
    {
        new Vector2Int(-1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1)
    };

    public Stack<Stone> myStones = new Stack<Stone>();
    public Stack<Plant> myPlants = new Stack<Plant>();
    public List<(Vector3, float, Sprite)> stonesPosition = new List<(Vector3, float, Sprite)>();
    public List<(Vector3, float, Sprite, Color)> plantsPosition = new List<(Vector3, float, Sprite, Color)>();

    private bool active;

    public Chunk(float chunkSize, Vector2Int chunkCoord)
    {
        worldController = WorldController.Instance; 
        size = chunkSize;
        coord = chunkCoord;
        active = true;
        for (var i = 0; i < 8; i++) //Получить массив соседних чанков
        {
            worldController.chunks.TryGetValue(coord + neighborsGrid[i], out adjacentChunks[i]);
        }

        pos = new Vector3(coord.x, coord.y) * size;
        var halfSize = size / 2;
        //Создание камней в пределах чанка
        var r = Random.Range(5, 20);
        for (var i = 0; i < r; i++)
        {
            Vector3 vect = pos + new Vector3(Random.Range(-halfSize, halfSize), Random.Range(-halfSize, halfSize));
            Sprite spr = worldController.stones[Random.Range(0, worldController.stones.Count)];
            float rot = Random.Range(0, 360.0f);
            stonesPosition.Add((vect, rot, spr));
            myStones.Push(worldController.SpawnStone(vect, rot, spr));
        }
        //Создание растений в пределах чанка
        r = Random.Range(0, 10);
        for (var i = 0; i < r; i++)
        {
            Vector3 vect = pos + new Vector3(Random.Range(-halfSize, halfSize), Random.Range(-halfSize, halfSize));
            if (!worldController.IsQuestArea(vect))
            {
                Sprite spr = worldController.plants[Random.Range(0, worldController.plants.Count)];
                float rot = Random.Range(0, 360.0f);
                Color col = Color.white - new Color(Random.Range(0, 0.5f), Random.Range(0, 0.5f), Random.Range(0, 0.5f), 0);
                plantsPosition.Add((vect, rot, spr, col));
                myPlants.Push(worldController.SpawnPlant(vect, rot, spr, col));
            }
        }
    }
    public void Activate()
    {
        if (!active)
        {
            active = true;
            for (var i = 0; i < stonesPosition.Count; i++)
            {
                myStones.Push(worldController.SpawnStone(stonesPosition[i].Item1, stonesPosition[i].Item2, stonesPosition[i].Item3));
            }
            for (var i = 0; i < plantsPosition.Count; i++)
            {
                myPlants.Push(worldController.SpawnPlant(plantsPosition[i].Item1, plantsPosition[i].Item2, plantsPosition[i].Item3, plantsPosition[i].Item4));
            }
        }
    }
    public void Deactivate()
    {
        if (active)
        {
            active = false;
            while (myStones.Count > 0)
            {
                worldController.poolStorage.stoneData.pool.Return(myStones.Pop());
            }
            while (myPlants.Count > 0)
            {
                worldController.poolStorage.plantData.pool.Return(myPlants.Pop());
            }
        }
    }
    public void MakeCurrent() //Сделать этот чанк текущим
    {
        //worldController.currentChunk = coord;
        for (var i = 0; i < 8; i++)
        {
            if (adjacentChunks[i] != null) //Активировать соседние чанки
            {
                adjacentChunks[i].Activate();
            }
            else //Если соседнего ещё чанка нет, то создать его
            {
                if (!worldController.chunks.ContainsKey(coord + neighborsGrid[i]))
                    worldController.AddChunk(coord + neighborsGrid[i]);
                adjacentChunks[i] = worldController.chunks[coord + neighborsGrid[i]];

            }
        }
    }
    public void UnMakeCurrent() //Перестать считать чанк текущим
    {
        for (var i = 0; i < 8; i++)
        {
            if (adjacentChunks[i] != null)
            {
                if ((worldController.currentChunk - neighborsGrid[i] - coord).magnitude > 1.5f) //Деактивировать чанки, которые не соседствуют с текущим
                    adjacentChunks[i].Deactivate();
            }
        }
    }
}
