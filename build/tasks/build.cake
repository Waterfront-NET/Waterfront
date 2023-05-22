#load ../data/*.cake

Task("build")
.IsDependentOn("restore")
.WithCriteria(!args.NoBuild)
.DoesForEach(Project.Runtimes, runtime => {
  DotNetBuild(Project.Path.ToString(), new DotNetBuildSettings {
    Configuration = args.Configuration,
    NoRestore = true,
    Runtime = runtime,
    ArgumentCustomization = argBuilder => argBuilder.Append("--self-contained")
  });

  /* if(args.Configuration is "Release" && !args.NoCopyArtifacts) {

  } */
});

Task("publish")
.IsDependentOn("build")
.DoesForEach(Project.Runtimes, runtime => {
  DotNetPublish(Project.Path.ToString(), new DotNetPublishSettings {
    Configuration = args.Configuration,
    NoBuild = true,
    SelfContained = true,
    // PublishSingleFile = true,
    Runtime = runtime,
  });
});
