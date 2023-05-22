#addin nuget:?package=Cake.Incubator&version=8.0.0
#load paths.cake
#load args.cake

Project.Name = "Waterfront";
Project.Path = paths.Src.CombineWithFilePath("Waterfront/Waterfront.csproj");

static class Project {
  public static readonly List<string> Runtimes = new() { "linux-musl-x64", "linux-x64", "win-x64" };
  public static string Name { get; set; }
  public static string Shortname => Name.ToLowerInvariant();
  public static FilePath Path { get; set; }
  public static DirectoryPath Directory => Path.GetDirectory();
  public static DirectoryPath BinDirectory() => Directory.Combine("bin");
  public static DirectoryPath BinDirectory(string configuration) {
    return BinDirectory().Combine(configuration);
  }
  public static DirectoryPath BinDirectory(string configuration, string framework) {
    return BinDirectory(configuration).Combine(framework);
  }
  public static FilePath PackagePath(string configuration, string version) {
    return BinDirectory(configuration).CombineWithFilePath($"{Name}.{version}.nupkg");
  }
  public static FilePath SymbolPackagePath(string configuration, string version) {
    return BinDirectory(configuration).CombineWithFilePath($"{Name}.{version}.snupkg");
  }
  public static DirectoryPath PublishDirectory(string configuration, string framework, string runtime = null) {
    var baseDir = BinDirectory(configuration, framework);

    if(string.IsNullOrEmpty(runtime)) {
      return baseDir.Combine("publish");
    }

    return baseDir.Combine(runtime).Combine("publish");
  }
}


/* static List<BuildProject> projects;

projects = new List<BuildProject> {
  new BuildProject {
    Name = "Waterfront",
    Path = paths.Src.CombineWithFilePath("Waterfront/Waterfront.csproj")
  }
};

class BuildProject {
  public string Name { get; init; }
  public FilePath Path { get; init; }
  public DirectoryPath Directory => Path.GetDirectory();
  public string Shortname => Name.Replace("Waterfront.", string.Empty).ToLowerInvariant();
  public bool IsTest => Name.EndsWith(".Tests");

  public string Task(string task) => $":{Shortname}:{task}";
}
 */
