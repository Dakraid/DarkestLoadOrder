namespace DarkestLoadOrder.Serialization.Project
{
    using System;
    using System.Xml.Serialization;
    using System.Collections.Generic;
        [XmlRoot(ElementName ="Tags")]
        public class Tags {
            [XmlElement(ElementName ="Tags")]
            public string TagItems { get; set; }
        }

        [XmlRoot(ElementName ="project")]
        public class Project {
            [XmlElement(ElementName ="PreviewIconFile")]
            public string PreviewIconFile { get; set; }
            [XmlElement(ElementName ="ItemDescriptionShort")]
            public string ItemDescriptionShort { get; set; }
            [XmlElement(ElementName ="ModDataPath")]
            public string ModDataPath { get; set; }
            [XmlElement(ElementName ="Title")]
            public string Title { get; set; }
            [XmlElement(ElementName ="Language")]
            public string Language { get; set; }
            [XmlElement(ElementName ="UpdateDetails")]
            public string UpdateDetails { get; set; }
            [XmlElement(ElementName ="Visibility")]
            public string Visibility { get; set; }
            [XmlElement(ElementName ="UploadMode")]
            public string UploadMode { get; set; }
            [XmlElement(ElementName ="VersionMajor")]
            public string VersionMajor { get; set; }
            [XmlElement(ElementName ="VersionMinor")]
            public string VersionMinor { get; set; }
            [XmlElement(ElementName ="TargetBuild")]
            public string TargetBuild { get; set; }
            [XmlElement(ElementName ="Tags")]
            public Tags Tags { get; set; }
            [XmlElement(ElementName ="ItemDescription")]
            public string ItemDescription { get; set; }
            [XmlElement(ElementName ="PublishedFileId")]
            public string PublishedFileId { get; set; }
        }
}
