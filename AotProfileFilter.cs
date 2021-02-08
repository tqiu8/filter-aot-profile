// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mono.Profiler.Aot;

public class AotProfileFilter
{
    public string Input { get; set; }
    public string Output { get; set; }
    public string[]? ExcludeMethods { get; set; }
    public bool Dump { get; set; }

    public AotProfileFilter(string input, string output, string[] excluded=null, bool dump=false)
    {
        Input = input;
        Output = output; 
        if (excluded != null)
        {
            ExcludeMethods = excluded;
        }
        if (dump)
        {
            Dump = dump;
        }
    }

    public string[] DumpMethods(ProfileData profile)
    {
        var methodNames = new List<string>();
        foreach (MethodRecord method in profile.Methods)
        {
            methodNames.Add(method.Name);
        }
        return methodNames.ToArray();
    }

    public bool Execute ()
    {
        var reader = new ProfileReader();
        ProfileData profile;
        string[] methodNames;
        using (FileStream stream = File.OpenRead(Input))
             profile = reader.ReadAllData(stream);
        if (ExcludeMethods != null)
        {
            var newMethods = new List<MethodRecord>();
            
            foreach (MethodRecord method in profile.Methods)
            {
                bool isFiltered = ExcludeMethods!.Any (e => method.Name == e);
                if (!isFiltered)
                    newMethods.Add (method);
            }
            profile.Methods = newMethods.ToArray();
            
        }
        if (Dump)
        {
            methodNames = DumpMethods(profile);
            File.WriteAllLines("methods.txt", methodNames.ToArray());
        }
        var writer = new ProfileWriter();
        using (FileStream outStream = File.Create(Output))
            writer.WriteAllData(outStream, profile);

        return true;
    }
}