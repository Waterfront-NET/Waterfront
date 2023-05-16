static BuildPaths paths;
paths = new BuildPaths{Root=Context.Environment.WorkingDirectory};

class BuildPaths {
  public DirectoryPath Root { get; init; }
  public DirectoryPath Src => Root.Combine("src");
  public DirectoryPath Test => Root.Combine("test");
  public FilePath Solution => Root.CombineWithFilePath("Waterfront.Server.sln");
  public DirectoryPath Libraries => Root.Combine("artifacts/lib");
  public DirectoryPath DockerImages => Root.Combine("artifacts/docker-images");
  public FilePath Dockerfile => Src.CombineWithFilePath("Waterfront.Server/Dockerfile");
}
