namespace Macropus.Service;

public enum EServiceStatus
{
	UNKNOWN = -1,
	ReadyToStart,
	Starting,
	Started,
	Termination,
	Terminated,
	Error
}