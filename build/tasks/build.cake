#load ../data/*.cake

var mainBuildTask = Task("build").WithCriteria(!args.NoBuild);

Project.Runtimes.ForEach(runtime => {
  Task($"build::{runtime}")
  .IsDependentOn($"restore::{runtime}")
  .WithCriteria(!args.NoBuild)
  .Does(() => {
    Verbose("Building Waterfront with configuration: {0}, for runtime: {1}");

    DotNetBuild(Project.Path.ToString(), new DotNetBuildSettings {
      Configuration = args.Configuration,
      NoRestore = true,
      Runtime = runtime,
      ArgumentCustomization = argsBuilder => argsBuilder.Append("--self-contained")
    });
  });

  mainBuildTask.IsDependentOn($"build::{runtime}");
});
