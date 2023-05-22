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
  });

  mainPublishTask.IsDependentOn($"publish::{runtime}");
});
