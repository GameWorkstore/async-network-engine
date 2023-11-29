// Copyright 1998-2018 Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;
using System.IO;
public class Protobuf : ModuleRules
{
    private string ModulePath
    {
        get { return ModuleDirectory; }
    }

    private string ThirdPartyPath
    {
        get { return Path.Combine(ModulePath, "ThirdParty/"); }
    }

    public Protobuf(ReadOnlyTargetRules Target) : base(Target)
    {
        PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

        PublicIncludePaths.AddRange(
            new string[] {
                Path.Combine(ThirdPartyPath,"protobuf/include"),
                Path.Combine(ThirdPartyPath,"abseil/include"),
                Path.Combine(ThirdPartyPath,"utf8_range/include")
            }
        );

        PrivateDependencyModuleNames.AddRange(
            new string[]
            {
                "Core"
            }
        );

        if (Target.bForceEnableRTTI)
        {
            bUseRTTI = true;
            PrivateDefinitions.Add("GOOGLE_PROTOBUF_NO_RTTI=0");
        }
        else
        {
            bUseRTTI = false;
            PrivateDefinitions.Add("GOOGLE_PROTOBUF_NO_RTTI=1");
        }
        PrivateDefinitions.Add("HAVE_ZLIB=0");
        PrivateDefinitions.Add("__cpluscplus=199711L");

        PublicDefinitions.Add("__SIZEOF_INT128__=0");
        //PublicDefinitions.Add("PROTOBUF_USE_DLLS=0");
        //PublicDefinitions.Add("__GNUC__=0");
        //PublicDefinitions.Add(Target.Platform != UnrealTargetPlatform.Win64 ? "HAVE_PTHREAD=0" : "HAVE_PTHREAD=1");
        //PublicDefinitions.Add(Target.Platform != UnrealTargetPlatform.Linux ? "__clang__=1" : "__clang__=0");
        PrivateDefinitions.Add("_CRT_SECURE_NO_WARNINGS");
        //PublicDefinitions.Add("MSVC_STATIC_RUNTIME=0");
        //PublicDefinitions.Add("BUILD_SHARED_LIBS=0");

        bEnableUndefinedIdentifierWarnings = false;
        bEnableExceptions = true;

        //ABSEIL
        PublicDefinitions.Add("PROTOBUF_ENABLE_DEBUG_LOGGING_MAY_LEAK_PII=0");
    }
}
