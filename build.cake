///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Clean");
var configuration = Argument("configuration", "Release");

var settings = new DotNetPublishSettings
{
	Configuration = configuration,
	PublishSingleFile= true,
	IncludeNativeLibrariesForSelfExtract= true,
	EnableCompressionInSingleFile= true,
	OutputDirectory = "./dist/"
};

var deldirset = new DeleteDirectorySettings
{
	Recursive = true,
	Force = true
};



///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	// Executed BEFORE the first task.
	Information("Running tasks...");
});

Teardown(ctx =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Build")
	.Does(() => {
		DotNetPublish("./PipGUI V2.sln",settings);
	}
);

Task("Clean")
	.IsDependentOn("Build")
	.Does(() => {
		//CleanDirectory("./");
		DeleteDirectory("./bin",deldirset);
		if (DirectoryExists("./obj"))
		{
			DeleteDirectory("./obj",deldirset);
		}
		if (DirectoryExists("./.vs"))
		{
			DeleteDirectory("./.vs",deldirset);
		}
	}
);

RunTarget(target);