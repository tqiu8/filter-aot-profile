// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Mono.Profiler.Aot
{
    //
    // Represents the contents of an .aotprofile file created by the
    // AOT profiler
    //
    public class ProfileRecord
    {
        public ProfileRecord (int id)
        {
            Id = id;
        }
        
        // [JsonIgnore]
        public int Id {
            get; set;
        }
    }

    public class ModuleRecord : ProfileRecord
    {
        public ModuleRecord () : base (0)
        {
            Name = null!;
            Mvid = null!;
        }

        public ModuleRecord (int id, string name, string mvid) : base (id)
        {
            Name = name;
            Mvid = mvid;
            Types = new List<TypeRecord>();
        }

        public string Name {
            get; set;
        }

        public string Mvid {
            get; set;
        }

        public List<TypeRecord> Types {
            get; set;
        }

        public void AddType(TypeRecord type) {
            Types.Add(type);
        }

        public override string ToString ()
        {
            return Name;
        }
    }

    public class GenericInstRecord : ProfileRecord
    {
        public GenericInstRecord () : base (0)
        {
            Types = Array.Empty<TypeRecord> ();
        }

        public GenericInstRecord (int id, TypeRecord[] types) : base (id)
        {
            Types = types;
        }

        public TypeRecord[] Types {
            get; set;
        }

        public override string ToString ()
        {
            if (Types == null || Types.Length <= 0)
                return "";

            var sb = new StringBuilder ('<');
            var first = true;
            foreach (var type in Types) {
                if (!first)
                    sb.Append (", ");
                else
                    first = false;

                sb.Append (type.ToString ());
            }

            sb.Append ('>');

            return sb.ToString ();
        }
    }

    public class TypeRecord : ProfileRecord
    {
        public TypeRecord () : base (0)
        {
            Module = null!;
            Name = null!;
        }

        public TypeRecord (int id, ModuleRecord module, string name, GenericInstRecord? ginst) : base (id)
        {
            Module = module;
            Name = name;
            GenericInst = ginst;
            Methods = new List<MethodRecord>();
        }

        // [JsonIgnore]
        public ModuleRecord Module {
            get; set;
        }

        public string Name {
            get; set;
        }

        public List<MethodRecord> Methods {
            get; set;
        }

        [JsonIgnore]
        public string FullName {
            get {
                string prefix;

                if (Name.Length > 0 && Name [0] == '.')
                    prefix = Module!.ToString ();
                else
                    prefix = "";

                return $"{prefix}{Name}{GenericInst}";
            }
        }

        public GenericInstRecord? GenericInst {
            get; set;
        }

        public override string ToString ()
        {
            return FullName;
        }

        public void AddMethod(MethodRecord method) {
            Methods.Add(method);
        }
    }

    public class MethodRecord : ProfileRecord
    {
        public MethodRecord () : base (0)
        {
            Type = null!;
            Name = null!;
            Signature = null!;
        }

        public MethodRecord (int id, TypeRecord type, GenericInstRecord? ginst, string name, string sig, int param_count) : base (id)
        {
            Type = type;
            GenericInst = ginst;
            Name = name;
            Signature = sig;
            ParamCount = param_count;
        }
        // [JsonIgnore]
        public TypeRecord Type {
            get; set;
        }
        // [JsonIgnore]
        public GenericInstRecord? GenericInst {
            get; set;
        }
        public string Name {
            get; set;
        }
        public string Signature {
            get; set;
        }
        // [JsonIgnore]
        public int ParamCount {
            get; set;
        }
        [JsonIgnore]
        public string FullName {
            get {
                string prefix;

                if (Name.Length > 0 && Name [0] == '.')
                    prefix = Type!.ToString ();
                else
                    prefix = "";

                return $"{prefix}{Signature}{Name}{GenericInst}";
            }
        }

        public override string ToString ()
        {
            return $"{Signature.Replace ("(", $" {Type}:{Name} (")}";
        }
    }
}
