using System;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Text.Json;

namespace Flight_Tracker_System.Data;

public class JsonParser
{
    private static string _fileName = "Data/flights.json";


    public static T Deserialise<T>()
    {
        string jsonString = File.ReadAllText(_fileName);

        T? model = JsonSerializer.Deserialize<T>(jsonString);

        return model ?? throw new InvalidOperationException($"Failed to deserialise {_fileName}: result was null");
    }
}
