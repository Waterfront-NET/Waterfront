#addin nuget:?package=Cake.Docker&version=1.2.0
#load ../data/*.cake

const string IMAGE_TAG_NAME = "waterfront";

List<string> imageTags = new (args.Tags) {$"waterfront:{version.SemVer}"};


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


/* Task("docker/export")
.DoesForEach(imageTags, tag => {

  var targetFile = paths.Root.Combine("artifacts/docker-images")
  .CombineWithFilePath(tag.Replace(':', '_') + ".tar.gz");

  Verbose("Exporting image {0} to file {1}", tag, targetFile);

  DockerSave(new DockerImageSaveSettings {
    Output = targetFile.ToString()
  }, tag);
});


Task("docker/push")
.DoesForEach(imageTags, tag => {
  DockerPush(new DockerImagePushSettings {

  }, tag);
});
 */
