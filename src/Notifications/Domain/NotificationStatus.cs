namespace AlguienDijoChamba.Api.Notifications.Domain;

public enum NotificationStatus
{
    // Estados del ciclo de vida de la solicitud/contrato
    Pending,
    InProgress,
    Accepted,
    Rejected,
    Completed,
    Discarded
}