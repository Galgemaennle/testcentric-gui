// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric GUI contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System.Collections.Generic;
using NUnit.Framework;
using TestCentric.Gui.Model.Fakes;
using TestCentric.Common;
using NUnit.Engine;

namespace TestCentric.Gui.Model
{
    public class TestModelCreationTests
    {
        [TestCase("dummy.dll")]
        [TestCase("dummy.dll", "--work=/Path/To/Directory")]
        [TestCase("dummy.dll", "--trace=Off")]
        [TestCase("dummy.dll", "--trace=Error", "--work=/Path/To/Directory")]
        [TestCase("dummy.dll", "--trace=Warning")]
        [TestCase("dummy.dll", "--trace=Info", "--work=/Path/To/Directory")]
        [TestCase("dummy.dll", "--trace=Debug")]
        [TestCase("dummy.dll", "--work=/Some/Directory", "==agents:32")]
        [TestCase("dummy.dll", "--agents:5")]
        [TestCase("dummy.dll", "--X86")]
        [TestCase("dummy.dll", "--param:X=5")]
        [TestCase("dummy.dll", "--param:X=5", "-p:Y=7")]
        public void CreateTestModel(params string[] args)
        {
            ITestEngine engine = new MockTestEngine();
            var options = new CommandLineOptions(args);
            var model = TestModel.CreateTestModel(engine, options);

            Assert.NotNull(model, "Unable to create TestModel");

            string expectedTraceLevel = options.InternalTraceLevel ?? "Off";
            string actualTraceLevel = model.PackageOverrides[EnginePackageSettings.InternalTraceLevel] as string;
            Assert.That(actualTraceLevel, Is.EqualTo(expectedTraceLevel));

            Assert.That(engine.WorkDirectory, Is.EqualTo(options.WorkDirectory));

            var checker = new PackageOverridesChecker(model);
            checker.CheckSetting(options.MaxAgents, EnginePackageSettings.MaxAgents);
            checker.CheckSetting(options.RunAsX86, EnginePackageSettings.RunAsX86);

            if (options.TestParameters.Count > 0)
            {
                string[] parms = new string[options.TestParameters.Count];
                int index = 0;
                foreach (string key in options.TestParameters.Keys)
                    parms[index++] = $"{key}={options.TestParameters[key]}";

                checker.CheckSetting(options.TestParameters, "TestParametersDictionary");
                checker.CheckSetting(string.Join(";", parms), "TestParameters");
            }
        }

        private class PackageOverridesChecker
        {
            private IDictionary<string, object> _dictionary;

            public PackageOverridesChecker(ITestModel model)
            {
                _dictionary = model.PackageOverrides;
            }

            public void CheckSetting(bool option, string key)
            {
                if (option || _dictionary.ContainsKey(key))
                {
                    Assert.That(_dictionary, Contains.Key(key));
                    Assert.That(_dictionary[key], Is.EqualTo(option));
                }
            }

            public void CheckSetting<T>(T option, string key)
            {
                if (option != null)
                {
                    Assert.That(_dictionary, Contains.Key(key));
                    Assert.That(_dictionary[key], Is.EqualTo(option));
                }
                else
                    Assert.That(_dictionary, Does.Not.ContainKey(key));
            }
        }
    }
}
