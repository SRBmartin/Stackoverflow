export type HealthStatus = 'Healthy' | 'Unhealthy';

export interface HealthCheckDto {
  DateTimeUtc: string;
  Status: HealthStatus;
  ServiceName: 'NotificationService' | 'Stackoverflow';
}