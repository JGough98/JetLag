namespace JetLag.Scripts.Models;

public enum ExecutionPauseReason { Arrived, Delayed, Cancelled }

public record ExecutionOutcome(ExecutionPauseReason Reason, string StopName, int DelaySeconds, bool IsCancelled);
