#addin nuget:?package=Cake.Docker&version=1.2.0
#load ../data/*.cake

const string IMAGE_TAG_NAME = "waterfront";

List<string> imageTags = new (args.Tags) {$"waterfront:{version.SemVer}"};

Task("docker/build")
.WithCriteria(args.Configuration is "Release")
.IsDependentOn(":server:build")
.Does(() => {
  var project = projects.Find(project => project.Name == "Waterfront.Server");

  Information("Building Docker images for project {0}", project.Name);

  var contextDir = project.Directory.ToString();
  var dockerFile = project.Directory.CombineWithFilePath("Dockerfile").ToString();
  var sourceDir = project.Directory.GetRelativePath(project.Directory
                                   .Combine("bin/Release/net6.0"))
                                   .ToString();

  Verbose(
    "Context directory: {0}\nDockerfile path: {1}\nImage source directory: {2}",
    contextDir,
    dockerFile,
    sourceDir
  );

  DockerBuild(new DockerImageBuildSettings {
    BuildArg = new[] {
      $"SOURCEDIR={sourceDir}"
    },
    File = dockerFile,
    Tag = imageTags.ToArray()
  }, contextDir);
});


Task("docker/export")
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
