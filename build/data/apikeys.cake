
static BuildApiKeys apikeys;
apikeys = new BuildApiKeys {
  DockerId = EnvironmentVariable("DOCKER_ID", EnvironmentVariable("DOCKER_USERNAME")),
  DockerPassword = EnvironmentVariable("DOCKER_PASSWORD", EnvironmentVariable("DOCKER_TOKEN"))
};

class BuildApiKeys {
  public string DockerId { get; init }
  public string DockerPassword { get; init; }
}
