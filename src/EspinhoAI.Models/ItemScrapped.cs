using System;
namespace EspinhoAI.Models
{
	public class ItemScrapped
	{
		public ItemScrapped()
		{
		}

		public string Url { get; set; }

		public string FilePath { get; set; }

		public string ParentUrl { get; set; }

		public DateTime DateScrapped { get; set; }
	}
}

