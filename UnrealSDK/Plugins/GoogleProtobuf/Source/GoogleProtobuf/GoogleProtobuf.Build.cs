// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;
using System.IO;

public class GoogleProtobuf : ModuleRules
{
	public GoogleProtobuf(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;

		PublicIncludePaths.Add(Path.Combine(ModuleDirectory, "Public"));
		
		PublicDependencyModuleNames.AddRange(new string[]{ "Core" });

		bEnableUndefinedIdentifierWarnings = false;
		bEnableExceptions = true;

		if (Target.Platform == UnrealTargetPlatform.Win64)
		{
			PublicDefinitions.Add("GOOGLE_PROTOBUF_NO_RTTI");
			PublicDefinitions.Add("__NVCC__");
			PublicDefinitions.Add("GOOGLE_PROTOBUF_INTERNAL_DONATE_STEAL_INLINE=0");
			PublicDefinitions.Add("GOOGLE_PROTOBUF_INTERNAL_DONATE_STEAL=0");
			PublicDefinitions.Add("GOOGLE_PROTOBUF_USE_UNALIGNED=0");
			//PublicDefinitions.Add("NDEBUG=0");
		}

		/*PrivateDependencyModuleNames.AddRange(
			new string[]
			{
				"CoreUObject",
				"Engine",
				"Slate",
				"SlateCore",
				"UMG"
			}
		);


		DynamicallyLoadedModuleNames.AddRange(
			new string[]
			{
				// ... add any modules that your module loads dynamically here ...
			}
		);*/
	}
}