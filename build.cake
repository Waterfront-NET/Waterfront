#load build/tasks/*.cake
#load build/data/*.cake


Setup(ctx => {
  EnsureDirectoryExists(paths.Libraries);
  EnsureDirectoryExists(paths.DockerImages);

  Verbose("Setting up version environment variables...");
  Environment.SetEnvironmentVariable("GitVersion_SemVer", version.SemVer);
  Environment.SetEnvironmentVariable("GitVersion_InformationalVersion", version.InformationalVersion);
});

RunTarget(args.Target);
