#load build/tasks/*.cake
#load build/data/args.cake
#load build/data/paths.cake


Setup(ctx => {
  EnsureDirectoryExists(paths.Libraries);
  EnsureDirectoryExists(paths.DockerImages);
});

RunTarget(args.Target);
