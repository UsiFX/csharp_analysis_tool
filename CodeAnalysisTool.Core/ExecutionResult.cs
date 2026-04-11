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

namespace CodeAnalysisTool.Core;

/*
 * Holds the result of a code compilation and execution attempt.
 */
public class ExecutionResult
{
    /* Standard output produced by the executed program. */
    public string Output { get; set; } = string.Empty;

    /* Compilation or runtime error messages. */
    public string Errors { get; set; } = string.Empty;

    /* Whether compilation succeeded (no compile-time errors). */
    public bool CompilationSuccess { get; set; }

    /* Whether execution completed without throwing an unhandled exception. */
    public bool ExecutionSuccess { get; set; }

    /* Wall-clock time spent executing the compiled program. */
    public TimeSpan ExecutionTime { get; set; }

    /* List of individual compilation diagnostic messages. */
    public IReadOnlyList<string> CompilationErrors { get; set; } = [];
}
