// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mono.Profiler.Aot;

public class AotProfileToJson
{
    public string? Input { get; set; }
    public string? Output { get; set; }

    public AotProfileToJson(string input, string output)
    {
        Input = input;
        Output = output;
    }
    public bool Execute ()
    {
        var reader = new ProfileReader();
        ProfileData data;
        using (FileStream stream = File.OpenRead(Input!))
        {
            data = reader.ReadAllData(stream);
            // ModuleRecord[] modules = data.Modules;
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.Preserve,
                IgnoreNullValues = true,
                WriteIndented = true,
            };
            string s = JsonSerializer.Serialize(data, serializeOptions);
            File.WriteAllText(Output!, s);
        }
        return true;
    }
}