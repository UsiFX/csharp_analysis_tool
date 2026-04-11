/*
 * Copyright (C) 2026 UsiFX. All rights reserved.
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace CodeAnalysisTool.Core;

//Executes a compiled in-memory assembly and captures its console output
public class ExecutionEngine
{
    /*
     * Executes the entry point of the supplied assembly and captures stdout/stderr.
     * assembly: A compiled assembly that contains a Main() entry point.
     * Returns: An ExecutionResult describing the outcome.
     */
    public ExecutionResult Execute(Assembly assembly)
    {
        var result = new ExecutionResult { CompilationSuccess = true };

        // Locate the entry-point method via the assembly manifest.
        MethodInfo? entryPoint = assembly.EntryPoint;

        if (entryPoint is null)
        {
            result.ExecutionSuccess = false;
            result.Errors = "No entry point (Main method) found in the compiled assembly.";
            return result;
        }

        // Redirect Console.Out so we can capture all printed output.
        var outputCapture = new StringBuilder();
        using var writer = new StringWriter(outputCapture);
        TextWriter originalOut = Console.Out;
        TextWriter originalError = Console.Error;

        Console.SetOut(writer);
        Console.SetError(writer);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            // Support both void Main() and int Main(string[]) signatures.
            object?[] args = entryPoint.GetParameters().Length > 0
                ? [Array.Empty<string>()]
                : [];

            entryPoint.Invoke(null, args);
            result.ExecutionSuccess = true;
        }
        catch (TargetInvocationException tie)
        {
            result.ExecutionSuccess = false;
            result.Errors = tie.InnerException?.ToString()
                           ?? tie.ToString();
        }
        catch (Exception ex)
        {
            result.ExecutionSuccess = false;
            result.Errors = ex.ToString();
        }
        finally
        {
            stopwatch.Stop();
            // Flush captured output before restoring the real streams.
            writer.Flush();
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }

        result.Output = outputCapture.ToString();
        result.ExecutionTime = stopwatch.Elapsed;
        return result;
    }
}
