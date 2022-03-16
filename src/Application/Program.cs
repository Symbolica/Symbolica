using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Symbolica;
using Symbolica.Abstraction;
using Symbolica.Computation;
using Symbolica.Implementation;

static async Task<int> Handler(IConsole console, DirectoryInfo dir, string optLevel, Options options)
{
    var bytes = await Serializer.Serialize(dir, optLevel);
    var executor = new Executor(options);

    var (executedInstructions, exception) = await executor.Run<ContextHandle>(bytes);
    console.WriteLine($"Executed {executedInstructions} instructions.");

    if (exception != null)
    {
        console.WriteLine(exception.Message);

        if (exception is StateException stateException)
            console.WriteLine(string.Join(", ", stateException.Space.GetExample().Select(p => $"{p.Key}={p.Value}")));

        return 1;
    }

    console.WriteLine("No errors were found.");

    return 0;
}

var rootCommand = new RootCommand("Build and analyse a program from source with Symbolica.")
{
    new Argument<DirectoryInfo>(
            "dir",
            "The directory containing the .symbolica.sh file to be run.")
        .ExistingOnly(),
    new Argument<string>(
            "optLevel",
            () => "--O0",
            "The level at which to optimize the code. Corresponds to an LLVM opt level.")
        .FromAmong("--O0", "--O1", "--O2", "--O3", "--Os", "--Oz"),
    new Option<bool>(
        "--use-symbolic-addresses",
        "Controls whether the base addresses for allocations are treated symbolically. Enabling this helps to ensure that any pointer arithmetic in your code isn't 'getting lucky' and accidentally landing at some other valid memory. You can disable this if you aren't concerned about detecting invalid memory accesses. In which case we will emulate it with a simple incrementing constant allocator."),
    new Option<bool>(
        "--use-symbolic-continuations",
        "Controls whether the environment for non-local jumps is treated symbolically. Enabling this helps to ensure that your code does not depend on any implementation details of how environments are saved and restored for non-local jumps. You can disable this if you aren't concerned about verifying that. In which case we will emulate it with a simple incrementing constant lookup."),
    new Option<bool>(
        "--use-symbolic-garbage",
        "Controls whether the contents of uninitialized memory allocations are treated symbolically. Enabling this helps to ensure that your code isn't incorrectly assuming that uninitialized memory has some reliable constant value. You can disable this if you aren't concerned about using unitialized memory. In which case it will default to a value of zero.")
};

rootCommand.Handler = CommandHandler.Create(Handler);

return await rootCommand.InvokeAsync(args);
