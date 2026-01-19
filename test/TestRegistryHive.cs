using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NFluent;
using NUnit.Framework;
using Serilog;

namespace CODA.RegistryParser.Test;

[TestFixture]
internal class TestRegistryHive
{
    [OneTimeSetUp]
    public void PreTestSetup()
    {
        //Log.Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Debug().CreateLogger();
    }

    [Test]
    public void CheckHardAndSoftParsingErrors()
    {
        var sam = new RegistryHive(@"./hives/SAM");
        sam.FlushRecordListsAfterParse = false;
        sam.ParseHive();

        Check.That(sam.SoftParsingErrors).IsEqualTo(0);
        Check.That(sam.HardParsingErrors).IsEqualTo(0);
    }

   
}