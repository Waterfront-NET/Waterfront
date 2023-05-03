#load ../data/*.cake

var mainRestoreTask = Task("restore");

foreach(var project in projects) {
  var task = Task(project.Task("restore"))
  .Does(() => {
    DotNetRestore(project.Path.ToString(), new DotNetRestoreSettings {
      NoDependencies = true
    });
  });

  mainRestoreTask.IsDependentOn(task);
}
