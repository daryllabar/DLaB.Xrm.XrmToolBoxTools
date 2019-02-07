using System;
using DLaB.VSSolutionAccelerator.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.VSSolutionAccelerator.Tests
{
    [TestClass]
    public class ProjectFileParserTests
    {
        private string[] GetExampleProject()
        {
            return @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{2B294DBF-8730-436E-B401-8745FEA632FE}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>Xyz.Xrm.Plugin</RootNamespace>
    <AssemblyName>Xyz.Xrm.PluginAssembly</AssemblyName>
    <TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <Deterministic>true</Deterministic>
    <NuGetPackageImportStamp>
    </NuGetPackageImportStamp>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <CodeAnalysisRuleSet>..\Xyz.ruleset</CodeAnalysisRuleSet>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <CodeAnalysisRuleSet>..\Xyz.ruleset</CodeAnalysisRuleSet>
  </PropertyGroup>
  <PropertyGroup>
    <SignAssembly>true</SignAssembly>
  </PropertyGroup>
  <PropertyGroup>
    <AssemblyOriginatorKeyFile>Key.snk</AssemblyOriginatorKeyFile>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""Microsoft.Crm.Sdk.Proxy, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL"">
      <HintPath>..\packages\Microsoft.CrmSdk.CoreAssemblies.9.0.2.4\lib\net452\Microsoft.Crm.Sdk.Proxy.dll</HintPath>
    </Reference>
    <Reference Include=""System"" />
  </ItemGroup>
  <ItemGroup>
    <Compile Include=""Workflow\CreateGuidActivity.cs"" />
  </ItemGroup>
  <ItemGroup>
    <None Include=""packages.config"" />
  </ItemGroup>
  <Choose>
    <When Condition=""'$(VisualStudioVersion)' == '10.0' And '$(IsCodedUITest)' == 'True'"">
      <ItemGroup>
        <Reference Include=""Microsoft.VisualStudio.QualityTools.CodedUITestFramework, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL"">
          <Private>False</Private>
        </Reference>
        <Reference Include=""Microsoft.VisualStudio.TestTools.UITest.Common, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL"">
          <Private>False</Private>
        </Reference>
        <Reference Include=""Microsoft.VisualStudio.TestTools.UITest.Extension, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL"">
          <Private>False</Private>
        </Reference>
        <Reference Include=""Microsoft.VisualStudio.TestTools.UITesting, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL"">
          <Private>False</Private>
        </Reference>
      </ItemGroup>
    </When>
  </Choose>
  <Import Project=""..\Xyz.Xrm\Xyz.Xrm.projitems"" Label=""Shared"" />
  <Import Project=""..\Xyz.Xrm.Workflow\Xyz.Xrm.Workflow.projitems"" Label=""Shared"" />
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
  <Target Name=""EnsureNuGetPackageBuildImports"" BeforeTargets=""PrepareForBuild"">
    <PropertyGroup>
      <ErrorText>This project references NuGet package(s) that are missing on this computer. Use NuGet Package Restore to download them.  For more information, see http://go.microsoft.com/fwlink/?LinkID=322105. The missing file is {0}.</ErrorText>
    </PropertyGroup>
    <Error Condition=""!Exists('..\packages\ILRepack.2.0.16\build\ILRepack.props')"" Text=""$([System.String]::Format('$(ErrorText)', '..\packages\ILRepack.2.0.16\build\ILRepack.props'))"" />
  </Target>
  <PropertyGroup>
    <PostBuildEvent>REM PostBuild
    </PostBuildEvent>
  </PropertyGroup>
</Project>".Split(new[] {Environment.NewLine}, StringSplitOptions.None);
        }
        [TestMethod]
        public void Parse()
        {
            var project = GetExampleProject();
            var parser = new ProjectFileParser(project);
            Assert.AreEqual(3, parser.PrePropertyGroups.Count);
            Assert.AreEqual(5, parser.PropertyGroups.Count);
            Assert.AreEqual("Xyz.Xrm.PluginAssembly",parser.AssemblyName);
            Assert.AreEqual("Xyz.Xrm.Plugin", parser.Namespace);
            Assert.AreEqual(new Guid("2B294DBF-8730-436E-B401-8745FEA632FE"), parser.Id);
            Assert.AreEqual(3, parser.ItemGroups.Count);
            Assert.AreEqual(4, parser.ItemGroups[ProjectFileParser.ItemGroupTypes.Reference].Count);
            Assert.AreEqual(1, parser.ItemGroups[ProjectFileParser.ItemGroupTypes.Compile].Count);
            Assert.AreEqual(1, parser.ItemGroups[ProjectFileParser.ItemGroupTypes.None].Count);
            Assert.AreEqual(3, parser.Imports.Count);
            Assert.AreEqual(11, parser.PostImports.Count);

            Assert.That.LinesAreEqual(project, parser.GetProject());
        }
    }
}
