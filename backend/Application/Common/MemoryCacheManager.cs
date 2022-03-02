﻿namespace Application.Common;

public class MemoryCacheManager
{
    private readonly List<ICommittable> _items = new();

    public IMemoryCachedValue<T> CreateCachedValue<T>() where T : struct
    {
        var item = new MemoryCachedValue<T>();
        _items.Add(item);
        return item;
    }
    
    public void CommitEnqueuedUpdates()
    {
        foreach (var item in _items)
            item.Commit();
    }

    private class MemoryCachedValue<T> : ICommittable, IMemoryCachedValue<T> where T : struct
    {
        private T? _committedValue;
        private T? _enqueuedUpdatedValue;

        public T? GetCommittedValue()
        {
            return _committedValue;
        }

        public void EnqueueUpdate(T updatedValue)
        {
            _enqueuedUpdatedValue = updatedValue;
        }

        public void Commit()
        {
            _committedValue = _enqueuedUpdatedValue;
            _enqueuedUpdatedValue = null;
        }
    }

    private interface ICommittable
    {
        void Commit();
    }
}

public interface IMemoryCachedValue<T> where T : struct
{
    T? GetCommittedValue();
    void EnqueueUpdate(T updatedValue);
}
