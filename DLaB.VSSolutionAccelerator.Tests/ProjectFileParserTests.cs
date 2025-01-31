using DLaB.VSSolutionAccelerator.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DLaB.VSSolutionAccelerator.Tests
{
    [TestClass]
    public class ProjectFileParserTests
    {
        [TestMethod]
        public void Parse()
        {
            var path = nameof(ProjectFileParserTests) + "." + nameof(Parse);
            var project = GetExampleProject();
            var parser = new ProjectFileParser(path, project);
            Assert.AreEqual(3, parser.Groups.Count(g => g.GroupType == GroupType.PropertyGroup));
            Assert.IsFalse(parser.GenerateAssemblyInfo);
            Assert.IsFalse(parser.SignAssembly);
            Assert.AreEqual("Debug;Release;DevDeploy", parser.Configurations);
            Assert.AreEqual("Xyz.Xrm.Plugin", parser.PackageName );
            Assert.AreEqual("Matt Barbour", parser.Authors );
            Assert.AreEqual("Xyz", parser.Company );
            Assert.AreEqual("Plugin with Dependent Assemblies", parser.AssemblyDescription );
            Assert.AreEqual(path, parser.Path );
            Assert.AreEqual("{4C25E9B5-9FA6-436c-8E19-B395D2A65FAF};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", parser.ProjectTypeGuids );
            Assert.AreEqual("..\\Xyz.Xrm\\Xyz.Xrm.csproj", parser.ProjectReferences.First() );
            Assert.AreEqual("..\\Xyz.Xrm2\\Xyz.Xrm2.csproj", parser.ProjectReferences.Last() );
            Assert.AreEqual(2, parser.ProjectReferences.Count);
            Assert.AreEqual("1.5.0.1", parser.PackageReferences["DLaB.Common"]);
            Assert.AreEqual("5.0.0.7", parser.PackageReferences["DLaB.Xrm"]);
            Assert.AreEqual("9.0.2.56", parser.PackageReferences["Microsoft.CrmSdk.CoreAssemblies"]);
            Assert.AreEqual(3, parser.PackageReferences.Count);
            Assert.AreEqual(13, parser.Groups.Count);
            Assert.IsNull(parser.Namespace);

            Assert.That.LinesAreEqual(project, parser.GetProject());
        }

        [TestMethod]
        public void LegacyProjectFormat_Should_Fail()
        {
            bool fail;
            var project = GetLegacyExampleProject();
            try
            {
                Assert.IsNotNull(new ProjectFileParser(nameof(ProjectFileParserTests) + "." + nameof(LegacyProjectFormat_Should_Fail), project));
                fail = true;
            }
            catch (Exception ex)
            {
                fail = !ex.Message.EndsWith("is not in the new SDK Style format!");
            }

            Assert.IsFalse(fail);
        }

        private string[] GetExampleProject()
        {
            return @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
	  <FileVersion>1.0.0.0</FileVersion>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <TargetFramework>net462</TargetFramework>
    <OutputType>Library</OutputType>
	  <LangVersion>12</LangVersion>
	  <SignAssembly>false</SignAssembly>
	  <Configurations>Debug;Release;DevDeploy</Configurations>
  </PropertyGroup>
	<!--
    NuGet pack and restore as MSBuild targets reference:
    https://docs.microsoft.com/en-us/nuget/reference/msbuild-targets
  -->
	<PropertyGroup>
		<PackageId>Xyz.Xrm.Plugin</PackageId>
		<Version>$(FileVersion)</Version>
		<Authors>Matt Barbour</Authors>
		<Company>Xyz</Company>
		<Description>Plugin with Dependent Assemblies</Description>
		<ProjectTypeGuids>{4C25E9B5-9FA6-436c-8E19-B395D2A65FAF};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>
	</PropertyGroup>
  <ItemGroup>
    <ProjectReference Include=""..\Xyz.Xrm\Xyz.Xrm.csproj"" />
    <ProjectReference Include=""..\Xyz.Xrm2\Xyz.Xrm2.csproj"" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include=""DLaB.Common"" Version=""1.5.0.1"" />
    <PackageReference Include=""DLaB.Xrm"" Version=""5.0.0.7"" />
    <PackageReference Include=""Microsoft.CrmSdk.CoreAssemblies"" Version=""9.0.2.56"" />
  </ItemGroup>
	
	<!-- Plugin Package Deployment Settings -->
	<PropertyGroup>
		<DeploymentConfigurationName>Release</DeploymentConfigurationName>
		<DeploymentOutDir>bin\$(DeploymentConfigurationName)\</DeploymentOutDir>
		<DeploymentPacAuthName>Xyz Dev</DeploymentPacAuthName>
		<GeneratePackageOnBuild>false</GeneratePackageOnBuild>
		<PluginPackageId>deadbeef-dead-beef-dead-beefdeadbeef</PluginPackageId>
	</PropertyGroup>
	<!-- Updates the File Version in memory so that the plugin dll is built with the correct version.  Apparently msBuild already has an in memory version of the cs proj, and updating the file as a pre build won't update the assembly version -->
	<Target Name=""IncrementFileVersion"" BeforeTargets=""PrepareForBuild"" Condition=""'$(Configuration)' == 'DevDeploy'"">
		<PropertyGroup>
			<FileVersionRevisionNext>$([MSBuild]::Add($([System.String]::Copy($(FileVersion)).Split('.')[3]), 1))</FileVersionRevisionNext>
			<FileVersion>$([System.String]::Copy($(FileVersion)).Split('.')[0]).$([System.String]::Copy($(FileVersion)).Split('.')[1]).$([System.String]::Copy($(FileVersion)).Split('.')[2]).$(FileVersionRevisionNext)</FileVersion>
		</PropertyGroup>
		<Message Text=""Setting Plugin Assembly FileVersion to: $(FileVersion) "" Importance=""high"" />
	</Target>
	<!-- PreBuild is required to update the csproj file on disk, and then be able to package the nupkg in the post build -->
	<Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
		<Exec Command=""if $(ConfigurationName) == DevDeploy (&#xD;&#xA;  echo Incrementing Version '$(SolutionDir)CodeGeneration\VersionUpdater.exe Increment --project $(ProjectPath)'&#xD;&#xA;  &quot;$(SolutionDir)CodeGeneration\VersionUpdater.exe&quot; Increment --project &quot;$(ProjectPath)&quot;&#xD;&#xA;&#xD;&#xA;  echo Deleting old nupkg file del &quot;$(ProjectDir)$(DeploymentOutDir)*.nupkg&quot; /q&#xD;&#xA;  del &quot;$(ProjectDir)$(DeploymentOutDir)*.nupkg&quot; /q&#xD;&#xA;&#xD;&#xA;  echo dotnet pack $(ProjectPath) --configuration $(DeploymentConfigurationName) --output &quot;$(ProjectDir)$(DeploymentOutDir)&quot;&#xD;&#xA;  dotnet pack $(ProjectPath) --configuration $(DeploymentConfigurationName) --output &quot;$(ProjectDir)$(DeploymentOutDir)&quot;&#xD;&#xA;&#xD;&#xA;  echo Switching To &quot;$(DeploymentPacAuthName)&quot; Auth Connection&#xD;&#xA;  PAC auth select -n &quot;$(DeploymentPacAuthName)&quot;&#xD;&#xA;&#xD;&#xA;  echo *** Pushing Plugin ***&#xD;&#xA;  echo PAC plugin push -id $(PluginPackageId) -pf &quot;$(ProjectDir)$(DeploymentOutDir)$(TargetName).$(FileVersion).nupkg&quot;&#xD;&#xA;  PAC plugin push -id $(PluginPackageId) -pf &quot;$(ProjectDir)$(DeploymentOutDir)$(TargetName).$(FileVersion).nupkg&quot;&#xD;&#xA;)"" />
	</Target>
</Project>".Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }

        private string[] GetLegacyExampleProject()
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
</Project>".Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }
    }
}
