#addin nuget:?package=Cake.Docker&version=1.2.0
#load ../data/*.cake

const string IMAGE_TAG_NAME = "waterfront";

List<string> imageTags = new (args.Tags.Select(tag => $"{apikeys.DockerId}/waterfront:{tag}")) {$"{apikeys.DockerId}/waterfront:{version.SemVer}"};


Task("docker/build")
.WithCriteria(args.Configuration is "Release")
.IsDependentOn("publish::linux-musl-x64")
.Does(() => {

  var contextDir = Project.Directory;
  var dockerFile = contextDir.CombineWithFilePath("Dockerfile");
  var sourceDir = contextDir.GetRelativePath(Project.PublishDirectory(args.Configuration, "net6.0", "linux-musl-x64"));

  DockerBuild(new DockerImageBuildSettings {
    BuildArg = new[] {
      $"SOURCEDIR={sourceDir}"
    },
    File = dockerFile.ToString(),
    Tag = imageTags.ToArray()
  }, contextDir.ToString());
});

Task("docker/export")
.DoesForEach(imageTags, tag => {
  var targetFile = paths.DockerImages.CombineWithFilePath(tag.Replace(":", "_").Replace("/", "_") + ".tar.gz");
  Verbose("Exporting image {0}", tag);
  Debug("Target archive file path: {0}", targetFile);

  DockerSave(new DockerImageSaveSettings {
    Output = targetFile.ToString()
  }, tag);
});

Task("docker/login")
.Does(() => DockerLogin(
    apikeys.DockerId,
    apikeys.DockerPassword
  )
);

Task("docker/push")
.IsDependentOn("docker/login")
.DoesForEach(imageTags, tag => DockerPush(tag));
