#addin nuget:?package=Cake.Docker&version=1.2.0
#load ../data/*.cake

Task("docker/build/dev-image").Does(() => {
  DockerBuild(new DockerImageBuildSettings {
    Tag = new[] {"waterfront"}
  }, ".");
});

Task(":server:docker/build-image")
.IsDependentOn(":server:build")
.Does(() => {
  DockerBuild(new DockerImageBuildSettings {
    BuildArg = new[] {
      "CONFIGURATION=" + args.Configuration
    },
    File = paths.Dockerfile.ToString(),
    Tag = new[] {$"waterfront:{version.SemVer}", "waterfront:latest"}
  }, paths.Src.Combine("Waterfront.Server").ToString());
});
