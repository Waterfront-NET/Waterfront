#load ../data/*.cake

var mainPublishTask = Task("publish");

Project.Runtimes.ForEach(runtime => {
  Task($"publish::{runtime}")
  .IsDependentOn($"build::{runtime}")
  .Does(() => {
    DotNetPublish(Project.Path.ToString(), new DotNetPublishSettings {
      Configuration = args.Configuration,
      NoBuild = true,
      SelfContained = true,
      Runtime = runtime
    });

    if(args.Configuration is "Release" && !args.NoCopyArtifacts) {
      Verbose("Compressing build artifacts...");

      var sourceFolder = Project.PublishDirectory(args.Configuration, "net6.0", runtime);
      var archiveName = $"Waterfront.{version.SemVer}.{runtime}.zip";
      var targetFilePath = paths.Libraries.CombineWithFilePath(archiveName);

      Debug("Source folder: {0}", sourceFolder);
      Debug("Archive name: {0}", archiveName);
      Debug("Target filepath: {0}", targetFilePath);

      Zip(sourceFolder, targetFilePath);
    }
  });

  mainPublishTask.IsDependentOn($"publish::{runtime}");
});
