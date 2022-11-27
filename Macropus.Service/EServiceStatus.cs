namespace Macropus.Service;

public enum EServiceStatus
{
	// ReSharper disable once InconsistentNaming
	UNKNOWN = -1,
	ReadyToStart,
	Starting,
	Started,
	Termination,
	Terminated,
	Error
}