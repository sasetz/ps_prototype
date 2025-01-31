using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomQueue<T>
{
    private T[] _items;
    private int _count;

    public int Count => _count;
    public int Capacity => _items.Length;
    public bool IsEmpty => _count == 0;

    public CustomQueue(int capacity = 1024)
    {
        _items = new T[capacity];
        _count = 0;
    }

    public int Enqueue(T item)
    {
        if (_items.Length < _count + 1)
        {
            Debug.LogError("The array is too small for this object! A leak is possible");
        }
        _items[_count] = item;
        _count++;
        return _count - 1;
    }

    public T Dequeue()
    {
        if (_count <= 0)
        {
            Debug.LogError("not enough items in queue! Possibly a bug");
            Debug.DebugBreak();
            return default;
        }
        
        var dequeuedItem = _items[0];
        for (int i = 1; i < _count; i++)
        {
            _items[i - 1] = _items[i];
        }
        _count--;
        return dequeuedItem;
    }

    public ref T this[int index]
    {
        get
        {
            if (index < 0 || index >= _count) throw new IndexOutOfRangeException();
            return ref _items[index];
        }
    }

    public void Clear()
    {
        _count = 0;
    }
}
