#load ../data/*.cake

var mainBuildTask = Task("build");

foreach(var project in projects) {
  var task = Task(project.Task("build"))
  .IsDependentOn(project.Task("restore"))
  .WithCriteria(!args.NoBuild)
  .Does(() => {
    DotNetBuild(project.Path.ToString(), new DotNetBuildSettings {
      Configuration = args.Configuration,
      NoRestore = true,
      NoDependencies = true
    });

    if(args.Configuration is "Release" && !project.IsTest && !args.NoCopyArtifacts) {
      Verbose("Copying build output for project {0}", project.Name);

      var sourceFolder = project.Directory.Combine("bin/Release/net6.0");
      var targetArchive = paths.Libraries.CombineWithFilePath($"{project.Name}.{version.SemVer}.zip");

      Zip(sourceFolder, targetArchive);
    }
  });

  mainBuildTask.IsDependentOn(task);
}
