namespace Common.Models.Dtos;

public class DockStateChangeRequest
{
    public DockState State { get; set; }

    public Guid DockId { get; set; }
}

public enum DockState
{
    Unknown = 0,
    Open, 
    Closed
}