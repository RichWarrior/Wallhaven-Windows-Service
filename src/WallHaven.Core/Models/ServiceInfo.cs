using System.Xml.Serialization;

namespace WallHaven.Core.Models
{
    [XmlRoot(ElementName = "ServiceInfo")]
	public class ServiceInfo
	{
		[XmlElement(ElementName = "RefreshMinute")]
		public int RefreshMinute { get; set; }

		[XmlElement(ElementName = "SearchLimit")]
		public int? SearchLimit { get; set; }
	}
}
