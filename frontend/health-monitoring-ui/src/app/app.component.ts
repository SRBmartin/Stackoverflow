import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HealthCheckDto, HealthStatus } from './models/health-check.dto';
import { HealthService } from './services/health.service';

type ServiceFilter = 'All' | 'NotificationService' | 'Stackoverflow';
type StatusFilter  = 'All' | HealthStatus;

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Health Monitoring';
  loading = false;
  error: string | null = null;

  data: HealthCheckDto[] = [];

  serviceFilter: ServiceFilter = 'All';
  statusFilter: StatusFilter = 'All';

  readonly services: ServiceFilter[] = ['All', 'NotificationService', 'Stackoverflow'];

  private abortController?: AbortController;

  constructor(private health: HealthService) {}

  ngOnInit(): void {
    this.load();
  }

  ngOnDestroy(): void {
    this.abortController?.abort();
  }

  load(): void {
    this.abortController?.abort();
    this.abortController = new AbortController();

    this.loading = true;
    this.error = null;

    this.health.getLatest().subscribe({
      next: (resp) => {
        this.data = Array.isArray(resp) ? resp : [];
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load health data.';
        this.loading = false;
      }
    });
  }

  get filtered(): HealthCheckDto[] {
    return this.data.filter(row => {
      const serviceOk = this.serviceFilter === 'All' || row.ServiceName === this.serviceFilter;
      const statusOk  = this.statusFilter  === 'All' || (row.Status as string) === this.statusFilter;
      return serviceOk && statusOk;
    });
  }

  get healthyPct(): number {
    const total = this.data.length;
    if (!total) return 0;
    const healthy = this.data.filter(x => (x.Status as string) === 'Healthy').length;
    return Math.round((healthy / total) * 100);
  }

  get unhealthyPct(): number {
    const total = this.data.length;
    if (!total) return 0;
    return 100 - this.healthyPct;
  }

  trackByRow(_: number, item: HealthCheckDto): string {
    return `${item.ServiceName}|${item.DateTimeUtc}|${item.Status}`;
  }

  formatBelgrade(dtIso: string): string {
    try {
      const d = new Date(dtIso);
      return new Intl.DateTimeFormat('en-US', {
        year: 'numeric', month: '2-digit', day: '2-digit',
        hour: '2-digit', minute: '2-digit', second: '2-digit',
        hour12: false, timeZone: 'Europe/Belgrade'
      }).format(d);
    } catch {
      return dtIso;
    }
  }

  isHealthy(s: string): boolean {
    return s === 'Healthy';
  }
}
