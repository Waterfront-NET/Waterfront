#addin nuget:?package=Cake.Docker&version=1.2.0
#load ../data/*.cake

string[] imageTags = {
  "waterfront:latest",
  $"waterfront:{version.SemVer}"
};

Task("docker/build-image")
.WithCriteria(args.Configuration is "Release")
.IsDependentOn(":server:build")
.Does(() => {
  var project = projects.Find(project => project.Name == "Waterfront.Server");

  DockerBuild(new DockerImageBuildSettings {
    BuildArg = new[] {
      $"CONFIGURATION={args.Configuration}"
    },
    File = project.Directory.CombineWithFilePath("Dockerfile").ToString(),
    Tag = imageTags
  }, project.Directory.ToString());
});

Task("docker/push-images")
.DoesForEach(imageTags, tag => {
  DockerPush(new DockerImagePushSettings {

  }, tag);
});
