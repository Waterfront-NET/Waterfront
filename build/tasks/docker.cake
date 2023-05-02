#addin nuget:?package=Cake.Docker&version=1.2.0

Task("docker/build/dev-image").Does(() => {
  DockerBuild(new DockerImageBuildSettings {
    Tag = new[] {"waterfront"}
  }, ".");
});
