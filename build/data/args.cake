static BuildArguments args;
args = new BuildArguments {
  Target = Argument("target", Argument("t", "build")),
  Configuration = Argument("configuration", Argument("c", "Debug")),
  NoBuild = HasArgument("no-build"),
  NoCopyArtifacts = HasArgument("no-copy-artifacts"),
  Tags = Arguments("tags", Array.Empty<string>() as ICollection<string>),
  CertificatePath = Argument("certificate-path", Argument("cert-path", Argument("cert", "localhost.crt"))),
  PrivateKeyPath = Argument("private-key-path", Argument("pk-path", Argument("private-key", "localhost.key")))
};

class BuildArguments {
  public string Target { get; init; }
  public string Configuration { get; init; }
  public bool NoBuild { get; init; }
  public bool NoCopyArtifacts { get; init; }
  public ICollection<string> Tags { get; init; }
  public FilePath CertificatePath { get; init; }
  public FilePath PrivateKeyPath { get; init; }
}
