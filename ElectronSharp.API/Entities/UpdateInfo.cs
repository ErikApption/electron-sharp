﻿using ElectronSharp.API.Entities.Serialization;
using System;

namespace ElectronSharp.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateInfo
    {
        /// <summary>
        /// The version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UpdateFileInfo[] Files { get; set; } = Array.Empty<UpdateFileInfo>();

        /// <summary>
        /// The release name.
        /// </summary>
        public string ReleaseName { get; set; }

        /// <summary>
        /// The release notes.
        /// </summary>
        [Newtonsoft.Json.JsonConverter(typeof(ReleaseNotesConverter))]
        public string ReleaseNotes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ReleaseDate { get; set; }

        /// <summary>
        /// The staged rollout percentage, 0-100.
        /// </summary>
        public int StagingPercentage { get; set; }
    }
}