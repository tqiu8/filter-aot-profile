// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mono.Profiler.Aot;

public class JsonToAotProfile
{
    public string Input { get; set; }
    public string OutputDir { get; set; }

    public JsonToAotProfile(string input, string output)
    {
        Input = input;
        OutputDir = output;
    }
    
    public string[] GenerateRandomExclusions(int length, string methodsPath)
    {
        string[] methods = File.ReadAllLines(methodsPath);
        Console.WriteLine($"length of methods {methods.Length}");
        List<string> newMethods = new List<string>();
        var rand = new Random();
        for (var i = 0; i<=length; i++) {
            var ix = rand.Next(methods.Length);
            newMethods.Add(methods[ix]);
            Console.WriteLine(methods[ix]);
        }
        return newMethods.ToArray();
    }

    public bool Execute (string methodPath, int batch)
    {
        var reader = new ProfileReader();
        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.Preserve,
        };
        byte[] inputData = File.ReadAllBytes(Input!);
        var writer = new ProfileWriter();

        for(var i=0; i<batch; i++)
        {
            ProfileData data = JsonSerializer.Deserialize<ProfileData>(inputData, serializeOptions)!;
            var outputPath = Path.Combine(OutputDir, $"trimmed-{i}.profile");
            string[] excludedMethods = GenerateRandomExclusions(10, methodPath);
            using (FileStream outStream = File.Create(outputPath))
            {
                writer.WriteAllData(outStream, data, excludedMethods);
            }
            var trackingPath = Path.Combine(OutputDir, $"methods-{i}.txt");
            File.WriteAllLines(trackingPath, excludedMethods);
        }
        
        return true;
    }
}