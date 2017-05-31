namespace ArchitectNow.Models.Options
{
	public class Features
    {
	    public bool UseHangfire { get; set; }
	    public bool EnableCompression { get; set; }
	    public bool DisableCaching { get; set; }
	    public bool UseRaygun { get; set; }
    }
}
