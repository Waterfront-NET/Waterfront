#addin nuget:?package=Cake.Incubator&version=8.0.0
#load paths.cake
#load args.cake

static List<BuildProject> projects;

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
