#load ../data/*.cake

var mainRestoreTask = Task("restore");

Project.Runtimes.ForEach(runtime => {
  Task($"restore::{runtime}")
  .Does(() => DotNetRestore(new DotNetRestoreSettings {
    WorkingDirectory = Project.Directory,
    Runtime = runtime
  }));

  mainRestoreTask.IsDependentOn($"restore::{runtime}");
});
