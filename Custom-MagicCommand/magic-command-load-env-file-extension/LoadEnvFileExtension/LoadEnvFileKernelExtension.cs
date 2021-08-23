using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace LoadEnvFileExtension
{
    public class LoadEnvFileKernelExtension : IKernelExtension
    {
        public Task OnLoadAsync(Kernel kernel)
        {
            var loadEnvCommand = new Command("#!env", "Load .env to .NET Notebook.")
            {
                new Option<string>(new[]{ "-f", "--file-path" }, "The .env file path"),
                new Option<string>(new[]{ "-n", "--var-name" }, "The variable name which contain the .env setting"),
            };

            loadEnvCommand.Handler = CommandHandler.Create(
                async (string filePath, string varName, KernelInvocationContext invocationContext) =>
                {
                    invocationContext.Display($"Load {filePath}");

                    var filePathString = string.IsNullOrEmpty(filePath) ? string.Empty : Path.Combine(Environment.CurrentDirectory, filePath);
                    Console.WriteLine($".env file path: {filePathString}");
                    var command = new SubmitCode($"var {varName} = dotenv.net.DotEnv.Fluent().WithEnvFiles(\"{filePathString.Replace(@"\", @"\\")}\").Read();");

                    await invocationContext.HandlingKernel.SendAsync(command);
                });

            kernel.AddDirective(loadEnvCommand);

            if (KernelInvocationContext.Current is { } context)
            {
                PocketView view = PocketViewTags.div(
                    PocketViewTags.code(nameof(LoadEnvFileExtension)), " is loaded. It adds loading feature for .env file.", PocketViewTags.br,
                    "Try it by running: ", PocketViewTags.code("#!env -f [file-path] -n [variable-name]")
                );

                context.Display(view);
            }

            return Task.CompletedTask;
        }
    }
}
