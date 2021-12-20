﻿namespace ConcordiumSdk.Types;

public readonly struct TransactionHash
{
    private readonly string _formatted;
    private readonly byte[] _value;

    public TransactionHash(byte[] value)
    {
        if (value.Length != 32) throw new ArgumentException("value must be 32 bytes");
        _value = value;
        _formatted = Convert.ToHexString(value).ToLowerInvariant();
    }

    public TransactionHash(string value)
    {
        if (value.Length != 64) throw new ArgumentException("string must be 64 char hex string.");
        _value = Convert.FromHexString(value);
        _formatted = value.ToLowerInvariant();
    }

    public string AsString => _formatted;
    public byte[] AsBytes => _value;
    
    public override string ToString()
    {
        return _formatted;
    }
}