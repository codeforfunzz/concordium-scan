﻿using System.Text.Json;
using System.Text.Json.Serialization;
using ConcordiumSdk.Utilities;

namespace ConcordiumSdk.NodeApi.Types.JsonConverters;

public class Level1UpdateConverter : JsonConverter<Level1Update>
{
    private readonly IDictionary<Type, string> _serializeMap;
    
    public Level1UpdateConverter()
    {
        _serializeMap = new Dictionary<Type, string>
        {
            { typeof(Level2KeysLevel1Update), "level2KeysUpdate" },
            { typeof(Level2KeysV1Level1Update), "level2KeysUpdateV1" },
            { typeof(Level1KeysLevel1Update), "level1KeysUpdate" },
        };
    }

    public override Level1Update? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.EnsureTokenType(JsonTokenType.StartObject);
        var startDepth = reader.CurrentDepth;
        
        var payloadTypeKey = reader.ReadString("typeOfUpdate");
        
        reader.ForwardReaderToPropertyValue("updatePayload");

        Level1Update? result;
        switch (payloadTypeKey)
        {
            case "level1KeysUpdate":
            {
                var content = JsonSerializer.Deserialize<HigherLevelAccessStructureLevel1Keys>(ref reader, options)!;
                result = new Level1KeysLevel1Update(content);
                break;
            }
            case "level2KeysUpdate":
            {
                var content = JsonSerializer.Deserialize<AuthorizationsV0>(ref reader, options)!;
                result = new Level2KeysLevel1Update(content);
                break;
            }
            case "level2KeysUpdateV1":
            {
                var content = JsonSerializer.Deserialize<AuthorizationsV1>(ref reader, options)!;
                result = new Level2KeysV1Level1Update(content);
                break;
            }
            default:
                throw new NotImplementedException($"Deserialization of update type '{payloadTypeKey}' is not implemented.");
        }

        reader.ForwardReaderToTokenTypeAtDepth(JsonTokenType.EndObject, startDepth);
        return result;
    }

    public override void Write(Utf8JsonWriter writer, Level1Update value, JsonSerializerOptions options)
    {
        if (!_serializeMap.TryGetValue(value.GetType(), out var typeOfUpdateString))
            throw new NotImplementedException($"type of update '{value.GetType()}' is not in the serialize map.");
        
        writer.WriteStartObject();
        writer.WriteString("typeOfUpdate", typeOfUpdateString);
        writer.WritePropertyName("updatePayload");
        object payloadValue = value switch
        {
            Level1KeysLevel1Update payload => payload.Content,
            Level2KeysLevel1Update payload => payload.Content,
            Level2KeysV1Level1Update payload => payload.Content,
            _ => throw new NotImplementedException($"Serialization of type {value.GetType()} is not implemented.")
        };
        JsonSerializer.Serialize(writer, payloadValue, payloadValue.GetType(), options);
        writer.WriteEndObject();
    }
}