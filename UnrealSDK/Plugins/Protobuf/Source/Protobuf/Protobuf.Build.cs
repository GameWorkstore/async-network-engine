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
                Path.Combine(ThirdPartyPath,"abseil/include")
            }
		);
				
		
		PrivateIncludePaths.AddRange(
			new string[] {
                Path.Combine(ThirdPartyPath,"protobuf/include"),
                Path.Combine(ThirdPartyPath,"abseil/include")
            }
		);
			
		
		PublicDependencyModuleNames.AddRange(
			new string[]
			{
				"Core",
				// ... add other public dependencies that you statically link with here ...
			}
		);
		
		PrivateDependencyModuleNames.AddRange(
			new string[]
			{
				// ... add private dependencies that you statically link with here ...	
			}
		);
		
		
		DynamicallyLoadedModuleNames.AddRange(
			new string[]
			{
				// ... add any modules that your module loads dynamically here ...
			}
		);

        PublicSystemIncludePaths.AddRange(
            new string[]
            {
                Path.Combine(ThirdPartyPath,"protobuf/include"),
                Path.Combine(ThirdPartyPath,"abseil/include")
            }
        );

        PublicAdditionalLibraries.AddRange(
            new string[]
            {
            }
        );

        if(Target.bForceEnableRTTI)
        {
            bUseRTTI = true;
            PublicDefinitions.Add("GOOGLE_PROTOBUF_NO_RTTI=0");
        }
        else
        {
            bUseRTTI = false;
            PublicDefinitions.Add("GOOGLE_PROTOBUF_NO_RTTI=1");
        }
		PublicDefinitions.Add("HAVE_ZLIB=0");
        PublicDefinitions.Add("__cpluscplus=199711L");

        if (Target.Platform != UnrealTargetPlatform.Win64)
        {
            PublicDefinitions.Add("HAVE_PTHREAD");	
        }

        PublicDefinitions.Add("_CRT_SECURE_NO_WARNINGS");

        bEnableUndefinedIdentifierWarnings = false;
        bEnableExceptions = true;

        //ABSEIL
        PublicDefinitions.Add("ABSL_OPTION_USE_INLINE_NAMESPACE=0");
    }
}
