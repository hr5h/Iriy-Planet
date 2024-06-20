using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс для управления чанками
/// </summary>
public class ChunkManager
{
    #region Fields
    private float _chunkSize;

    private readonly Dictionary<Vector2Int, HashSet<Human>> humans = new Dictionary<Vector2Int, HashSet<Human>>();
    private readonly Dictionary<Vector2Int, HashSet<Monster>> monsters = new Dictionary<Vector2Int, HashSet<Monster>>();
    private readonly Dictionary<Vector2Int, HashSet<Item>> items = new Dictionary<Vector2Int, HashSet<Item>>();
    private readonly Dictionary<Vector2Int, HashSet<Shelter>> shelters = new Dictionary<Vector2Int, HashSet<Shelter>>();
    private readonly Dictionary<Vector2Int, HashSet<Bonfire>> bonfires = new Dictionary<Vector2Int, HashSet<Bonfire>>();
    private readonly Dictionary<Vector2Int, HashSet<BonfireOld>> bonfiresOld = new Dictionary<Vector2Int, HashSet<BonfireOld>>();
    private readonly Dictionary<Vector2Int, HashSet<DropItems>> dropItems = new Dictionary<Vector2Int, HashSet<DropItems>>();
    private readonly Dictionary<Vector2Int, HashSet<MapLabel>> mapLabels = new Dictionary<Vector2Int, HashSet<MapLabel>>();

    public IReadOnlyDictionary<Vector2Int, HashSet<Human>> Humans => humans;
    public IReadOnlyDictionary<Vector2Int, HashSet<Monster>> Monsters => monsters;
    public IReadOnlyDictionary<Vector2Int, HashSet<Item>> Items => items;
    public IReadOnlyDictionary<Vector2Int, HashSet<Shelter>> Shelters => shelters;
    public IReadOnlyDictionary<Vector2Int, HashSet<Bonfire>> Bonfires => bonfires;
    public IReadOnlyDictionary<Vector2Int, HashSet<BonfireOld>> BonfiresOld => bonfiresOld;
    public IReadOnlyDictionary<Vector2Int, HashSet<DropItems>> DropItems => dropItems;
    public IReadOnlyDictionary<Vector2Int, HashSet<MapLabel>> MapLabels => mapLabels;
    #endregion

    #region Constructors
    public ChunkManager(float chunkSize)
    {
        _chunkSize = chunkSize;
    }
    #endregion

    public Vector2Int CalculateCurrentChunk(Vector3 pos)
    {
        return Vector2Int.RoundToInt(pos / _chunkSize);
    }

    #region Change chunk methods
    public void ChangeChunk(ref Vector2Int previousChunk, ref Vector2Int currentChunk, Human @object)
    {
        Logger.Debug($"{@object.MyName} перешел из {previousChunk} в {currentChunk}");
        Add(currentChunk, @object);
        Remove(previousChunk, @object);
        previousChunk = currentChunk;
    }
    public void ChangeChunk(ref Vector2Int previousChunk, ref Vector2Int currentChunk, Monster @object)
    {
        Add(currentChunk, @object);
        Remove(previousChunk, @object);
        previousChunk = currentChunk;
    }
    public void ChangeChunk(ref Vector2Int previousChunk, ref Vector2Int currentChunk, Item @object)
    {
        Add(currentChunk, @object);
        Remove(previousChunk, @object);
        previousChunk = currentChunk;
    }
    public void ChangeChunk(ref Vector2Int previousChunk, ref Vector2Int currentChunk, Bonfire @object)
    {
        Add(currentChunk, @object);
        Remove(previousChunk, @object);
        previousChunk = currentChunk;
    }
    public void ChangeChunk(ref Vector2Int previousChunk, ref Vector2Int currentChunk, BonfireOld @object)
    {
        Add(currentChunk, @object);
        Remove(previousChunk, @object);
        previousChunk = currentChunk;
    }
    public void ChangeChunk(ref Vector2Int previousChunk, ref Vector2Int currentChunk, Shelter @object)
    {
        Add(currentChunk, @object);
        Remove(previousChunk, @object);
        previousChunk = currentChunk;
    }
    public void ChangeChunk(ref Vector2Int previousChunk, ref Vector2Int currentChunk, DropItems @object)
    {
        Add(currentChunk, @object);
        Remove(previousChunk, @object);
        previousChunk = currentChunk;
    }
    public void ChangeChunk(ref Vector2Int previousChunk, ref Vector2Int currentChunk, MapLabel @object)
    {
        Add(currentChunk, @object);
        Remove(previousChunk, @object);
        previousChunk = currentChunk;
    }
    #endregion

    #region Add element methods
    public void Add(Vector2Int coord, Human human) => AddOrUpdate(humans, coord, human);
    public void Add(Vector2Int coord, Monster monster) => AddOrUpdate(monsters, coord, monster);
    public void Add(Vector2Int coord, Bonfire bonfire) => AddOrUpdate(bonfires, coord, bonfire);
    public void Add(Vector2Int coord, BonfireOld bonfireOld) => AddOrUpdate(bonfiresOld, coord, bonfireOld);
    public void Add(Vector2Int coord, Item item) => AddOrUpdate(items, coord, item);
    public void Add(Vector2Int coord, Shelter shelter) => AddOrUpdate(shelters, coord, shelter);
    public void Add(Vector2Int coord, DropItems drop) => AddOrUpdate(dropItems, coord, drop);
    public void Add(Vector2Int coord, MapLabel label) => AddOrUpdate(mapLabels, coord, label);
    #endregion

    #region Remove element methods
    public void Remove(Vector2Int coord, Human human) => RemoveIfExists(humans, coord, human);
    public void Remove(Vector2Int coord, Monster monster) => RemoveIfExists(monsters, coord, monster);
    public void Remove(Vector2Int coord, Bonfire bonfire) => RemoveIfExists(bonfires, coord, bonfire);
    public void Remove(Vector2Int coord, BonfireOld bonfireOld) => RemoveIfExists(bonfiresOld, coord, bonfireOld);
    public void Remove(Vector2Int coord, Item item) => RemoveIfExists(items, coord, item);
    public void Remove(Vector2Int coord, Shelter shelter) => RemoveIfExists(shelters, coord, shelter);
    public void Remove(Vector2Int coord, DropItems drop) => RemoveIfExists(dropItems, coord, drop);
    public void Remove(Vector2Int coord, MapLabel drop) => RemoveIfExists(mapLabels, coord, drop);
    #endregion

    private void AddOrUpdate<T>(Dictionary<Vector2Int, HashSet<T>> dictionary, Vector2Int coord, T element)
    {
        if (!dictionary.ContainsKey(coord))
            dictionary[coord] = new HashSet<T>();

        dictionary[coord].Add(element);
    }

    private void RemoveIfExists<T>(Dictionary<Vector2Int, HashSet<T>> dictionary, Vector2Int coord, T element)
    {
        if (dictionary.ContainsKey(coord))
        {
            dictionary[coord].Remove(element);
            if (dictionary[coord].Count == 0)
                dictionary.Remove(coord);
        }
    }
}
