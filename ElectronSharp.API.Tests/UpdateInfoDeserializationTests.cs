using ElectronSharp.API.Entities;
using SocketIOClient.Newtonsoft.Json;
using static ElectronSharp.API.BridgeConnector;

namespace ElectronSharp.API.Tests
{
	public class UpdateInfoDeserializationTests
	{
		private readonly NewtonsoftJsonSerializer _serializer;

		public UpdateInfoDeserializationTests()
		{
			_serializer = new CamelCaseNewtonsoftJsonSerializer();
		}

		[Fact]
		public void NewtonsoftJsonSerializer_WhenNoReleaseNotes_DeserializesUpdateInfo()
		{
			string json = GetUpdateInfoJson("UpdateInfoWithoutReleaseNotes.json");
			var updateInfo = _serializer.Deserialize<UpdateInfo>(json);
			updateInfo.Should().NotBeNull();
			updateInfo.ReleaseNotes.Should().BeNull();
		}

		[Fact]
		public void NewtonsoftJsonSerializer_WhenReleaseNotesIsArray_DeserializesUpdateInfo()
		{
			string json = GetUpdateInfoJson("UpdateInfoWithReleaseNotesArray.json");
			var updateInfo = _serializer.Deserialize<UpdateInfo>(json);
			updateInfo.Should().NotBeNull();

			var notes = updateInfo.DeserializeReleaseNotes();
			notes.Should().NotBeNull();

			var notesArray = notes.ToArray();
			notesArray.Length.Should().Be(10);
			notesArray[2].Version.Should().Be("1.0.66");
			notesArray[2].Notes.Should().NotBeNull();
			notesArray[2].Notes.Count.Should().Be(6);
			notesArray[2].Notes[0].Should().Be("Implemented improvements when opening a configuration file.");
			notesArray[2].Notes[1].Should().Be("Fixed �Load configuration� functionality in TCT settings.");
			notesArray[2].Notes[2].Should().Be("Fixed issues with the modified parameters list.");
			notesArray[2].Notes[3].Should().Be("Fixed issues with authorized numbers and SMS event lists.");
			notesArray[2].Notes[4].Should().Be("Fixed internal hub page navigation.");
			notesArray[2].Notes[5].Should().Be("Fixed translation issues.");
		}

		[Fact]
		public void NewtonsoftJsonSerializer_WhenReleaseNotesIsString_DeserializesUpdateInfo()
		{
			string json = GetUpdateInfoJson("UpdateInfoWithReleaseNotesString.json");
			var updateInfo = _serializer.Deserialize<UpdateInfo>(json);
			updateInfo.Should().NotBeNull();
			updateInfo.ReleaseNotes.Should().Be("[ { \"version\": \"1.2.3\", \"notes\": [ \"First note.\", \"Second note.\"] } ]");

			var notes = updateInfo.DeserializeReleaseNotes();
			notes.Should().NotBeNull();

			var notesArray = notes.ToArray();
			notesArray.Length.Should().Be(1);
			notesArray[0].Version.Should().Be("1.2.3");
			notesArray[0].Notes.Should().NotBeNull();
			notesArray[0].Notes.Count.Should().Be(2);
			notesArray[0].Notes[0].Should().Be("First note.");
			notesArray[0].Notes[1].Should().Be("Second note.");
		}

		private static string GetUpdateInfoJson(string embeddedFileName)
		{
			string manifestResourceName = $"{typeof(UpdateInfoDeserializationTests).Namespace}.EmbeddedResources.{embeddedFileName}";
			using var stream = typeof(UpdateInfoDeserializationTests).Assembly.GetManifestResourceStream(manifestResourceName);
			if (stream is null)
				throw new InvalidOperationException($"Embedded file `{embeddedFileName}` was not found.");

			using var streamReader = new StreamReader(stream);
			return streamReader.ReadToEnd();
		}
	}

	/// <summary>
	/// Relase notes for one version.
	/// Defines future format for ReleaseNotes.json to avoid markdown parsing.
	/// </summary>
	/// <remarks>
	/// In future, when we'll switch to UpdateInfo.Release notes to string,
	/// this class with be moved to some Configurator project.
	/// </remarks>
	internal class VersionReleaseNotes
	{
		public string Version { get; set; } = string.Empty;

		public List<string> Notes { get; set; } = new();
	}

	internal static class UpdateInfoExtensions
	{
		private static readonly CamelCaseNewtonsoftJsonSerializer Serializer = new();

		public static IEnumerable<VersionReleaseNotes> DeserializeReleaseNotes(this UpdateInfo updateInfo) =>
			Serializer.Deserialize<List<VersionReleaseNotes>>(updateInfo.ReleaseNotes);
	}
}