﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using Microsoft.Plugin.Program.Logger;
using Windows.Foundation.Metadata;
using Package = Windows.ApplicationModel.Package;

namespace Microsoft.Plugin.Program.Programs
{
    public class PackageWrapper : IPackage
    {
        public string Name { get; } = string.Empty;

        public string FullName { get; } = string.Empty;

        public string FamilyName { get; } = string.Empty;

        public bool IsFramework { get; }

        public bool IsDevelopmentMode { get; }

        public string InstalledLocation { get; } = string.Empty;

        public PackageWrapper()
        {
        }

        public PackageWrapper(string name, string fullName, string familyName, bool isFramework, bool isDevelopmentMode, string installedLocation)
        {
            Name = name;
            FullName = fullName;
            FamilyName = familyName;
            IsFramework = isFramework;
            IsDevelopmentMode = isDevelopmentMode;
            InstalledLocation = installedLocation;
        }

        public static PackageWrapper GetWrapperFromPackage(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            string path;
            try
            {
                path = package.InstalledLocation.Path;
            }
            catch (Exception e) when (e is ArgumentException || e is FileNotFoundException || e is DirectoryNotFoundException)
            {
                ProgramLogger.Exception($"Exception {package.Id.Name}", e, MethodBase.GetCurrentMethod().DeclaringType, "Path could not be determined");
                return new PackageWrapper(
                    package.Id.Name,
                    package.Id.FullName,
                    package.Id.FamilyName,
                    package.IsFramework,
                    package.IsDevelopmentMode,
                    string.Empty);
            }

            return new PackageWrapper(
                    package.Id.Name,
                    package.Id.FullName,
                    package.Id.FamilyName,
                    package.IsFramework,
                    package.IsDevelopmentMode,
                    path);
        }
    }
}
