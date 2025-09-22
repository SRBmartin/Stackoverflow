import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HealthCheckDto, HealthStatus } from './models/health-check.dto';
import { HealthService } from './services/health.service';
import Chart from 'chart.js/auto';

type ServiceFilter = 'All' | 'NotificationService' | 'Stackoverflow';
type StatusFilter  = 'All' | HealthStatus;

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements AfterViewInit {
  title = 'Health Monitoring';
  loading = false;
  error: string | null = null;

  data: HealthCheckDto[] = [];

  private _serviceFilter: ServiceFilter = 'All';
  get serviceFilter(): ServiceFilter { return this._serviceFilter; }
  set serviceFilter(v: ServiceFilter) {
    this._serviceFilter = v;
    this.updateRateChart();
  }

  statusFilter: StatusFilter = 'All';
  readonly services: ServiceFilter[] = ['All', 'NotificationService', 'Stackoverflow'];

  private abortController?: AbortController;

  @ViewChild('rateChart') rateChartCanvas?: ElementRef<HTMLCanvasElement>;
  private rateChart?: Chart;

  constructor(private health: HealthService) {}

  ngOnInit(): void {
    this.load();
  }

  ngAfterViewInit(): void {
    this.initRateChart();
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
        this.updateRateChart();
      },
      error: () => {
        this.error = 'Failed to load health data.';
        this.loading = false;
        this.updateRateChart();
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

  private initRateChart(): void {
    if (!this.rateChartCanvas) return;
    const ctx = this.rateChartCanvas.nativeElement.getContext('2d');
    if (!ctx) return;

    const counts = this.computeCountsForService(this.serviceFilter);

    this.rateChart = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: ['Healthy', 'Unhealthy'],
        datasets: [
          {
            data: [counts.healthy, counts.unhealthy],
            backgroundColor: ['#16a34a', '#dc2626'],
            borderColor: ['#0a541f', '#7a1515'],
            borderWidth: 1
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'bottom',
            labels: {
              color: 'black',
              font: { weight: 700 }
            }
          },
          tooltip: { enabled: true }
        }
      }
    });
  }

  private updateRateChart(): void {
    if (!this.rateChart) {
      this.initRateChart();
      return;
    }
    const counts = this.computeCountsForService(this.serviceFilter);
    const ds = this.rateChart.data.datasets[0];
    ds.data = [counts.healthy, counts.unhealthy];
    this.rateChart.update();
  }

  private computeCountsForService(svc: ServiceFilter): { healthy: number; unhealthy: number } {
    const source = this.data.filter(d => svc === 'All' || d.ServiceName === svc);
    let healthy = 0, unhealthy = 0;
    for (const r of source) {
      if ((r.Status as string) === 'Healthy') healthy++;
      else unhealthy++;
    }
    return { healthy, unhealthy };
  }
}