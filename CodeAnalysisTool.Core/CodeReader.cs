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

// Reads C# source files from the file system
public class CodeReader
{
    /*
     * Reads the content of the specified file.
     * filePath: Absolute or relative path to a .cs file.
     * Returns: The file contents as a string.
     * Throws FileNotFoundException when the file does not exist.
     * Throws ArgumentException when the path is null or empty.
     */
    public string ReadFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Source file not found: {filePath}", filePath);

        return File.ReadAllText(filePath);
    }
}
