using System;
namespace EspinhoAI.Models.OCR
{

    using System;
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "PcGts", Namespace = "http://schema.primaresearch.org/PAGE/gts/pagecontent/2019-07-15")]
    public class PcGts
    {
        [XmlElement(ElementName = "Metadata")]
        public Metadata Metadata { get; set; }
        [XmlElement(ElementName = "Page")]
        public Page Page { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }
        [XmlAttribute(AttributeName = "pcGtsId")]
        public string PcGtsId { get; set; }
    }

    public class Metadata
    {
        [XmlElement(ElementName = "Creator")]
        public string Creator { get; set; }
        [XmlElement(ElementName = "Created")]
        public DateTime Created { get; set; }
        [XmlElement(ElementName = "LastChange")]
        public DateTime LastChange { get; set; }
        [XmlElement(ElementName = "Comments")]
        public string Comments { get; set; }
    }

    public class Page
    {
        [XmlElement(ElementName = "ReadingOrder")]
        public ReadingOrder ReadingOrder { get; set; }
        [XmlElement(ElementName = "TextRegion")]
        public List<TextRegion> TextRegions { get; set; }
        [XmlAttribute(AttributeName = "imageFilename")]
        public string ImageFilename { get; set; }
        [XmlAttribute(AttributeName = "imageWidth")]
        public int ImageWidth { get; set; }
        [XmlAttribute(AttributeName = "imageHeight")]
        public int ImageHeight { get; set; }
    }

    public class ReadingOrder
    {
        [XmlElement(ElementName = "OrderedGroup")]
        public OrderedGroup OrderedGroup { get; set; }
    }

    public class OrderedGroup
    {
        [XmlElement(ElementName = "RegionRefIndexed")]
        public List<RegionRefIndexed> RegionRefIndexed { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    public class RegionRefIndexed
    {
        [XmlAttribute(AttributeName = "index")]
        public int Index { get; set; }
        [XmlAttribute(AttributeName = "regionRef")]
        public string RegionRef { get; set; }
    }

    public class TextRegion
    {
        [XmlElement(ElementName = "Coords")]
        public Coords Coords { get; set; }
        [XmlElement(ElementName = "TextLine")]
        public List<TextLine> TextLine { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "TextEquiv")]
        public TextEquiv TextEquiv { get; set; }
    }

    public class Coords
    {
        [XmlAttribute(AttributeName = "points")]
        public string Points { get; set; }
    }

    public class TextLine
    {
        [XmlElement(ElementName = "Coords")]
        public Coords Coords { get; set; }
        [XmlElement(ElementName = "Word")]
        public List<Word> Word { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "production")]
        public string Production { get; set; }
        [XmlElement(ElementName = "TextEquiv")]
        public TextEquiv TextEquiv { get; set; }
    }

    public class Word
    {
        [XmlElement(ElementName = "Coords")]
        public Coords Coords { get; set; }
        [XmlElement(ElementName = "Glyph")]
        public List<Glyph> Glyph { get; set; }
        [XmlElement(ElementName = "TextEquiv")]
        public TextEquiv TextEquiv { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    public class Glyph
    {
        [XmlElement(ElementName = "Coords")]
        public Coords Coords { get; set; }
        [XmlElement(ElementName = "TextEquiv")]
        public TextEquiv TextEquiv { get; set; }
        [XmlElement(ElementName = "TextStyle")]
        public TextStyle TextStyle { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "ligature")]
        public bool Ligature { get; set; }
        [XmlAttribute(AttributeName = "production")]
        public string Production { get; set; }
    }

    public class TextEquiv
    {
        [XmlElement(ElementName = "Unicode")]
        public string Unicode { get; set; }
    }

    public class TextStyle
    {
        [XmlAttribute(AttributeName = "fontFamily")]
        public string FontFamily { get; set; }
        [XmlAttribute(AttributeName = "fontSize")]
        public int FontSize { get; set; }
        [XmlAttribute(AttributeName = "textColourRgb")]
        public int TextColourRgb { get; set; }
    }
}


