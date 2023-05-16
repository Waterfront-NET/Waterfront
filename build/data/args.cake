static BuildArguments args;
args = new BuildArguments {
  Target = Argument("target", Argument("t", "build")),
  Configuration = Argument("configuration", Argument("c", "Debug")),
  NoBuild = HasArgument("no-build"),
  NoCopyArtifacts = HasArgument("no-copy-artifacts"),
  Tags = Arguments("tags", Array.Empty<string>() as ICollection<string>)
};

class BuildArguments {
  public string Target { get; init; }
  public string Configuration { get; init; }
  public bool NoBuild { get; init; }
  public bool NoCopyArtifacts { get; init; }
  public ICollection<string> Tags { get; init; }
}
