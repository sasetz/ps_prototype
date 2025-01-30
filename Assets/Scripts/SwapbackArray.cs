using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Random = UnityEngine.Random;

public class SwapbackArray<T>
{
    private T[] _items;
    private int _count;

    public int Count => _count;
    public int Capacity => _items.Length;
    public bool IsEmpty => _count == 0;

    public SwapbackArray(int capacity = 1024)
    {
        _items = new T[capacity];
        _count = 0;
    }

    public void Add(T item)
    {
        if (_items.Length < _count + 1)
        {
            Debug.LogError("The array is too small for this object! A leak is possible");
            var newArray = new T[_count * 2];
            for (int i = 0; i < _items.Length; i++)
            {
                newArray[i] = _items[i];
            }
            _items = newArray;
        }
        _items[_count] = item;
        _count++;
    }

    public bool RemoveAt(int index)
    {
        if (index < 0 || index >= _count)
        {
            Debug.LogError("index is incorrect!");
            Debug.DebugBreak();
            return false;
        }
        
        _count--;
        _items[index] = _items[_count]; // Swap with last item
        return true;
    }

    public int IndexOf(T item)
    {
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < _count; i++)
        {
            if (comparer.Equals(_items[i], item)) return i;
        }
        return -1;
    }

    public ref T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count) throw new IndexOutOfRangeException();
            return ref _items[index];
        }
        // set
        // {
        //     if (index < 0 || index >= _count) throw new IndexOutOfRangeException();
        //     _items[index] = value;
        // }
    }

    public void Clear()
    {
        _count = 0;
    }

    public T PopRandom()
    {
        if (_count <= 0) return default;
        int index = Random.Range(0, _count - 1);
        T item = _items[index];
        _count--;
        _items[index] = _items[_count];

        return item;
    }

    public void DeleteAll(Predicate<T> match)
    {
        int i = 0;
        while (i < _count)
        {
            if (match(_items[i]))
            {
                RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }
}
