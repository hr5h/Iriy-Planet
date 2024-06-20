using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Хранилище сущностей, являющихся частью игрового мира
/// </summary>
public class EntityRepository
{
    #region Fields
    public IReadOnlyList<Human> Humans => humans;
    public IReadOnlyList<Monster> Monsters => monsters;
    public IReadOnlyList<Item> Items => items;
    public IReadOnlyList<Shelter> Shelters => shelters;
    public IReadOnlyList<Bonfire> Bonfires => bonfires;
    public IReadOnlyList<BonfireOld> BonfiresOld => bonfiresOld;
    public IReadOnlyList<DropItems> DropItems => dropItems;
    public IReadOnlyList<MapLabel> MapLabels => mapLabels;

    private readonly List<Human> humans = new List<Human>();
    private readonly List<Monster> monsters = new List<Monster>();
    private readonly List<Item> items = new List<Item>();
    private readonly List<Shelter> shelters = new List<Shelter>();
    private readonly List<Bonfire> bonfires = new List<Bonfire>();
    private readonly List<BonfireOld> bonfiresOld = new List<BonfireOld>();
    private readonly List<DropItems> dropItems = new List<DropItems>();
    private readonly List<MapLabel> mapLabels = new List<MapLabel>();
    #endregion

    #region Contructors
    public EntityRepository() { }
    #endregion

    #region Add element methods
    public void Add(Human human) => humans.Add(human);
    public void Add(Monster monster) => monsters.Add(monster);
    public void Add(Bonfire bonfire) => bonfires.Add(bonfire);
    public void Add(BonfireOld bonfireOld) => bonfiresOld.Add(bonfireOld);
    public void Add(Item item) => items.Add(item);
    public void Add(Shelter shelter) => shelters.Add(shelter);
    public void Add(DropItems drop) => dropItems.Add(drop);
    public void Add(MapLabel label) => mapLabels.Add(label);
    #endregion

    #region Remove element methods
    public void Remove(Human human) => humans.Remove(human);
    public void Remove(Monster monster) => monsters.Remove(monster);
    public void Remove(Bonfire bonfire) => bonfires.Remove(bonfire);
    public void Remove(BonfireOld bonfireOld) => bonfiresOld.Remove(bonfireOld);
    public void Remove(Item item) => items.Remove(item);
    public void Remove(Shelter shelter) => shelters.Remove(shelter);
    public void Remove(DropItems drop) => dropItems.Remove(drop);
    public void Remove(MapLabel label) => mapLabels.Remove(label);
    #endregion
}
