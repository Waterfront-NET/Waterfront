#load ../data/*.cake

var mainBuildTask = Task("build");

foreach(var project in projects) {
  var task = Task(project.Task("build"))
  .IsDependentOn(project.Task("restore"))
  .Does(() => {
    DotNetBuild(project.Path.ToString(), new DotNetBuildSettings {
      Configuration = args.Configuration,
      NoRestore = true,
      NoDependencies = true
    });
  });

  mainBuildTask.IsDependentOn(task);
}
