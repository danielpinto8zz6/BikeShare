namespace Common.Models.Dtos;

public class DockStateChangeRequest
{
    public DockStateAction Action { get; set; }

    public Guid DockId { get; set; }
}

public enum DockStateAction
{
    Unknown = 0,
    Open, 
    Close
}