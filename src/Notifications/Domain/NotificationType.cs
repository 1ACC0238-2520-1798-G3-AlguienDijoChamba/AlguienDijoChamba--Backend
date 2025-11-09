namespace AlguienDijoChamba.Api.Notifications.Domain;

public enum NotificationType
{
    // Creadas por el ciclo de vida de un servicio (estas están ligadas a un ServiceId)
    ProffesionalMessage,

    
    // Notificaciones informativas
    AdminMessage,      
    SubscriptionWarning, 
}