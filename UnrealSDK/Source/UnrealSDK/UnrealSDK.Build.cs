// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class UnrealSDK : ModuleRules
{
	public UnrealSDK(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
	
		PublicDependencyModuleNames.AddRange(
            new string[]{
                "Core",
                "CoreUObject",
                "Engine",
                "InputCore",
                "HTTP",
                "Protobuf",
                "AsyncNetworkEngine"
            }
        );

		PrivateDependencyModuleNames.AddRange(new string[] {  });

        bEnableUndefinedIdentifierWarnings = false;
        bEnableExceptions = true;
    }
}
