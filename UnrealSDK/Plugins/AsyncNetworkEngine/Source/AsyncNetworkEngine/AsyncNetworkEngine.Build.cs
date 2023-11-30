// Copyright Epic Games, Inc. All Rights Reserved.
using System.IO;
using UnrealBuildTool;
public class AsyncNetworkEngine : ModuleRules
{
	public AsyncNetworkEngine(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

		PublicIncludePaths.AddRange(
			new string[]
			{
				Path.Combine(ModuleDirectory,"CppSDK/src"),
                Path.Combine(ModuleDirectory,"UnrealSDK"),
            }
		);

		PublicDependencyModuleNames.AddRange(
			new string[]
			{
				"Core",
				"Protobuf",
				"HTTP"
			}
		);
		
		PrivateDependencyModuleNames.AddRange(
			new string[]
			{
				"CoreUObject",
				"Engine",
				"Slate",
				"SlateCore",
			}
		);
    }
}
