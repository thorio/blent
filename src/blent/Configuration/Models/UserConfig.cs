namespace Blent.Configuration.Models
{
	public class UserConfig
	{
		public string AppDirectory { get; set; }
		public int Parallelism { get; set; }
		public Checks Checks { get; set; }
	}
}
